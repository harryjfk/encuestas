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

namespace Domain.Managers
{
    public class PosibleRespuestaManager : GenericManager<PosibleRespuesta>
    {
        public PosibleRespuestaManager(GenericRepository<PosibleRespuesta> repository, Manager manager)
            : base(repository, manager)
        {
        }

        public PosibleRespuestaManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }

        public override List<string> Validate(PosibleRespuesta element)
        {
            var list = base.Validate(element);
            list.Required(element, t => t.IdPregunta, "Pregunta");
            return list;
        }
        public override OperationResult<PosibleRespuesta> Add(PosibleRespuesta element)
        {
            var manager = Manager;
            var list = element.Valores != null ? element.Valores.Where(t => t != null).Select(t => new Valor() { Texto = t.Texto, IdPregunta = (t.IdPregunta != 0 ? t.IdPregunta : null) }).ToList() : new List<Valor>();
            if (element.Valores != null)
                element.Valores.Clear();
            var pregunta = manager.Pregunta.Find(element.IdPregunta);
            if (pregunta != null)
            {
                while (pregunta.PosiblesRespuestas != null && pregunta.PosiblesRespuestas.Any())
                {
                    var tr = pregunta.PosiblesRespuestas.FirstOrDefault();
                    manager.PosibleRespuesta.Delete(tr);
                }
                manager.Pregunta.Modify(pregunta);
                manager.Pregunta.SaveChanges();
            }
            var op = base.Add(element);
            SaveChanges();
            foreach (var valor in list)
            {
                valor.IdPosibleRespuesta = op.Entity.Id;
                manager.Valor.Add(valor);
                manager.Valor.SaveChanges();
            }
            return op;
        }

        public override OperationResult<PosibleRespuesta> Modify(PosibleRespuesta el, params string[] properties)
        {
            var manager = Manager;
            var list = el.Valores != null ? el.Valores.Where(t => t != null).Select(t => new Valor()
            {
                Personalizado = t.Personalizado, 
                Texto = t.Texto, 
                IdPregunta = (t.IdPregunta != 0 ? t.IdPregunta : null)
            }).ToList() : new List<Valor>();
            var element = Find(el.Id);
            if (element == null) return new OperationResult<PosibleRespuesta>(element);
            element.TipoPosibleRespuesta = el.TipoPosibleRespuesta;
            foreach (var valor in element.Valores.ToList())
            {
                manager.Valor.Delete(valor.Id);
            }
            manager.Valor.SaveChanges();
            var op = base.Modify(element, properties);
            SaveChanges();
            foreach (var valor in list)
            {
                valor.IdPosibleRespuesta = element.Id;
                manager.Valor.Add(valor);
                manager.Valor.SaveChanges();
            }
            return op;
        }
    }
}
