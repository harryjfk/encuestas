using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositorios;
using Entity;
using PagedList;

namespace Domain.Managers
{
    public class EncuestaEmpresarialManager : GenericManager<EncuestaEmpresarial>
    {
        public EncuestaEmpresarialManager(GenericRepository<EncuestaEmpresarial> repository, Manager manager)
            : base(repository, manager)
        {
        }

        public EncuestaEmpresarialManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }
        public void GenerateCurrent(long idEstablecimiento)
        {
            var manager = Manager;
            var now = DateTime.Now.Subtract(TimeSpan.FromDays(DateTime.DaysInMonth(DateTime.Now.Year,DateTime.Now.Month)));
            var establecimiento = Manager.Establecimiento.Find(idEstablecimiento);
            if (establecimiento == null || establecimiento.Informante == null) return;
            var item = Get(t => t.Fecha.Month == now.Month && t.Fecha.Year == now.Year && t.IdEstablecimiento == idEstablecimiento).FirstOrDefault();
            if (item == null)
            {
                var encuesta = new EncuestaEmpresarial()
                {
                    IdEstablecimiento = idEstablecimiento,
                    EstadoEncuesta = EstadoEncuesta.NoEnviada,
                    IdInformante = establecimiento.IdInformante,
                    //IdAnalista = establecimiento.IdAnalista,
                    Fecha = now
                };

                var encuestaEstadisticaLast = Manager.EncuestaEstadistica.Get().OrderBy(x => x.Id).LastOrDefault();
                var encuestaEmpresarialLast = this.Get().OrderBy(x => x.Id).LastOrDefault();

                if (encuestaEstadisticaLast == null && encuestaEmpresarialLast == null)
                {
                    encuesta.Id = 1;
                }
                else if (encuestaEstadisticaLast != null && encuestaEmpresarialLast == null)
                {
                    encuesta.Id = encuestaEstadisticaLast.Id + 1;
                }
                else if (encuestaEstadisticaLast == null && encuestaEmpresarialLast != null)
                {
                    encuesta.Id = encuestaEmpresarialLast.Id + 1;
                }
                else if (encuestaEstadisticaLast != null && encuestaEmpresarialLast != null)
                {
                    if (encuestaEstadisticaLast.Id > encuestaEmpresarialLast.Id)
                    {
                        encuesta.Id = encuestaEstadisticaLast.Id + 1;
                    }
                    else
                    {
                        encuesta.Id = encuestaEmpresarialLast.Id + 1;
                    }
                }

                manager.EncuestaEmpresarial.Add(encuesta);
                manager.EncuestaEmpresarial.SaveChanges();
                var pregunats = manager.Pregunta.Get(t => t.Activado && t.IdEncuestaEmpresarial == null && !t.Valores.Any()).ToList();
                foreach (var p in pregunats)
                {
                    if (p.TipoPregunta == TipoPregunta.Permanente)
                    {
                        CreatePreguntaOfEncuesta(p, encuesta.Id);
                    }
                    else
                    {
                        var days=now.Subtract(p.Comienza.GetValueOrDefault()).TotalDays;
                        var mCount = (int)Math.Ceiling(days / 30);
                        if (mCount % p.Intervalo == 0)
                        {
                            CreatePreguntaOfEncuesta(p, encuesta.Id);
                        }
                    }
                }
            }
        }

        public override void UpdateKey(EncuestaEmpresarial element)
        {
            long id = 1;
            var last = Manager.Encuesta.Get().OrderBy(t => t.Id).LastOrDefault();
            if (last != null)
                id = last.Id + 1;
            element.Id = id;
        }

        public Pregunta CreatePreguntaOfEncuesta(Pregunta preg, long idEncuesta, int c = 0)
        {
            var manager = Manager;
            var t = new Pregunta()
            {
                Texto = preg.Texto,
                Comienza = preg.Comienza,
                Intervalo = preg.Intervalo,
                IdEncuestaEmpresarial = idEncuesta,
                orden=preg.orden,
                PreguntasObligatorias=preg.PreguntasObligatorias
            };
            manager.Pregunta.Add(t);
            manager.Pregunta.SaveChanges();
            foreach (var pr in preg.PosiblesRespuestas)
            {
                var r = new PosibleRespuesta()
                {
                    IdPregunta = t.Id,
                    TipoPosibleRespuesta = pr.TipoPosibleRespuesta,
                };
                manager.PosibleRespuesta.Add(r);
                manager.PosibleRespuesta.SaveChanges();
                foreach (var v in pr.Valores)
                {
                    long? pId = null;
                    if (v.Pregunta != null)
                    {
                        var prg = CreatePreguntaOfEncuesta(v.Pregunta, idEncuesta, ++c);
                        if (prg != null)
                            pId = prg.Id;
                    }
                    var vt = new Valor()
                    {
                        IdPosibleRespuesta = r.Id,
                        Texto = v.Texto,
                        IdPregunta = pId,
                        Personalizado=v.Personalizado,
                        texto_personalizado = v.texto_personalizado
                    };
                    manager.Valor.Add(vt);
                    manager.Valor.SaveChanges();

                }
            }
            return t;
        }

        public EncuestaEmpresarial Enviar(EncuestaEmpresarial encuesta, bool enviar)
        {
            var manager = Manager;
            var current = manager.EncuestaEmpresarial.Find(encuesta.Id);
            if (current == null)
                return null;
            foreach (var pr in encuesta.Preguntas)
            {
                GetValue(manager, pr);
            }

            //SALVAR CAMBIOS
            if (enviar)
            {
                current.fecha_ultimo_envio = DateTime.Now;
                current.EstadoEncuesta = EstadoEncuesta.Validada;
            }
            manager.EncuestaEmpresarial.Modify(current);
            manager.EncuestaEmpresarial.SaveChanges();
            return current;
        }

        public List<string> ValidarEncuesta(EncuestaEmpresarial encuesta)
        {
            var list = new List<string>();
            foreach (var pregunta in encuesta.Preguntas)
            {
                var valid = ValidatePregunta(pregunta);
                 if (!valid)
                {
                    list.Add(string.Format("Debe seleccionar un valor para la pregunta {0}",pregunta.Texto));
                }
                foreach (var valor in pregunta.Valores)
                {
                    if (valor.Seleccionado)
                    {
                        if (valor.Pregunta != null)
                        {
                            var valid2 = ValidatePregunta(valor.Pregunta);
                            if (!valid2)
                            {
                                list.Add(string.Format("Debe completar la pregunta {0}:{1}", pregunta.Texto,valor.Texto));
                            }

                        }
                        var personalizado = false;
                        var temp = Manager.Valor.Find(valor.Id);
                        if (temp != null)
                            personalizado = temp.Personalizado;
                        if (personalizado)
                        {
                            if (string.IsNullOrEmpty(valor.texto_personalizado) ||
                                string.IsNullOrWhiteSpace(valor.texto_personalizado))
                            {
                                list.Add(string.Format("Debe completar la pregunta {0}:{1}", pregunta.Texto, valor.Texto));
                            }
                        }
                    }
                  
                }
               
            }
            return list;
        }

        private bool ValidatePregunta(Pregunta pregunta)
        {
            return pregunta.Valores.Any(valor => valor.Seleccionado);
        }

        private static void GetValue(Manager manager, Pregunta pr)
        {
            var pregunta = manager.Pregunta.Find(pr.Id);
            if (pregunta != null)
            {
                foreach (var vl in pr.Valores)
                {
                    var valor = manager.Valor.Find(vl.Id);
                    if (valor != null)
                    {
                        valor.Seleccionado = vl.Seleccionado;
                        valor.texto_personalizado = vl.texto_personalizado;
                        if (vl.Pregunta != null)
                            GetValue(manager, vl.Pregunta);
                        manager.Valor.Modify(valor);
                        manager.Valor.SaveChanges();
                    }
                }
            }
        }

        public IPagedList GetAsignadosInformante(Query<EncuestaEmpresarial> query)
        {
            var estab = new long?();
            var info = new long?();
            if (query.Criteria != null)
            {
                if (query.Criteria.IdEstablecimiento != 0)
                    estab = query.Criteria.IdEstablecimiento;
                if (query.Criteria.IdInformante != 0)
                    if (query.Criteria.IdInformante != null) info = (long)query.Criteria.IdInformante;
            }
            var temp = Repository.Get(query.Filter, null, query.Order)
                .Where(t => t.IdEstablecimiento == estab
                && t.IdInformante == info);
            if (query.Paginacion != null)
            {
                var list = temp.ToPagedList(query.Paginacion.Page, query.Paginacion.ItemsPerPage);
                query.Elements = list;
                return list;
            }
            else
            {
                var encuestas = temp as EncuestaEmpresarial[] ?? temp.ToArray();
                var list = encuestas.ToPagedList(1, encuestas.Any() ? encuestas.Count() : 1);
                query.Elements = list;
                return list;
            }

        }

        public IPagedList GetAsignadosAnalista(Query<EncuestaEmpresarial> query)
        {
            var estab = new long?();
            var ana = new long?();
            if (query.Criteria != null)
            {
                if (query.Criteria.IdEstablecimiento != 0)
                    estab = query.Criteria.IdEstablecimiento;
                if (query.Criteria.IdAnalista != 0)
                    if (query.Criteria.IdAnalista != null) ana = (long)query.Criteria.IdAnalista;
            }
            var temp = Repository.Get(query.Filter, null, query.Order)
                .Where(t => t.IdEstablecimiento == estab
                && t.IdAnalista == ana && (t.EstadoEncuesta != EstadoEncuesta.NoEnviada));
            if (query.Paginacion != null)
            {
                var list = temp.ToPagedList(query.Paginacion.Page, query.Paginacion.ItemsPerPage);
                query.Elements = list;
                return list;
            }
            else
            {
                var encuestas = temp as EncuestaEmpresarial[] ?? temp.ToArray();
                var list = encuestas.ToPagedList(1, encuestas.Any() ? encuestas.Count() : 1);
                query.Elements = list;
                return list;
            }
        }

        public void Observar(long id, string observacion)
        {
            var encuesta = Manager.EncuestaEmpresarial.Find(id);
            if (encuesta == null) return;
            encuesta.EstadoEncuesta = EstadoEncuesta.Observada;
            encuesta.Justificacion = observacion;
            Manager.EncuestaEmpresarial.Modify(encuesta);
            Manager.EncuestaEmpresarial.SaveChanges();
        }

        public void Validar(long id)
        {
            var encuesta = Manager.EncuestaEmpresarial.Find(id);
            if (encuesta == null) return;
            encuesta.EstadoEncuesta = EstadoEncuesta.Validada;
            Manager.EncuestaEmpresarial.Modify(encuesta);
            Manager.EncuestaEmpresarial.SaveChanges();
        }
    }
}
