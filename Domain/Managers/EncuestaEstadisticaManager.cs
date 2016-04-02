using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositorios;
using Entity;
using Entity.Parciales;
using Entity.Reportes;
using PagedList;

namespace Domain.Managers
{
    public class EncuestaEstadisticaManager : GenericManager<EncuestaEstadistica>
    {
        public EncuestaEstadisticaManager(GenericRepository<EncuestaEstadistica> repository, Manager manager)
            : base(repository, manager)
        {
        }

        public EncuestaEstadisticaManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }

        public EncuestaEstadistica FindById(long idEncuesta)
        {
            return Manager.EncuestaEstadistica.Get(t => t.Id == idEncuesta).FirstOrDefault();
        }

        public override OperationResult<EncuestaEstadistica> Add(EncuestaEstadistica element)
        {
            var result = base.Add(element);
            var auditoria = new Auditoria()
            {
                id_encuesta = element.Id,
                accion = "Nuevo Registro",
                fecha = DateTime.Now,
                usuario = Usuario
            };
            Manager.AuditoriaManager.Add(auditoria);
            Manager.AuditoriaManager.SaveChanges();
            return result;
        }

        public override OperationResult<EncuestaEstadistica> Modify(EncuestaEstadistica element, params string[] properties)
        {
            var result = base.Modify(element, properties);
            var auditoria = new Auditoria()
            {
                id_encuesta = element.Id,
                accion = "Modificación",
                fecha = DateTime.Now,
                usuario = Usuario
            };
            Manager.AuditoriaManager.Add(auditoria);
            Manager.AuditoriaManager.SaveChanges();
            return result;
        }

        public void GenerateCurrent(long idEstablecimiento, DateTime? date = null)
        {
            var establecimiento = Manager.Establecimiento.Find(idEstablecimiento);
            if (establecimiento == null || establecimiento.Informante == null) return;

            if (establecimiento.Informante != null && establecimiento.CAT_ESTAB_ANALISTA.Count > 0)
            {
                if (establecimiento.creado != null)
                {
                    var dateNow = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, 1);
                    var dateFirstEncuesta = new DateTime(establecimiento.creado.GetValueOrDefault().Year, establecimiento.creado.GetValueOrDefault().Month, 1).AddMonths(-1);

                    while (dateFirstEncuesta <= dateNow)
                    {
                        var now = new DateTime(dateFirstEncuesta.Year, dateFirstEncuesta.Month, 1);

                        var item = Get(t => t.Fecha.Month == now.Month && t.Fecha.Year == now.Year && t.IdEstablecimiento == idEstablecimiento).FirstOrDefault();
                        if (item == null)
                        {
                            var encuesta = new EncuestaEstadistica()
                            {
                                IdEstablecimiento = idEstablecimiento,
                                EstadoEncuesta = EstadoEncuesta.NoEnviada,
                                Fecha = now,
                                IdInformante = establecimiento.Informante.Identificador,
                                //IdAnalista = establecimiento.IdAnalista,
                                VolumenProduccionMensual = new VolumenProduccion()
                                {

                                },
                                // ValorProduccionMensual = new ValorProduccion(),
                                VentasProductosEstablecimiento = new VentasProductosEstablecimientos()
                                {
                                    ServiciosActivados = establecimiento.TipoEnum == TipoEstablecimiento.Servicio ? true : false
                                },
                                TrabajadoresDiasTrabajados = new TrabajadoresDiasTrabajados(),
                                FactorProduccion = new FactorProducccion()
                            };


                            var encuestaEstadisticaLast = this.Get().OrderBy(x => x.Id).LastOrDefault();
                            var encuestaEmpresarialLast = Manager.EncuestaEmpresarial.Get().OrderBy(x => x.Id).LastOrDefault();

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

                            Add(encuesta);
                            SaveChanges();
                            var first = establecimiento.CAT_ESTAB_ANALISTA.Select(t => t.orden).OrderBy(t => t).FirstOrDefault();
                            foreach (var analista in establecimiento.CAT_ESTAB_ANALISTA)
                            {
                                var ne = new EncuestaAnalista()
                                {
                                    orden = analista.orden,
                                    id_ciiu = analista.id_ciiu,
                                    id_analista = analista.id_analista,
                                    id_encuesta = encuesta.Id,
                                    EstadoEncuesta = EstadoEncuesta.NoEnviada,
                                    current = (analista.orden == first) ? 1 : 0
                                };
                                Manager.EncuestaAnalistaManager.Add(ne);
                                Manager.EncuestaAnalistaManager.SaveChanges();
                            }
                            var volumenP = new VolumenProduccion()
                            {
                                Identificador = encuesta.Id,
                            };
                            Manager.VolumenProduccionManager.Add(volumenP);
                            Manager.VolumenProduccionManager.SaveChanges();

                            var old = this.Get(t => t.IdEstablecimiento == idEstablecimiento).Where(t => t.Id != encuesta.Id && t.Fecha < encuesta.Fecha).OrderBy(t => t.Fecha).ToList();

                            foreach (var lp in establecimiento.LineasProductoEstablecimiento)
                            {
                                var um = lp.LineaProducto.LineasProductoUnidadMedida.FirstOrDefault();
                                if (um != null)
                                {
                                    var mp = new MateriaPropia()
                                    {
                                        IdLineaProducto = lp.LineaProducto.Id,
                                        IdUnidadMedida = um.id_unidad_medida.GetValueOrDefault(),
                                        IdVolumenProduccion = volumenP.Identificador,
                                        JustificacionValorUnitario = null,
                                        JustificacionProduccion = null,
                                        justificacion_venta_pais = null,
                                        justificacion_venta_extranjero = null
                                    };

                                    if (old.Count > 0)
                                    {
                                        var lastEnc = old.LastOrDefault();
                                        if (lastEnc != null)
                                        {
                                            var materiaPropia =
                                            lastEnc.VolumenProduccionMensual.MateriasPropia.FirstOrDefault(
                                                t => t.IdLineaProducto == mp.IdLineaProducto);
                                            if (materiaPropia != null)
                                            {
                                                mp.Existencia = (materiaPropia.Existencia.GetValueOrDefault() + materiaPropia.Produccion.GetValueOrDefault() + materiaPropia.OtrosIngresos.GetValueOrDefault()) -
                                                            (materiaPropia.VentasPais.GetValueOrDefault() + materiaPropia.VentasExtranjero.GetValueOrDefault() + materiaPropia.OtrasSalidas.GetValueOrDefault());
                                            }
                                            else
                                            {
                                                mp.Existencia = null;
                                            }
                                        }
                                    }

                                    Manager.MateriaPropiaManager.Add(mp);
                                    Manager.MateriaPropiaManager.SaveChanges();
                                }

                            }
                            var ciius = new[]
                            {
                            new{ciiu="2592",detalle="TRATAMIENTO Y REVESTIMIENTO DE METALES MAQUINADO"},
                            new{ciiu="3312",detalle="REPARACION Y MANTENIMIENTO DE MAQUINARIAS"},
                            new{ciiu="3314",detalle="REPARACION DE EQUIPOS ELECTRICOS"},
                            new{ciiu="3315",detalle="REPARACION DE EQUIPOS DE TRANSPORTE, EXCEPTO VEHICULOS AUTOMOTORES"},
                        };
                            foreach (var ciiu in ciius)
                            {
                                var serv = new VentasServicioManufactura()
                                {
                                    IdVentaProductoestablecimiento = encuesta.VentasProductosEstablecimiento.Identificador,
                                    ciiu = ciiu.ciiu,
                                    detalle = ciiu.detalle
                                };
                                Manager.VentaServicioManufacturaManager.Add(serv);
                                Manager.VentaServicioManufacturaManager.SaveChanges();
                            }

                        }
                        dateFirstEncuesta = dateFirstEncuesta.AddMonths(1);
                    }
                }
            }
        }

        public void UpdateExistenciasEncuestas(long idEncuesta, long idLineaProd)
        {
            var encuesta = Manager.EncuestaEstadistica.FindById(idEncuesta);
            var lineaProd = Manager.LineaProducto.Find(idLineaProd);
            if (encuesta == null || lineaProd == null) return;
            var rest = Manager.EncuestaEstadistica.Get(t => t.Fecha > encuesta.Fecha && t.IdEstablecimiento == encuesta.IdEstablecimiento).ToList();
            foreach (var encuestaEstadistica in rest.OrderBy(t => t.Fecha))
            {
                UpdateExistenciasEncuesta(encuestaEstadistica, lineaProd);
            }
        }

        private void UpdateExistenciasEncuesta(EncuestaEstadistica encuesta, LineaProducto lineaProd)
        {
            if (encuesta == null || lineaProd == null) return;

            var old = Get(t => t.Id != encuesta.Id && t.Fecha < encuesta.Fecha && t.IdEstablecimiento == encuesta.IdEstablecimiento).OrderBy(t => t.Fecha).ToList();
            
            if (old.Count > 0)
            {
                var mp =
                    encuesta.VolumenProduccionMensual.MateriasPropia.FirstOrDefault(t => t.IdLineaProducto == lineaProd.Id);
                if (mp == null) return;
                var lastEnc = old.LastOrDefault();
                if (lastEnc != null)
                {
                   var materiaPropia =
                   lastEnc.VolumenProduccionMensual.MateriasPropia.FirstOrDefault(
                       t => t.IdLineaProducto == lineaProd.Id);
                    if (materiaPropia != null)
                    {
                        mp.Existencia = (materiaPropia.Existencia.GetValueOrDefault() + materiaPropia.Produccion.GetValueOrDefault() + materiaPropia.OtrosIngresos.GetValueOrDefault()) -
                                    (materiaPropia.VentasPais.GetValueOrDefault() + materiaPropia.VentasExtranjero.GetValueOrDefault() + materiaPropia.OtrasSalidas.GetValueOrDefault());

                        Manager.MateriaPropiaManager.Modify(mp);
                        Manager.MateriaPropiaManager.SaveChanges();
                    }
                }
            }
        }

        public override void UpdateKey(EncuestaEstadistica element)
        {
            long id = 1;
            var last = Manager.Encuesta.Get().OrderBy(t => t.Id).LastOrDefault();
            if (last != null)
                id = last.Id + 1;
            element.Id = id;
        }

        public List<string> Enviar(EncuestaEstadistica encuesta, bool enviar)
        {
            var manager = Manager;
            var list = new List<string>();
            var current = manager.EncuestaEstadistica.FindById(encuesta.Id);
            if (current == null)
            {
                return null;
            }
            
            if (current.Establecimiento.ContactoPredeterminado != null)
            {
                var defecto = manager.Contacto.Find(current.Establecimiento.ContactoPredeterminado.Id);
                if (defecto != null)
                {
                    defecto.Nombre = encuesta.Establecimiento.ContactoTemporal.Nombre;
                    defecto.Correo = encuesta.Establecimiento.ContactoTemporal.Correo;
                    defecto.Telefono = encuesta.Establecimiento.ContactoTemporal.Telefono;
                    defecto.IdCargo = encuesta.Establecimiento.ContactoTemporal.IdCargo;
                    defecto.Anexo = encuesta.Establecimiento.ContactoTemporal.Anexo;
                    defecto.Celular = encuesta.Establecimiento.ContactoTemporal.Celular;
                }
            }

            //VALOR PRODUCCION MENSUAL ESTABLECIMIENTO
            //current.ValorProduccionMensual.ProductosMateriaPropia = encuesta.ValorProduccionMensual.ProductosMateriaPropia;
            //current.ValorProduccionMensual.ProductosMateriaTerceros = encuesta.ValorProduccionMensual.ProductosMateriaTerceros;

            //VALOR VENTAS MENSUALES PRODUCTOS ELABORADOS ESTABLECIMIENTO
            //current.VentasProductosEstablecimiento.VentasPaisExtranjero =
            //    encuesta.VentasProductosEstablecimiento.VentasPaisExtranjero;
            //current.VentasProductosEstablecimiento.VentasPaisExtranjero.VentaPais =
            //   encuesta.VentasProductosEstablecimiento.VentasPaisExtranjero.VentaPais;

            //TRABAJADORES DIAS TRABAJADOS
            current.TrabajadoresDiasTrabajados.AdministrativosPlanta = encuesta.TrabajadoresDiasTrabajados.AdministrativosPlanta;
            current.TrabajadoresDiasTrabajados.DiasTrabajados = encuesta.TrabajadoresDiasTrabajados.DiasTrabajados;
            current.TrabajadoresDiasTrabajados.TrabajadoresProduccion = encuesta.TrabajadoresDiasTrabajados.TrabajadoresProduccion;
            manager.EncuestaEstadistica.SaveChanges();

            list = Validar(current);

            if (list.Count == 0)
            {
                current.EstadoEncuesta = EstadoEncuesta.Enviada;
                current.fecha_ultimo_envio = DateTime.Now;
                manager.EncuestaEstadistica.Modify(current);
                manager.EncuestaEstadistica.SaveChanges();
            }

            return list;
        }

        public List<string> UpdatePrevious(EncuestaEstadistica encuesta)
        {
            var manager = Manager;
            var list = new List<string>();
            var current = manager.EncuestaEstadistica.FindById(encuesta.Id);
            if (current == null)
            {
                return list;
            }

            current.TrabajadoresDiasTrabajados.AdministrativosPlanta =
               encuesta.TrabajadoresDiasTrabajados.AdministrativosPlanta;
            current.TrabajadoresDiasTrabajados.DiasTrabajados =
               encuesta.TrabajadoresDiasTrabajados.DiasTrabajados;
            current.TrabajadoresDiasTrabajados.TrabajadoresProduccion =
                encuesta.TrabajadoresDiasTrabajados.TrabajadoresProduccion;
            manager.EncuestaEstadistica.SaveChanges();

            list = Validar(current);

            if (list.Count == 0)
            {
                foreach (var mat in current.VolumenProduccionMensual.MateriasPropia)
                {
                    UpdateExistenciasEncuestas(encuesta.Id, mat.IdLineaProducto);
                }
                current.actualizacion = 0;
                manager.EncuestaEstadistica.Modify(current);
                manager.EncuestaEstadistica.SaveChanges();
            }
            return list;
        }

        public List<string> SalvarContacto(EncuestaEstadistica encuesta)
        {
            var manager = Manager;
            var list = new List<string>();
            var current = manager.EncuestaEstadistica.FindById(encuesta.Id);
            if (current == null)
            {
                list.Add("Problemas con los datos");
                return list;
            }
            
            var estab = manager.Establecimiento.Find(current.IdEstablecimiento);
            if (estab == null)
            {
                list.Add("Problemas con los datos");
                return list;
            }
            if (encuesta.Establecimiento.IdDepartamento == null)
            {
                list.Add("Debe seleccionar un Departamento");
                return list;
            }
            if (encuesta.Establecimiento.IdProvincia == null)
            {
                list.Add("Debe seleccionar una Provincia");
                return list;
            }
            if (encuesta.Establecimiento.IdDistrito == null)
            {
                list.Add("Debe seleccionar un Distrito");
                return list;
            }
            estab.Direccion = encuesta.Establecimiento.Direccion;
            estab.Telefono = encuesta.Establecimiento.Telefono;
            estab.IdDepartamento = encuesta.Establecimiento.IdDepartamento;
            estab.IdProvincia = encuesta.Establecimiento.IdProvincia;
            estab.IdDistrito = encuesta.Establecimiento.IdDistrito;
            manager.Establecimiento.Modify(estab);
            manager.Establecimiento.SaveChanges();
            if (current.Establecimiento.ContactoPredeterminado != null)
            {
                var defecto = manager.Contacto.Find(current.Establecimiento.ContactoPredeterminado.Id);
                if (defecto != null)
                {
                    defecto.Nombre = encuesta.Establecimiento.ContactoTemporal.Nombre;
                    defecto.Correo = encuesta.Establecimiento.ContactoTemporal.Correo;
                    defecto.Telefono = encuesta.Establecimiento.ContactoTemporal.Telefono;
                    defecto.IdCargo = encuesta.Establecimiento.ContactoTemporal.IdCargo;
                    defecto.Anexo = encuesta.Establecimiento.ContactoTemporal.Anexo;
                    defecto.Celular = encuesta.Establecimiento.ContactoTemporal.Celular;
                    manager.Contacto.Modify(defecto);
                    manager.Contacto.SaveChanges();
                }

            }
            return list;
        }

        public void AddLineaProducto(long idEncuesta, LineaProducto linea, bool addMateriaPropia = true)
        {
            var encuesta = Manager.EncuestaEstadistica.FindById(idEncuesta);
            var lineaprod = Manager.LineaProducto.Find(linea.Id);
            var ciiu = Manager.Ciiu.Find(linea.IdCiiu);
            if (ciiu != null && encuesta != null && lineaprod != null)
            {
                long idUM = 0;
                var first = lineaprod.LineasProductoUnidadMedida.FirstOrDefault();
                idUM = first != null ? first.id_unidad_medida.GetValueOrDefault() : 0;
                var establecimiento = encuesta.Establecimiento;
                if (establecimiento.Ciius.Select(x => x.Ciiu).Any(t => t.Id == ciiu.Id))
                {
                    Manager.LineaProductoEstablecimiento.Add(new LineaProductoEstablecimiento()
                    {
                        IdLineaProducto = linea.Id,
                        IdEstablecimiento = establecimiento.Id,
                        fecha_creacion_informante = encuesta.Fecha
                    });
                    Manager.LineaProductoEstablecimiento.SaveChanges();
                    if (addMateriaPropia)
                    {
                        var tr = Manager.MateriaPropiaManager.Add(new MateriaPropia()
                        {
                            IdLineaProducto = linea.Id,
                            IdUnidadMedida = idUM,
                            IdVolumenProduccion = encuesta.VolumenProduccionMensual.Identificador,

                        });
                        Manager.MateriaPropiaManager.SaveChanges();
                    }
                }
                else
                {
                    Manager.CiiuEstablecimientoManager.Add(
                       new CiiuEstablecimiento
                       {
                           IdCiiu = ciiu.Id,
                           IdEstablecimiento = establecimiento.Id
                       });

                    Manager.Establecimiento.Modify(establecimiento);
                    Manager.Establecimiento.SaveChanges();

                    Manager.LineaProductoEstablecimiento.Add(new LineaProductoEstablecimiento()
                    {
                        IdLineaProducto = linea.Id,
                        IdEstablecimiento = establecimiento.Id,
                        fecha_creacion_informante = encuesta.Fecha
                    });
                    Manager.LineaProductoEstablecimiento.SaveChanges();
                    if (addMateriaPropia)
                    {
                        var re = Manager.MateriaPropiaManager.Add(new MateriaPropia()
                        {
                            IdLineaProducto = linea.Id,
                            IdUnidadMedida = idUM,
                            IdVolumenProduccion = encuesta.VolumenProduccionMensual.Identificador,

                        });
                        Manager.MateriaPropiaManager.SaveChanges();
                    }
                }
            }
        }

        public List<string> Validar(EncuestaEstadistica element)
        {
            var list = base.Validate(element);

            //TRABAJADORES DIAS TRABAJADOS
            if (element.TrabajadoresDiasTrabajados.AdministrativosPlanta == null)
                list.Add("Debe introducir un valor para Administrativos en Planta");
            if (element.TrabajadoresDiasTrabajados.DiasTrabajados == null)
                list.Add("Debe introducir un valor para Días trabajados");
            if (element.TrabajadoresDiasTrabajados.TrabajadoresProduccion == null)
                list.Add("Debe introducir un valor para Trabajadores en Producción");

            foreach (var mt in element.VolumenProduccionMensual.MateriasPropia)
            {
                if (!mt.IsValid)
                    list.Add(string.Format("Los valores insertados para la línea de producto {0} en el capítulo 2 sección A no son correctos. (Existencias + Produccion + Otros Ingresos) ≥ (VentasPais + Ventas Extranjeros + Otras Salidas)", mt.LineaProducto.Nombre));
            }


            if (element.VentasProductosEstablecimiento.ServiciosActivados)
            {
                var services = element.VentasProductosEstablecimiento.VentasServicioManufactura;
                var any = false;
                foreach (var se in services)
                {
                    if (se.venta != null)
                    {
                        any = true;
                        break;
                    }
                }
                if (!any)
                    list.Add("Usted señaló que el establecimiento brindó servicios a terceros, debe llenar al menos uno.");
            }
            if (!element.FactorProduccion.ProduccionNormalB && !element.FactorProduccion.IncrementoB &&
                !element.FactorProduccion.DecrecimientoB)
            {
                list.Add("Debe seleccionar una opción en Factores que afectaron la producción");
            }
            if (element.FactorProduccion.IncrementoB)
            {
                if (element.FactorProduccion.CAT_FACTOR1.Count(t => t.TipoFactor == TipoFactor.Incremento) == 0 && element.FactorProduccion.OtroFactor == null)
                {
                    list.Add("Usted señaló Incremento dentro de Factores que Afectaron la producción. Debe escoger al menos un Factor de tipo incremento");
                }
            }
            if (element.FactorProduccion.DecrecimientoB)
            {
                if (element.FactorProduccion.CAT_FACTOR1.Count(t => t.TipoFactor == TipoFactor.Disminución) == 0 && element.FactorProduccion.OtroFactor == null)
                {
                    list.Add("Usted señaló Disminución dentro de Factores que Afectaron la producción. Debe escoger al menos un Factor de tipo disminución");
                }
            }

            element = Manager.EncuestaEstadistica.FindById(element.Id);
            var sum =
                element.VolumenProduccionMensual.MateriasPropia.Sum(
                    t =>
                        t.ValorUnitario.GetValueOrDefault() + t.Produccion.GetValueOrDefault() + t.OtrosIngresos.GetValueOrDefault() + t.OtrasSalidas.GetValueOrDefault() + t.VentasExtranjero.GetValueOrDefault() +
                        t.VentasPais.GetValueOrDefault());
            if (sum == 0)
            {
                list.Add("Debe ingresar los valores de al menos una línea de producto en \"Productos fabricados con materia prima propia para venta y/o autoconsumo del establecimiento\"");
            }
            foreach (var ter in element.DAT_VALOR_PROD_MENSUAL)
            {
                if (ter.MateriaPrimaTercerosActivada && ter.ProductosMateriaTerceros == null)
                {
                    list.Add("Debe ingresar todos los valores de materia prima de terceros de la producción mensual del establecimiento");
                    break;
                }
            }

            foreach (var venta in element.VentasProductosEstablecimiento.DAT_VENTAS_PAIS_EXTRANJERO)
            {
                if (venta.VentaExtranjeroActivado && venta.VentaExtranjero == null)
                {
                    list.Add("Debe ingresar los valores de Ventas en el extranjero en \"Valor total de ventas mensuales de productos elaborados en el establecimiento\"");
                    break;
                }
                if (venta.VentaPaisActivado && venta.VentaPais == null)
                {
                    list.Add("Debe ingresar los valores de Ventas en el pais en \"Valor total de ventas mensuales de productos elaborados en el establecimiento\"");
                    break;
                }
            }
            return list;
        }

        public IPagedList GetAsignadosInformante(Query<EncuestaEstadistica> query)
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
                var encuestas = temp as EncuestaEstadistica[] ?? temp.ToArray();
                var list = encuestas.ToPagedList(1, encuestas.Any() ? encuestas.Count() : 1);
                query.Elements = list;
                return list;
            }

        }

        public IPagedList GetForUpdate(Query<EncuestaEstadistica> query)
        {
            var temp = Repository.Get(t => (t.EstadoEncuesta == EstadoEncuesta.Validada || t.actualizacion == 1)
                                            && t.IdEstablecimiento == query.Criteria.IdEstablecimiento
                                            && t.Fecha.Year == query.Criteria.Year
                                            && (query.Criteria.Mes == null || query.Criteria.Mes == t.Fecha.Month), null, query.Order);

            if (query.Paginacion != null)
            {
                var list = temp.ToPagedList(query.Paginacion.Page, query.Paginacion.ItemsPerPage);
                query.Elements = list;
                return list;
            }
            else
            {
                var encuestas = temp.ToArray();
                var list = encuestas.ToPagedList(1, encuestas.Any() ? encuestas.Count() : 1);
                query.Elements = list;
                return list;
            }
        }

        public IPagedList GetAsignadosAnalista(Query<EncuestaEstadistica> query, long idAnalista)
        {
            var estab = new long?();
            // var ana = new long?();
            if (query.Criteria != null)
            {
                if (query.Criteria.IdEstablecimiento != 0)
                    estab = query.Criteria.IdEstablecimiento;
                //if (query.Criteria.IdAnalista != 0)
                //    if (query.Criteria.IdAnalista != null) ana = (long)query.Criteria.IdAnalista;
            }
            var temp = Repository.Get(query.Filter, null, query.Order)
                .Where(t => t.IdEstablecimiento == estab
                && t.DAT_ENCUESTA_ANALISTA.Any(h => h.id_analista == idAnalista && (h.IsCurrent || h.IsPast))
                && t.EstadoEncuesta != EstadoEncuesta.NoEnviada);
            if (query.Paginacion != null)
            {
                var list = temp.ToPagedList(query.Paginacion.Page, query.Paginacion.ItemsPerPage);
                query.Elements = list;
                return list;
            }
            else
            {
                var encuestas = temp as EncuestaEstadistica[] ?? temp.ToArray();
                var list = encuestas.ToPagedList(1, encuestas.Any() ? encuestas.Count() : 1);
                query.Elements = list;
                return list;
            }
        }

        public void Observar(long id, string observacion)
        {
            var encuesta = Manager.EncuestaEstadistica.FindById(id);
            if (encuesta == null) return;
            encuesta.EstadoEncuesta = EstadoEncuesta.Observada;
            encuesta.Justificacion = observacion;
            Manager.EncuestaEstadistica.Modify(encuesta);
            Manager.EncuestaEstadistica.SaveChanges();
            var auditoria = new Auditoria()
            {
                id_encuesta = encuesta.Id,
                accion = "Observada",
                fecha = DateTime.Now,
                usuario = Usuario
            };
            Manager.AuditoriaManager.Add(auditoria);
            Manager.AuditoriaManager.SaveChanges();
        }
   
        public void Validar(long id)
        {
            var manager = Manager;
            var encuesta = Manager.EncuestaEstadistica.FindById(id);
            if (encuesta == null) return;

            decimal idAnalista = 0;
            bool esMismoAnalista = true;

            foreach (var analista in encuesta.DAT_ENCUESTA_ANALISTA)
            {
                if (idAnalista == 0)
                {
                    idAnalista = analista.id_analista;
                }
                else
                {
                    if (idAnalista != analista.id_analista)
                    {
                        esMismoAnalista = false;
                        break;
                    }
                }
            }

            if (esMismoAnalista == false)
            {
                var analistas = encuesta.DAT_ENCUESTA_ANALISTA.OrderBy(t => t.current);
                var tt = analistas.FirstOrDefault(t => t.IsCurrent);
                if (tt == null) return;

                foreach (var analista in analistas)
                {
                    if (tt.id_analista == analista.id_analista)
                    {
                        analista.current = 2;
                        analista.EstadoEncuesta = EstadoEncuesta.Validada;
                        Manager.EncuestaAnalistaManager.Modify(analista);
                    }                    
                }

                var next = analistas.Where(t => t.orden > tt.orden && t.current == 0).OrderBy(t => t.orden).FirstOrDefault();
                if (next != null)
                {
                    next.current = 1;
                    Manager.EncuestaAnalistaManager.Modify(next);
                    Manager.EncuestaAnalistaManager.SaveChanges();
                    var auditoria = new Auditoria()
                    {
                        id_encuesta = encuesta.Id,
                        accion = "Validada",
                        fecha = DateTime.Now,
                        usuario = Usuario
                    };
                    Manager.AuditoriaManager.Add(auditoria);
                    Manager.AuditoriaManager.SaveChanges();
                }
                else
                {
                    encuesta.EstadoEncuesta = EstadoEncuesta.Validada;
                    encuesta.fecha_validacion = DateTime.Now;
                    Manager.EncuestaEstadistica.Modify(encuesta);
                    Manager.EncuestaEstadistica.SaveChanges();
                    var auditoria = new Auditoria()
                    {
                        id_encuesta = encuesta.Id,
                        accion = "Validada",
                        fecha = DateTime.Now,
                        usuario = Usuario
                    };
                    Manager.AuditoriaManager.Add(auditoria);
                    Manager.AuditoriaManager.SaveChanges();

                    //brbLoadIpp
                    //foreach (var producto in encuesta.VolumenProduccionMensual.MateriasPropia)
                    //{
                    //    int factor = Convert.ToInt32(producto.LineaProducto.LineasProductoUnidadMedida.Where(t => t.id_unidad_medida == producto.IdUnidadMedida).FirstOrDefault().factor_conversion);
                    //    int codigoProducto = Convert.ToInt32(producto.LineaProducto.Codigo.Substring(6, 3));
                    //    string ciiu = producto.LineaProducto.Codigo.Substring(0, 4);
                    //    string codLineaProducto = producto.LineaProducto.Codigo.Substring(4, 2);
                    //    Manager.IpmIppManager.SendIpp(producto.ValorUnitario.Value, factor, encuesta.Fecha.Year, encuesta.Fecha.Month.ToString(), Convert.ToInt32(encuesta.Establecimiento.IdentificadorInterno), codigoProducto, ciiu, codLineaProducto);
                    //}
                    //endbrbLoadIpp

                    foreach (var mat in encuesta.VolumenProduccionMensual.MateriasPropia)
                    {
                        UpdateExistenciasEncuestas(encuesta.Id, mat.IdLineaProducto);
                    }

                    if (encuesta.actualizacion == 1)
                    {   
                        encuesta.actualizacion = 0;
                        manager.EncuestaEstadistica.Modify(encuesta);
                        manager.EncuestaEstadistica.SaveChanges();
                    }
                }
            }
            else
            {
                foreach (var analista in encuesta.DAT_ENCUESTA_ANALISTA)
                {
                    analista.current = 2;
                    analista.EstadoEncuesta = EstadoEncuesta.Validada;
                    Manager.EncuestaAnalistaManager.Modify(analista);
                }

                Manager.EncuestaAnalistaManager.SaveChanges();

                encuesta.EstadoEncuesta = EstadoEncuesta.Validada;
                encuesta.fecha_validacion = DateTime.Now;
                Manager.EncuestaEstadistica.Modify(encuesta);
                Manager.EncuestaEstadistica.SaveChanges();
                var auditoria = new Auditoria()
                {
                    id_encuesta = encuesta.Id,
                    accion = "Validada",
                    fecha = DateTime.Now,
                    usuario = Usuario
                };
                
                Manager.AuditoriaManager.Add(auditoria);
                Manager.AuditoriaManager.SaveChanges();

                //brbLoadIpp
                //foreach (var producto in encuesta.VolumenProduccionMensual.MateriasPropia)
                //{
                //    int factor = Convert.ToInt32(producto.LineaProducto.LineasProductoUnidadMedida.Where(t => t.id_unidad_medida == producto.IdUnidadMedida).FirstOrDefault().factor_conversion);
                //    int codigoProducto = Convert.ToInt32(producto.LineaProducto.Codigo.Substring(6, 3));
                //    string ciiu = producto.LineaProducto.Codigo.Substring(0, 4);
                //    string codLineaProducto = producto.LineaProducto.Codigo.Substring(4, 2);
                //    Manager.IpmIppManager.SendIpp(producto.ValorUnitario.Value, factor, encuesta.Fecha.Year, encuesta.Fecha.Month.ToString(), Convert.ToInt32(encuesta.Establecimiento.IdentificadorInterno), codigoProducto, ciiu, codLineaProducto);
                //}
                //endbrbLoadIpp

                foreach (var mat in encuesta.VolumenProduccionMensual.MateriasPropia)
                {
                    UpdateExistenciasEncuestas(encuesta.Id, mat.IdLineaProducto);
                }

                if (encuesta.actualizacion == 1)
                {                    
                    encuesta.actualizacion = 0;
                    manager.EncuestaEstadistica.Modify(encuesta);
                    manager.EncuestaEstadistica.SaveChanges();
                }
            }
        }

        #region REPORTES

        public List<MateriaPropia> GetVolumenProduccionMateriaPropia(string type, int year, IEnumerable<int> month,
           IEnumerable<long> ids, IEnumerable<long> idEstablecimientos = null)
        {
            switch (type.ToLower())
            {
                case "establecimiento":
                    var materias = Manager.EncuestaEstadistica.Get(t => ids.Contains(t.IdEstablecimiento)
                                                                         && t.Fecha.Year == year &&
                                                                         month.Contains(t.Fecha.Month))
                        .SelectMany(t => t.VolumenProduccionMensual.MateriasPropia);
                    return materias.ToList();
                case "ciiu":
                    var materias2 = Manager.EncuestaEstadistica.Get(
                        t => idEstablecimientos.Contains(t.IdEstablecimiento)
                             && t.Fecha.Year == year &&
                             month.Contains(t.Fecha.Month))
                        .SelectMany(t => t.VolumenProduccionMensual.MateriasPropia).Where(t => ids.Contains(t.LineaProducto.IdCiiu));
                    return materias2.ToList();
                case "lineaproducto":
                    var materias3 = Manager.EncuestaEstadistica.Get(
                        t => idEstablecimientos.Contains(t.IdEstablecimiento)
                             && t.Fecha.Year == year &&
                             month.Contains(t.Fecha.Month))
                        .SelectMany(t => t.VolumenProduccionMensual.MateriasPropia).Where(h => ids.Contains(h.IdLineaProducto));
                    return materias3.ToList();

            }
            return new List<MateriaPropia>();
        }

        public List<MateriaTerceros> GetVolumenProduccionMateriaTerceros(string type, int year, IEnumerable<int> month,
            IEnumerable<long> ids, IEnumerable<long> idEstablecimientos = null)
        {
            switch (type.ToLower())
            {
                case "establecimiento":
                    var materias = Manager.EncuestaEstadistica.Get(t => ids.Contains(t.IdEstablecimiento)
                                                                         && t.Fecha.Year == year &&
                                                                         month.Contains(t.Fecha.Month))
                        .SelectMany(t => t.VolumenProduccionMensual.MateriasTercero);
                    return materias.ToList();
                case "ciiu":
                    var materias2 = Manager.EncuestaEstadistica.Get(
                        t => idEstablecimientos.Contains(t.IdEstablecimiento)
                             && t.Fecha.Year == year &&
                             month.Contains(t.Fecha.Month))
                        .SelectMany(t => t.VolumenProduccionMensual.MateriasTercero).Where(t => ids.Contains(t.LineaProducto.IdCiiu));
                    return materias2.ToList();
                case "lineaproducto":
                    var materias3 = Manager.EncuestaEstadistica.Get(
                        t => idEstablecimientos.Contains(t.IdEstablecimiento)
                             && t.Fecha.Year == year &&
                             month.Contains(t.Fecha.Month))
                        .SelectMany(t => t.VolumenProduccionMensual.MateriasTercero).Where(h => ids.Contains(h.IdLineaProducto));
                    return materias3.ToList();

            }
            return new List<MateriaTerceros>();
        }

        public List<ValorProduccion> GetValorProduccion(string type, int year, IEnumerable<int> month,
            IEnumerable<long> ids, IEnumerable<long> idEstablecimientos = null)
        {
            switch (type.ToLower())
            {
                case "establecimiento":
                    var materias = Manager.EncuestaEstadistica.Get(t => ids.Contains(t.IdEstablecimiento)
                                                                         && t.Fecha.Year == year &&
                                                                         month.Contains(t.Fecha.Month))
                        .SelectMany(t => t.DAT_VALOR_PROD_MENSUAL);
                    return materias.ToList();
                case "ciiu":
                    var materias2 = Manager.EncuestaEstadistica.Get(
                        t => idEstablecimientos.Contains(t.IdEstablecimiento)
                             && t.Fecha.Year == year &&
                             month.Contains(t.Fecha.Month))
                        .SelectMany(t => t.DAT_VALOR_PROD_MENSUAL).Where(t => ids.Contains(t.id_ciiu));
                    return materias2.ToList();

            }
            return new List<ValorProduccion>();
        }

        public List<VentasPaisExtranjero> GetValorVentas(string type, int year, IEnumerable<int> month,
          IEnumerable<long> ids, IEnumerable<long> idEstablecimientos = null)
        {
            switch (type.ToLower())
            {
                case "establecimiento":
                    var materias = Manager.EncuestaEstadistica.Get(t => ids.Contains(t.IdEstablecimiento)
                                                                         && t.Fecha.Year == year &&
                                                                         month.Contains(t.Fecha.Month))
                        .SelectMany(t => t.VentasProductosEstablecimiento.DAT_VENTAS_PAIS_EXTRANJERO);
                    return materias.ToList();
                case "ciiu":
                    var materias2 = Manager.EncuestaEstadistica.Get(
                        t => idEstablecimientos.Contains(t.IdEstablecimiento)
                             && t.Fecha.Year == year &&
                             month.Contains(t.Fecha.Month))
                        .SelectMany(t => t.VentasProductosEstablecimiento.DAT_VENTAS_PAIS_EXTRANJERO).Where(t => ids.Contains(t.id_ciiu));
                    return materias2.ToList();

            }
            return new List<VentasPaisExtranjero>();
        }

        public List<TrabajadoresDiasTrabajados> GetTrabajadoresDiasTrabajadoses(int year, IEnumerable<int> month,
           IEnumerable<long> ids)
        {
            var result = Manager.EncuestaEstadistica.Get(t => ids.Contains(t.IdEstablecimiento)
                && t.Fecha.Year == year && month.Contains(t.Fecha.Month)).Select(t => t.TrabajadoresDiasTrabajados);
            return result.ToList();
        }

        public PorcentajeEncuestaEstadistica PorcentajeEncuestaEstadistica(PorcentajeEncuestaEstadisticaFilter filter)
        {
            if (filter.IsAnnual)
            {
                var res = PorcentajeEncuestaEstadisticaAnual(filter.Year, filter.From, filter.To, filter.Estado, filter.IdAnalista);
                res.Filter = filter;
                return res;
            }
            var res1 = PorcentajeEncuestaEstadisticaMensual(filter.Year, filter.Month, filter.From, filter.To, filter.Estado, filter.IdAnalista);
            res1.Filter = filter;
            return res1;
        }

        public PorcentajeEncuestaEstadistica PorcentajeEncuestaEstadisticaAnual(int year, int from, int to, EstadoEncuesta estado, long? idAnalista = null)
        {
            var all = Manager.ViewProcentajeEncuestaExtadisticaManager.Get(t =>
                t.fecha.Year == year
                && t.fecha.Month >= from
                && t.fecha.Month <= to).AsQueryable();
            if (idAnalista != null && idAnalista.GetValueOrDefault() > 0)
                all = all.Where(t => t.id_analista == idAnalista.GetValueOrDefault());
            if (estado != EstadoEncuesta.Todos)
                all = all.Where(t => t.estado_encuesta == (int)estado);
            var elements = all.GroupBy(t => t.id_analista);
            var result = new PorcentajeEncuestaEstadistica();
            for (var i = from; i <= to; i++)
                result.HeadersList.Add(i.GetMonthText().ToUpper().Substring(0, 3));
            foreach (var element in elements)
            {
                var item = new PorcentajeEncuestaEstadisticaItem()
                {
                    IdAnalista = (long)element.Key,
                    Analista = element.FirstOrDefault().login_analista,
                    Total = all.Count(),
                    PorcentajeEncuestaEstadistica = result
                };
                foreach (var mes in element.GroupBy(t => t.fecha.Month))
                {
                    item.Month.Add(new MonthData()
                    {
                        Number = mes.Key,
                        Name = mes.Key.GetMonthText().ToUpper().Substring(0, 3),
                        MonthlyValue = mes.Count(),
                        Total = all.Count()
                    });
                    foreach (var ciiu in mes.GroupBy(t => t.id_ciiu))
                    {
                        var fr = ciiu.FirstOrDefault();
                        var ciiuData = new CiiuData()
                        {
                            Id = ciiu.Key,
                            Name = string.Format("{0}:{1}", fr.codigo_ciiu, fr.nombre_ciiu),
                            PorcentajeEncuestaEstadisticaItem = item

                        };
                        foreach (var cm in ciiu.GroupBy(t => t.fecha.Month))
                        {
                            ciiuData.Month.Add(new MonthData()
                            {
                                Name = cm.Key.GetMonthText().ToUpper().Substring(0, 3),
                                MonthlyValue = cm.Count(),
                                Number = cm.Key,
                                Total = mes.Count()
                            });
                        }
                        item.Ciius.Add(ciiuData);
                    }
                }
                result.Elements.Add(item);
            }
            return result;
        }

        public PorcentajeEncuestaEstadistica PorcentajeEncuestaEstadisticaMensual(int year, int month, int from, int to, EstadoEncuesta estado, long? idAnalista = null)
        {
            var all = Manager.ViewProcentajeEncuestaExtadisticaManager.Get(t =>
                t.fecha.Month == month
                && t.fecha.Year == year
                && t.fecha.Day <= to
                && t.fecha.Day >= from).AsQueryable();
            if (idAnalista != null && idAnalista.GetValueOrDefault() > 0)
                all = all.Where(t => t.id_analista == idAnalista.GetValueOrDefault());
            if (estado != EstadoEncuesta.Todos)
                all = all.Where(t => t.estado_encuesta == (int)estado);
            var elements = all.GroupBy(t => t.id_analista);
            var result = new PorcentajeEncuestaEstadistica();
            for (var i = from; i <= to; i++)
                result.HeadersList.Add(i.ToString());
            foreach (var element in elements)
            {
                var item = new PorcentajeEncuestaEstadisticaItem()
                {
                    IdAnalista = (long)element.Key,
                    Analista = element.FirstOrDefault().login_analista,
                    Total = all.Count(),
                    PorcentajeEncuestaEstadistica = result
                };
                foreach (var mes in element.GroupBy(t => t.fecha.Day))
                {
                    item.Month.Add(new MonthData()
                    {
                        Number = mes.Key,
                        Name = mes.Key.ToString(),
                        MonthlyValue = mes.Count(),
                        Total = all.Count()
                    });
                    foreach (var ciiu in mes.GroupBy(t => t.id_ciiu))
                    {
                        var fr = ciiu.FirstOrDefault();
                        var ciiuData = new CiiuData()
                        {
                            Id = ciiu.Key,
                            Name = string.Format("{0}:{1}", fr.codigo_ciiu, fr.nombre_ciiu),
                            PorcentajeEncuestaEstadisticaItem = item
                        };
                        foreach (var cm in ciiu.GroupBy(t => t.fecha.Day))
                        {
                            ciiuData.Month.Add(new MonthData()
                            {
                                Name = cm.Key.ToString(),
                                MonthlyValue = cm.Count(),
                                Number = cm.Key,
                                Total = mes.Count()
                            });
                        }
                        item.Ciius.Add(ciiuData);
                    }
                }
                result.Elements.Add(item);
            }
            return result;
        }
        
        public IndiceValorFisicoProducido ValorFisicoProducido(int year, int? month = null, long? idCiiu = null)
        {
            var result = new IndiceValorFisicoProducido()
            {
                Year = year,
                Month = month.GetValueOrDefault(),
                IdCiiu = idCiiu.GetValueOrDefault(),
            };
            if (month == null)
                for (var i = 1; i <= 12; i++)
                    result.Header.Add(i.GetMonthText().ToUpper().Substring(0, 3));
            else
                result.Header.Add(month.GetValueOrDefault().GetMonthText().ToUpper().Substring(0, 3));

            var from = new DateTime(year, 1, 1);
            var to = new DateTime(year, 12, 31);
            if (month != null)
            {
                from = new DateTime(year, month.GetValueOrDefault(), 1);
                to = new DateTime(year, month.GetValueOrDefault(), DateTime.DaysInMonth(year, month.GetValueOrDefault()));
            }
            if (idCiiu == null)
            {
                var all = Manager.Ciiu.Get(t => t.Activado);
                foreach (var ciiu in all)
                {
                    result.Elements.AddRange(CalculateIVF(ciiu.Id, from, to));
                }
            }
            else
            {
                result.Elements.AddRange(CalculateIVF(idCiiu.GetValueOrDefault(), from, to));
            }
            return result;
        }

        public List<IndiceValorFisicoProducidoItem> CalculateIVF(long idCiiu, DateTime from, DateTime to)
        {
            var volP = Manager.MetodoCalculoManager.Get(t => t.nombre.Equals("Volumen Producción")).Select(t => t.Id).FirstOrDefault();
            var vd = Manager.MetodoCalculoManager.Get(t => t.nombre.Equals("VD-IIP")).Select(t => t.Id).FirstOrDefault();
            var consAp = Manager.MetodoCalculoManager.Get(t => t.nombre.Equals("Consumo Aparente")).Select(t => t.Id).FirstOrDefault();
            var ciiu = Manager.Ciiu.Find(idCiiu);
            if (ciiu.id_metodo_calculo == volP)
                return CalculateIVF1(idCiiu, from, to);
            if (ciiu.id_metodo_calculo == vd)
                return CalculateIVF2(idCiiu, from, to);
            if (ciiu.id_metodo_calculo == consAp)
                return CalculateIVF3(idCiiu, from, to);
            //return new List<IndiceValorFisicoProducidoItem>();
            return CalculateIVF1(idCiiu, from, to);
        }

        //consumo aparente 
        private List<IndiceValorFisicoProducidoItem> CalculateIVF3(long idCiiu, DateTime @from, DateTime to)
        {
            return CalculateIVF1(idCiiu, from, to);
        }

        //VD-IIP 
        private List<IndiceValorFisicoProducidoItem> CalculateIVF2(long idCiiu, DateTime @from, DateTime to)
        {
            var result = new List<IndiceValorFisicoProducidoItem>();
            var ciiu = Manager.Ciiu.Find(idCiiu);
            if (ciiu == null) return new List<IndiceValorFisicoProducidoItem>();
            //for (var i = from; i <= to; i = i.AddMonths(1))
            //{
            var encuestas = Manager.EncuestaEstadistica.Get(t => t.EstadoEncuesta == EstadoEncuesta.Validada
                                                            && t.Fecha <= to && t.Fecha >= from &&
                                                             t.VolumenProduccionMensual.MateriasPropia.Any(
                                                                 h => h.LineaProducto.IdCiiu == idCiiu));

            var establecimientos = encuestas.GroupBy(t => t.Establecimiento.Id);
            foreach (var establecimiento in establecimientos)
            {
                var est = Manager.Establecimiento.Find(establecimiento.Key);
                var total = Manager.EncuestaEstadistica.Get(t => t.EstadoEncuesta == EstadoEncuesta.Validada
                                                             && t.Fecha <= to && t.Fecha >= from
                                                             && t.IdEstablecimiento == establecimiento.Key).Count();
                var parte = establecimiento.Count();
                var it = new IndiceValorFisicoProducidoItem()
                {
                    IdCiiu = idCiiu,
                    Establecimiento = est.Nombre,
                    IdEstablecimiento = establecimiento.Key,
                    Ciiu = ciiu.ToString(),
                    CodigoCiiu = ciiu.Codigo,
                    Ponderacion = parte * 100.0 / total
                };

                for (var i = from; i <= to; i = i.AddMonths(1))
                {
                    foreach (var item2 in establecimiento.Where(t => t.Fecha.Month == i.Month && t.Fecha.Year == i.Year))
                    {
                        var ciius = item2.VolumenProduccionMensual.MateriasPropia.GroupBy(t => t.LineaProducto.IdCiiu);
                        foreach (var materiaPropia in ciius)
                        {

                            var num = 0d;
                            var den = 0d;
                            foreach (var propia in materiaPropia)
                            {
                                var aBase =
                                    Manager.AñoBaseManager.Get(
                                        t =>
                                            t.id_ciiu == materiaPropia.Key && t.id_establecimiento == establecimiento.Key &&
                                            t.id_linea_producto == propia.IdLineaProducto).FirstOrDefault();

                                if (aBase == null) continue;
                                num += (double)(aBase.precio * propia.Produccion.GetValueOrDefault() * propia.ValorUnitario.GetValueOrDefault());
                                den += (double)(aBase.precio * aBase.valor_produccion);
                            }
                            var ivf = 0d;
                            if (den > 0)
                                ivf = num / den;
                            it.Values.Add(new IndiceValorFisicoProducidoValue()
                            {
                                Value = ivf,
                                Header = i.Month.GetMonthText().ToUpper().Substring(0, 3),
                                Index = i.Month
                            });
                        }
                    }
                }
                result.Add(it);
            }
            //}
            return result;
        }

        //Volumen de Produccion
        public List<IndiceValorFisicoProducidoItem> CalculateIVF1(long idCiiu, DateTime from, DateTime to)
        {
            var result = new List<IndiceValorFisicoProducidoItem>();
            var ciiu = Manager.Ciiu.Find(idCiiu);
            if (ciiu == null) return new List<IndiceValorFisicoProducidoItem>();
            //for (var i = from; i <= to; i = i.AddMonths(1))
            //{
            
            var encuestas = Manager.EncuestaEstadistica.Get(t => t.EstadoEncuesta == EstadoEncuesta.Validada
                                                            && t.Fecha <= to && t.Fecha >= from &&
                                                             t.VolumenProduccionMensual.MateriasPropia.Any(
                                                                 h => h.LineaProducto.IdCiiu == idCiiu));

            var establecimientos = encuestas.GroupBy(t => t.Establecimiento.Id);
            foreach (var establecimiento in establecimientos)
            {
                var est = Manager.Establecimiento.Find(establecimiento.Key);
                var total = Manager.EncuestaEstadistica.Get(t => t.EstadoEncuesta == EstadoEncuesta.Validada
                                                             && t.Fecha <= to && t.Fecha >= from
                                                             && t.IdEstablecimiento == establecimiento.Key).Count();
                var parte = establecimiento.Count();
                var it = new IndiceValorFisicoProducidoItem()
                {
                    IdCiiu = idCiiu,
                    Establecimiento = est.Nombre,
                    IdEstablecimiento = establecimiento.Key,
                    Ciiu = ciiu.ToString(),
                    CodigoCiiu = ciiu.Codigo,
                    Ponderacion = parte * 100.0 / total
                };

                for (var i = from; i <= to; i = i.AddMonths(1))
                {
                    foreach (var item2 in establecimiento.Where(t => t.Fecha.Month == i.Month && t.Fecha.Year == i.Year))
                    {
                        var ciius = item2.VolumenProduccionMensual.MateriasPropia.GroupBy(t => t.LineaProducto.IdCiiu);
                        foreach (var materiaPropia in ciius)
                        {

                            var num = 0d;
                            var den = 0d;
                            foreach (var propia in materiaPropia)
                            {
                                var aBase =
                                    Manager.AñoBaseManager.Get(
                                        t =>
                                            t.id_ciiu == materiaPropia.Key && t.id_establecimiento == establecimiento.Key &&
                                            t.id_linea_producto == propia.IdLineaProducto).FirstOrDefault();

                                if (aBase == null) continue;
                                num += (double)(aBase.precio * propia.Produccion.GetValueOrDefault());
                                den += (double)(aBase.precio * aBase.valor_produccion);
                            }
                            var ivf = 0d;
                            if (den > 0)
                                ivf = num / den;
                            it.Values.Add(new IndiceValorFisicoProducidoValue()
                            {
                                Value = ivf,
                                Header = i.Month.GetMonthText().ToUpper().Substring(0, 3),
                                Index = i.Month
                            });
                        }
                    }
                }
                result.Add(it);
            }
            //}
            return result;
        }
        
        public IndiceVariacion IndiceVariacion(long idInformante, int year, bool isIndice)
        {
            if (isIndice)
            {
                return Indice(idInformante, year);
            }
            else
            {
                return Variacion(idInformante, year);
            }
        }

        private IndiceVariacion Variacion(long idInformante, int year)
        {
            var result = new IndiceVariacion()
            {
                IsVariacion = true,
                Year = year,
                IdInformante = idInformante
            };

            var encuestas = Manager.EncuestaEstadistica.Get(t => t.Fecha.Year == year && t.IdInformante == idInformante);
            var ciius = Manager.Ciiu.Get("DAT_ENCUESTA_ANALISTA", t => t.Activado).Where(t => t.DAT_ENCUESTA_ANALISTA.Any(h => encuestas.Any(e => e.Id == h.id_encuesta)));

            var idTotal = "Sector Farril Total".GetHashCode();
            var idManufacturaPrimaria = "Manufactura Primaria".GetHashCode();
            var idManufacturaNoPrimaria = "Manufactura no Primaria".GetHashCode();
            var idBienesConsumo = "Bienes de Consumo".GetHashCode();
            var idBienesIntermedio = "Bienes Intermedios".GetHashCode();
            var idBienesCapital = "Bienes de Capital".GetHashCode();
            var idServicio = "Servicio".GetHashCode();
            var idCiiuInformante = "CIIU del Informante".GetHashCode();
            result.Indice.AddRange(new List<IniceElement>()
            {
                new IniceElement() {Id = idTotal,Name = "Sector Farril Total",Level = 0},
                new IniceElement() {Id = idManufacturaPrimaria,Name = "Manufactura Primaria",Level = 1},
                new IniceElement() {Id = idManufacturaNoPrimaria,Name = "Manufactura no Primaria",Level = 1},
                new IniceElement() {Id = idBienesConsumo,Name = "Bienes de Consumo",Level = 2},
                new IniceElement() {Id = idBienesIntermedio,Name = "Bienes Intermedios",Level = 2},
                new IniceElement() {Id = idBienesCapital,Name = "Bienes de Capital",Level = 2},
                new IniceElement() {Id = idServicio,Name = "Servicio",Level = 2},
                new IniceElement() {Id = idCiiuInformante,Name = "CIIU del Informante",Level = 1},
            });

            for (var i = 1; i <= 12; i++)
            {
                var from = new DateTime(year, i, 1);
                var to = new DateTime(year, i, DateTime.DaysInMonth(year, i));
                var total = 0d;
                var primaria = 0d;
                var noPrimaria = 0d;
                var bienesConsumo = 0d;
                var bienesIntermedios = 0d;
                var bienesCapital = 0d;
                var servicio = 0d;
                var ciiuInformante = 0d;
                result.Header.Add(new HeaderValue() { Number = i, Text = i.GetMonthText().Substring(0, 3) + "." });
                foreach (var ciiu in ciius)
                {
                    var fromA = new DateTime(from.Year - 1, from.Month, from.Day);
                    var toA = new DateTime(to.Year - 1, to.Month, to.Day);
                    var res = CalculateIVF(ciiu.Id, from, to);
                    var resA = CalculateIVF(ciiu.Id, fromA, toA);
                    if (res.Count == 0 || resA.Count == 0) continue;
                    var ivfOb = res[0];
                    var ivfObA = resA[0];
                    if (ivfOb.Values.Count == 0 || ivfObA.Values.Count == 0) continue;
                    var te = ivfOb.Values[0].Value;
                    var teA = ivfObA.Values[0].Value;
                    var ivf = 0d;
                    if (teA > 0)
                        ivf = (te / teA) * 100.0 - 100.0;
                    total += ivf;
                    if (ciiu.EnumSubSector == EnumSubsector.Manufactura_No_Primaria)
                    {
                        primaria += ivf;
                        switch (ciiu.EnumRubro)
                        {
                            case EnumRubro.Bienes_DeCapital:
                                bienesCapital += ivf;
                                break;
                            case EnumRubro.Bienes_De_Consumo:
                                bienesConsumo += ivf;
                                break;
                            case EnumRubro.Bienes_Intermedios:
                                bienesIntermedios += ivf;
                                break;
                            case EnumRubro.Servicios:
                                servicio += ivf;
                                break;
                        }
                    }
                    else
                    {
                        noPrimaria += ivf;
                    }
                    var isCiiuInformante = true; //ciiu.Establecimientos.Any(t => t.IdInformante == idInformante);
                    if (isCiiuInformante)
                    {
                        ciiuInformante += ivf;
                        var elem = result.Indice.FirstOrDefault(t => t.Id == ciiu.Id);
                        if (elem == null)
                        {
                            result.Indice.Add(new IniceElement()
                            {
                                Id = ciiu.Id,
                                Name = ciiu.ToString(),
                                Level = 2,
                                IsCiiuInformante=true,
                                Values = new List<MonthValue>()
                                {
                                    new MonthValue()
                                    {
                                        Number = i,
                                        Text = i.GetMonthText(),
                                        Value = ivf
                                    }
                                }
                            });
                        }
                        else
                        {
                            elem.Values.Add(new MonthValue()
                            {
                                Number = i,
                                Text = i.GetMonthText(),
                                Value = ivf
                            });
                        }


                    }

                }
                var totalEl = result.Indice.FirstOrDefault(t => t.Id == idTotal);
                var primariaEl = result.Indice.FirstOrDefault(t => t.Id == idManufacturaPrimaria);
                var noPrimariaEl = result.Indice.FirstOrDefault(t => t.Id == idManufacturaNoPrimaria);
                var bienesConsumoEl = result.Indice.FirstOrDefault(t => t.Id == idBienesConsumo);
                var bienesCapitalEl = result.Indice.FirstOrDefault(t => t.Id == idBienesCapital);
                var bienesIntermedioEl = result.Indice.FirstOrDefault(t => t.Id == idBienesIntermedio);
                var servicioEl = result.Indice.FirstOrDefault(t => t.Id == idServicio);
                var ciiuInformanteEl = result.Indice.FirstOrDefault(t => t.Id == idCiiuInformante);
                totalEl.Values.Add(new MonthValue()
                {
                    Number = i,
                    Text = i.GetMonthText(),
                    Value = total
                });
                primariaEl.Values.Add(new MonthValue()
                {
                    Number = i,
                    Text = i.GetMonthText(),
                    Value = primaria
                });
                noPrimariaEl.Values.Add(new MonthValue()
                {
                    Number = i,
                    Text = i.GetMonthText(),
                    Value = noPrimaria
                });
                bienesConsumoEl.Values.Add(new MonthValue()
                {
                    Number = i,
                    Text = i.GetMonthText(),
                    Value = bienesConsumo
                });
                bienesCapitalEl.Values.Add(new MonthValue()
                {
                    Number = i,
                    Text = i.GetMonthText(),
                    Value = bienesCapital
                });
                bienesIntermedioEl.Values.Add(new MonthValue()
                {
                    Number = i,
                    Text = i.GetMonthText(),
                    Value = bienesIntermedios
                });
                servicioEl.Values.Add(new MonthValue()
                {
                    Number = i,
                    Text = i.GetMonthText(),
                    Value = servicio
                });
                ciiuInformanteEl.Values.Add(new MonthValue()
                {
                    Number = i,
                    Text = i.GetMonthText(),
                    Value = ciiuInformante
                });
            }

            return result;
        }
        
        public IndiceVariacion Indice(long idInformante, int year)
        {
            var result = new IndiceVariacion()
            {
                IsVariacion = false,
                Year = year,
                IdInformante = idInformante
            };

            var encuestas = Manager.EncuestaEstadistica.Get(t => t.Fecha.Year == year && t.IdInformante == idInformante);
            var ciius = Manager.Ciiu.Get("DAT_ENCUESTA_ANALISTA", t => t.Activado).Where(t => t.DAT_ENCUESTA_ANALISTA.Any(h => encuestas.Any(e => e.Id == h.id_encuesta)));
            
            var idTotal = "Sector Farril Total".GetHashCode();
            var idManufacturaPrimaria = "Manufactura Primaria".GetHashCode();
            var idManufacturaNoPrimaria = "Manufactura no Primaria".GetHashCode();
            var idBienesConsumo = "Bienes de Consumo".GetHashCode();
            var idBienesIntermedio = "Bienes Intermedios".GetHashCode();
            var idBienesCapital = "Bienes de Capital".GetHashCode();
            var idServicio = "Servicio".GetHashCode();
            var idCiiuInformante = "CIIU del Informante".GetHashCode();

            result.Indice.AddRange(new List<IniceElement>()
            {
                new IniceElement() {Id = idTotal,Name = "Sector Farril Total",Level = 0},
                new IniceElement() {Id = idManufacturaPrimaria,Name = "Manufactura Primaria",Level = 1},
                new IniceElement() {Id = idManufacturaNoPrimaria,Name = "Manufactura no Primaria",Level = 1},
                new IniceElement() {Id = idBienesConsumo,Name = "Bienes de Consumo",Level = 2},
                new IniceElement() {Id = idBienesIntermedio,Name = "Bienes Intermedios",Level = 2},
                new IniceElement() {Id = idBienesCapital,Name = "Bienes de Capital",Level = 2},
                new IniceElement() {Id = idServicio,Name = "Servicio",Level = 2},
                new IniceElement() {Id = idCiiuInformante,Name = "CIIU del Informante",Level = 1},
            });

            for (var i = 1; i <= 12; i++)
            {
                var from = new DateTime(year, i, 1);
                var to = new DateTime(year, i, DateTime.DaysInMonth(year, i));
                var total = 0d;
                var primaria = 0d;
                var noPrimaria = 0d;
                var bienesConsumo = 0d;
                var bienesIntermedios = 0d;
                var bienesCapital = 0d;
                var servicio = 0d;
                var ciiuInformante = 0d;
                result.Header.Add(new HeaderValue() { Number = i, Text = i.GetMonthText().Substring(0, 3) + "." });
                foreach (var ciiu in ciius)
                {                    
                    var res = CalculateIVF(ciiu.Id, from, to);
                    if (res.Count == 0) continue;
                    var ivfOb = res[0];
                    if (ivfOb.Values.Count == 0) continue;
                    var ivf = ivfOb.Values[0].Value;
                    total += ivf;
                    if (ciiu.EnumSubSector == EnumSubsector.Manufactura_No_Primaria)
                    {
                        primaria += ivf;
                        switch (ciiu.EnumRubro)
                        {
                            case EnumRubro.Bienes_DeCapital:
                                bienesCapital += ivf;
                                break;
                            case EnumRubro.Bienes_De_Consumo:
                                bienesConsumo += ivf;
                                break;
                            case EnumRubro.Bienes_Intermedios:
                                bienesIntermedios += ivf;
                                break;
                            case EnumRubro.Servicios:
                                servicio += ivf;
                                break;
                        }
                    }
                    else
                    {
                        noPrimaria += ivf;
                    }
                    var isCiiuInformante = true; //ciiu.Establecimientos.Any(t => t.IdInformante == idInformante);
                    if (isCiiuInformante)
                    {
                        ciiuInformante += ivf;
                        var elem = result.Indice.FirstOrDefault(t => t.Id == ciiu.Id);
                        if (elem == null)
                        {
                            result.Indice.Add(new IniceElement()
                            {
                                Id = ciiu.Id,
                                Name = ciiu.ToString(),
                                Level = 2,
                                Values = new List<MonthValue>()
                                {
                                    new MonthValue()
                                    {
                                        Number = i,
                                        Text = i.GetMonthText(),
                                        Value = ivf
                                    }
                                }
                            });
                        }
                        else
                        {
                            elem.Values.Add(new MonthValue()
                            {
                                Number = i,
                                Text = i.GetMonthText(),
                                Value = ivf
                            });
                        }


                    }

                }
                var totalEl = result.Indice.FirstOrDefault(t => t.Id == idTotal);
                var primariaEl = result.Indice.FirstOrDefault(t => t.Id == idManufacturaPrimaria);
                var noPrimariaEl = result.Indice.FirstOrDefault(t => t.Id == idManufacturaNoPrimaria);
                var bienesConsumoEl = result.Indice.FirstOrDefault(t => t.Id == idBienesConsumo);
                var bienesCapitalEl = result.Indice.FirstOrDefault(t => t.Id == idBienesCapital);
                var bienesIntermedioEl = result.Indice.FirstOrDefault(t => t.Id == idBienesIntermedio);
                var servicioEl = result.Indice.FirstOrDefault(t => t.Id == idServicio);
                var ciiuInformanteEl = result.Indice.FirstOrDefault(t => t.Id == idCiiuInformante);
                totalEl.Values.Add(new MonthValue()
                {
                    Number = i,
                    Text = i.GetMonthText(),
                    Value = total
                });
                primariaEl.Values.Add(new MonthValue()
                {
                    Number = i,
                    Text = i.GetMonthText(),
                    Value = primaria
                });
                noPrimariaEl.Values.Add(new MonthValue()
                {
                    Number = i,
                    Text = i.GetMonthText(),
                    Value = noPrimaria
                });
                bienesConsumoEl.Values.Add(new MonthValue()
                {
                    Number = i,
                    Text = i.GetMonthText(),
                    Value = bienesConsumo
                });
                bienesCapitalEl.Values.Add(new MonthValue()
                {
                    Number = i,
                    Text = i.GetMonthText(),
                    Value = bienesCapital
                });
                bienesIntermedioEl.Values.Add(new MonthValue()
                {
                    Number = i,
                    Text = i.GetMonthText(),
                    Value = bienesIntermedios
                });
                servicioEl.Values.Add(new MonthValue()
                {
                    Number = i,
                    Text = i.GetMonthText(),
                    Value = servicio
                });
                ciiuInformanteEl.Values.Add(new MonthValue()
                {
                    Number = i,
                    Text = i.GetMonthText(),
                    Value = ciiuInformante
                });
            }

            return result;

        }
        #endregion

        
        public void GenerateUploadExcel(List<EncuestaEstadisticaUploadModel> encuestaEstadisticasUpload,
                                        List<EncuestaEstadisticaUploadModel_2> encuestaEstadisticasUpload_2,
                                        List<EncuestaEstadisticaUploadModel_3> encuestaEstadisticasUpload_3,
                                        List<EncuestaEstadisticaUploadModel_4> encuestaEstadisticasUpload_4,
                                        List<EncuestaEstadisticaUploadModel_5> encuestaEstadisticasUpload_5)
        {
            List<long> idEncuestas = new List<long>();
            for (int i = 0; i < encuestaEstadisticasUpload.Count; i++)
            {
                var establecimiento = Manager.Establecimiento.Get(t => t.IdentificadorInterno == encuestaEstadisticasUpload[i].IdInternoEstablecimiento).FirstOrDefault();

                if (establecimiento != null)
                {
                    EncuestaEstadistica encuesta;

                    encuesta = Manager.EncuestaEstadistica.Get(t => t.Fecha.Month == encuestaEstadisticasUpload[i].Fecha.Month && t.Fecha.Year == encuestaEstadisticasUpload[i].Fecha.Year && t.IdEstablecimiento == establecimiento.Id).FirstOrDefault();

                    if (encuesta == null)
                    {
                        encuesta = new EncuestaEstadistica
                        {
                            IdEstablecimiento = establecimiento.Id,
                            EstadoEncuesta = EstadoEncuesta.Validada,
                            Fecha = encuestaEstadisticasUpload[i].Fecha,
                            IdInformante = establecimiento.Informante.Identificador,
                            VolumenProduccionMensual = new VolumenProduccion()
                            {
                            },
                            VentasProductosEstablecimiento = new VentasProductosEstablecimientos()
                            {
                                ServiciosActivados = establecimiento.TipoEnum == TipoEstablecimiento.Servicio ? true : false
                            },
                            TrabajadoresDiasTrabajados = new TrabajadoresDiasTrabajados(),
                            FactorProduccion = new FactorProducccion(),
                            fecha_validacion = DateTime.Now,
                            fecha_ultimo_envio = DateTime.Now
                        };

                        var encuestaEstadisticaLast = Manager.EncuestaEstadistica.Get().OrderBy(x => x.Id).LastOrDefault();
                        var encuestaEmpresarialLast = Manager.EncuestaEmpresarial.Get().OrderBy(x => x.Id).LastOrDefault();

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

                        Manager.EncuestaEstadistica.Add(encuesta);
                        Manager.EncuestaEstadistica.SaveChanges();

                        var first = establecimiento.CAT_ESTAB_ANALISTA.Select(t => t.orden).OrderBy(t => t).FirstOrDefault();
                        foreach (var analista in establecimiento.CAT_ESTAB_ANALISTA)
                        {
                            var ne = new EncuestaAnalista()
                            {
                                orden = analista.orden,
                                id_ciiu = analista.id_ciiu,
                                id_analista = analista.id_analista,
                                id_encuesta = encuesta.Id,
                                EstadoEncuesta = EstadoEncuesta.Validada,
                                current = (analista.orden == first) ? 1 : 0
                            };
                            Manager.EncuestaAnalistaManager.Add(ne);
                            Manager.EncuestaAnalistaManager.SaveChanges();
                        }
                        var volumenP = new VolumenProduccion()
                        {
                            Identificador = encuesta.Id,
                        };
                        Manager.VolumenProduccionManager.Add(volumenP);
                        Manager.VolumenProduccionManager.SaveChanges();

                        if (encuestaEstadisticasUpload[i].Materia == "P")
                        {
                            GenerateMateriaPropia(encuestaEstadisticasUpload[i], establecimiento, encuesta.Id);
                        }
                        else
                        {
                            GenerateMateriaTercero(encuestaEstadisticasUpload[i], encuesta.Id);
                        }

                        idEncuestas.Add(encuesta.Id);
                        var ciius = new[]
                        {
                            new{ciiu="2592",detalle="TRATAMIENTO Y REVESTIMIENTO DE METALES MAQUINADO"},
                            new{ciiu="3312",detalle="REPARACION Y MANTENIMIENTO DE MAQUINARIAS"},
                            new{ciiu="3314",detalle="REPARACION DE EQUIPOS ELECTRICOS"},
                            new{ciiu="3315",detalle="REPARACION DE EQUIPOS DE TRANSPORTE, EXCEPTO VEHICULOS AUTOMOTORES"},
                        };

                        foreach (var ciiu in ciius)
                        {
                            var serv = new VentasServicioManufactura()
                            {
                                IdVentaProductoestablecimiento = encuesta.VentasProductosEstablecimiento.Identificador,
                                ciiu = ciiu.ciiu,
                                detalle = ciiu.detalle
                            };
                            Manager.VentaServicioManufacturaManager.Add(serv);
                            Manager.VentaServicioManufacturaManager.SaveChanges();
                        }
                    }
                    else
                    {
                        if (encuestaEstadisticasUpload[i].Materia == "P")
                        {
                            GenerateMateriaPropia(encuestaEstadisticasUpload[i], establecimiento, encuesta.Id);
                        }
                        else
                        {
                            GenerateMateriaTercero(encuestaEstadisticasUpload[i], encuesta.Id);
                        }
                    }
                }
            }

            foreach (var idEncuesta in idEncuestas)
            {
                Manager.ValorProduccionManager.Generate(idEncuesta);
            }

            for (int i = 0; i < encuestaEstadisticasUpload_2.Count; i++)
            {
                var establecimiento = Manager.Establecimiento.Get(t => t.IdentificadorInterno == encuestaEstadisticasUpload_2[i].IdInternoEstablecimiento).FirstOrDefault();

                if (establecimiento != null)
                {
                    EncuestaEstadistica encuesta;

                    encuesta = Manager.EncuestaEstadistica.Get(t => t.Fecha.Month == encuestaEstadisticasUpload_2[i].Fecha.Month && t.Fecha.Year == encuestaEstadisticasUpload_2[i].Fecha.Year && t.IdEstablecimiento == establecimiento.Id).FirstOrDefault();

                    if (encuesta != null)
                    {
                        var ciiu = Manager.Ciiu.Get(x => x.Codigo == encuestaEstadisticasUpload_2[i].CodigoCIIU).FirstOrDefault();

                        var valorTerceros = Manager.ValorProduccionManager.Get(t => t.id_encuesta == encuesta.Id && t.id_ciiu == ciiu.Id).FirstOrDefault();

                        if (valorTerceros != null)
                        {
                            if (valorTerceros.MateriaPrimaTercerosActivada && encuestaEstadisticasUpload_2[i].ValorTercero == null)
                            {
                                continue;
                            }

                            valorTerceros.ProductosMateriaTerceros = encuestaEstadisticasUpload_2[i].ValorTercero;
                            valorTerceros.justificacion_materia_terc = null;
                            Manager.ValorProduccionManager.Modify(valorTerceros);
                            Manager.ValorProduccionManager.SaveChanges();
                        }

                        var valorVentasPaisExtranjero = Manager.VentasPaisExtranjeroManager.Get(t => t.id_ventas_producto == encuesta.Id && t.id_ciiu == ciiu.Id).FirstOrDefault();
                        var validVentasPaisExtranjero = true;
                        if (valorVentasPaisExtranjero != null)
                        {
                            if (valorVentasPaisExtranjero.VentaExtranjeroActivado && encuestaEstadisticasUpload_2[i].ValorVentaExtranjero == null)
                            {
                                validVentasPaisExtranjero = false;
                            }

                            if (valorVentasPaisExtranjero.VentaPaisActivado && encuestaEstadisticasUpload_2[i].ValorVentaPais == null)
                            {
                                validVentasPaisExtranjero = false;
                            }

                            if (validVentasPaisExtranjero == false) continue;

                            valorVentasPaisExtranjero.VentaExtranjero = encuestaEstadisticasUpload_2[i].ValorVentaExtranjero;
                            valorVentasPaisExtranjero.VentaPais = encuestaEstadisticasUpload_2[i].ValorVentaPais;
                            valorVentasPaisExtranjero.justificacion_venta_ext = null;
                            valorVentasPaisExtranjero.justificacion_venta_pais = null;

                            Manager.VentasPaisExtranjeroManager.Modify(valorVentasPaisExtranjero);
                            Manager.VentasPaisExtranjeroManager.SaveChanges();
                        }
                    }
                }
            }

            List<long> idEncuestasServicio = new List<long>();
            for (int i = 0; i < encuestaEstadisticasUpload_3.Count; i++)
            {
                var establecimiento = Manager.Establecimiento.Get(t => t.IdentificadorInterno == encuestaEstadisticasUpload_3[i].IdInternoEstablecimiento).FirstOrDefault();

                if (establecimiento != null)
                {
                    EncuestaEstadistica encuesta;

                    encuesta = Manager.EncuestaEstadistica.Get(t => t.Fecha.Month == encuestaEstadisticasUpload_3[i].Fecha.Month && t.Fecha.Year == encuestaEstadisticasUpload_3[i].Fecha.Year && t.IdEstablecimiento == establecimiento.Id).FirstOrDefault();

                    if (encuesta != null)
                    {
                        var servicio = Manager.VentaServicioManufacturaManager.Get(t => t.ciiu == encuestaEstadisticasUpload_3[i].CodigoCIIU && t.IdVentaProductoestablecimiento == encuesta.Id).FirstOrDefault();

                        if (servicio != null)
                        {
                            servicio.venta_extranjero = encuestaEstadisticasUpload_3[i].VentaExtranjero;
                            servicio.venta = encuestaEstadisticasUpload_3[i].VentaPais;
                            servicio.justificacion_venta_ext = null;
                            servicio.justificacion_venta_pais = null;
                            Manager.VentaServicioManufacturaManager.Modify(servicio);
                            Manager.VentaServicioManufacturaManager.SaveChanges();
                        }

                        if (idEncuestasServicio.Where(t => t == encuesta.Id).Count() == 0)
                        {
                            idEncuestasServicio.Add(encuesta.Id);
                        }
                    }
                }
            }

            foreach (var idEncuesta in idEncuestasServicio)
            {
                var ventasProd = Manager.VentasProductosEstablecimientoManager.Get(t => t.Identificador == idEncuesta).First();
                ventasProd.brindo_servicios = 1;
                Manager.VentasProductosEstablecimientoManager.Modify(ventasProd);
                Manager.VentasProductosEstablecimientoManager.SaveChanges();
            }

            for (int i = 0; i < encuestaEstadisticasUpload_4.Count; i++)
            {
                var establecimiento = Manager.Establecimiento.Get(t => t.IdentificadorInterno == encuestaEstadisticasUpload_4[i].IdInternoEstablecimiento).FirstOrDefault();

                if (establecimiento != null)
                {
                    EncuestaEstadistica encuesta;

                    encuesta = Manager.EncuestaEstadistica.Get(t => t.Fecha.Month == encuestaEstadisticasUpload_4[i].Fecha.Month && t.Fecha.Year == encuestaEstadisticasUpload_4[i].Fecha.Year && t.IdEstablecimiento == establecimiento.Id).FirstOrDefault();

                    if (encuesta != null)
                    {
                        encuesta.TrabajadoresDiasTrabajados.TrabajadoresProduccion = encuestaEstadisticasUpload_4[i].Trabajadores;
                        encuesta.TrabajadoresDiasTrabajados.DiasTrabajados = encuestaEstadisticasUpload_4[i].DiasTrabajados;
                        encuesta.TrabajadoresDiasTrabajados.AdministrativosPlanta = encuestaEstadisticasUpload_4[i].Administrativos;
                        Manager.TrabajadoresDiasTrabajadosManager.Modify(encuesta.TrabajadoresDiasTrabajados);
                        Manager.TrabajadoresDiasTrabajadosManager.SaveChanges();
                    }
                }
            }

            List<long> idsEncuestasFactores = new List<long>();
            for (int i = 0; i < encuestaEstadisticasUpload_5.Count; i++)
            {
                var establecimiento = Manager.Establecimiento.Get(t => t.IdentificadorInterno == encuestaEstadisticasUpload_5[i].IdInternoEstablecimiento).FirstOrDefault();

                if (establecimiento != null)
                {
                    EncuestaEstadistica encuesta;

                    encuesta = Manager.EncuestaEstadistica.Get(t => t.Fecha.Month == encuestaEstadisticasUpload_5[i].Fecha.Month && t.Fecha.Year == encuestaEstadisticasUpload_5[i].Fecha.Year && t.IdEstablecimiento == establecimiento.Id).FirstOrDefault();

                    if (encuesta != null)
                    {
                        idsEncuestasFactores.Add(encuesta.Id);

                        List<long> idsFactor = new List<long>();
                        bool isFactorIncremento = false;

                        EncuestaEstadisticaUploadModel_5 encuestaEstadisticaUpload_5 = encuestaEstadisticasUpload_5[i];
                        if (encuestaEstadisticaUpload_5.AumentoDemandaInterna)
                        {
                            isFactorIncremento = true;
                            idsFactor.Add(1);
                        }

                        if (encuestaEstadisticaUpload_5.AumentoCapacidadInstalada)
                        {
                            isFactorIncremento = true;
                            idsFactor.Add(2);
                        }

                        if (encuestaEstadisticaUpload_5.CambiosTecnologicos)
                        {
                            isFactorIncremento = true;
                            idsFactor.Add(6);
                        }

                        if (encuestaEstadisticaUpload_5.CampaniaEstacionalidadProducto)
                        {
                            isFactorIncremento = true;
                            idsFactor.Add(7);
                        }

                        if (encuestaEstadisticaUpload_5.IncrementoExportacion)
                        {
                            isFactorIncremento = true;
                            idsFactor.Add(8);
                        }

                        if (encuestaEstadisticaUpload_5.ReposicionExistencias)
                        {
                            isFactorIncremento = true;
                            idsFactor.Add(9);
                        }

                        if (encuestaEstadisticaUpload_5.CompetenciaDesleal)
                        {
                            idsFactor.Add(10);
                        }

                        if (encuestaEstadisticaUpload_5.ContrabandoPirateria)
                        {
                            idsFactor.Add(11);
                        }

                        if (encuestaEstadisticaUpload_5.DemandaInternaLimitada)
                        {
                            idsFactor.Add(12);
                        }

                        if(encuestaEstadisticaUpload_5.DificultadAccesoFinanciamiento)
                        {
                            idsFactor.Add(13);
                        }

                        if (encuestaEstadisticaUpload_5.DificultadAbastecimientoInsumos)
                        {
                            idsFactor.Add(14);
                        }

                        if (encuestaEstadisticaUpload_5.DisminucionExportaciones)
                        {
                            idsFactor.Add(15);
                        }

                        if (encuestaEstadisticaUpload_5.FaltaCapitalTrabajo)
                        {
                            idsFactor.Add(16);
                        }

                        if (encuestaEstadisticaUpload_5.FaltaEnergia)
                        {
                            idsFactor.Add(17);
                        }

                        if (encuestaEstadisticaUpload_5.FaltaPersonalCalificado)
                        {
                            idsFactor.Add(18);
                        }

                        if (encuestaEstadisticaUpload_5.MantenimientoEquipos)
                        {
                            idsFactor.Add(19);
                        }

                        if (encuestaEstadisticaUpload_5.VacacionesColectivas)
                        {
                            idsFactor.Add(20);
                        }

                        if (encuestaEstadisticaUpload_5.AltasExistencias)
                        {
                            idsFactor.Add(4);
                        }

                        if (encuestaEstadisticaUpload_5.HuelgaParos)
                        {
                            idsFactor.Add(5);
                        }

                        foreach (var idFactor in idsFactor)
                        {
                            var factor = Manager.Factor.Find(idFactor);
                            encuesta.FactorProduccion.CAT_FACTOR1.Add(factor);
                            Manager.EncuestaEstadistica.SaveChanges();
                        }

                        if (isFactorIncremento)
                        {
                            encuesta.FactorProduccion.IncrementoB = true;
                            Manager.EncuestaEstadistica.Modify(encuesta);
                            Manager.EncuestaEstadistica.SaveChanges();
                        }
                        else
                        {
                            encuesta.FactorProduccion.DecrecimientoB = true;
                            Manager.EncuestaEstadistica.Modify(encuesta);
                            Manager.EncuestaEstadistica.SaveChanges();
                        }
                    }
                }
            }

            foreach (var idEncuesta in idEncuestas)
            {
                if (idsEncuestasFactores.Where(t => t == idEncuesta).Count() == 0)
                {
                    var encuesta = Manager.EncuestaEstadistica.FindById(idEncuesta);                    
                    encuesta.FactorProduccion.ProduccionNormalB = true;
                    Manager.EncuestaEstadistica.Modify(encuesta);
                    Manager.EncuestaEstadistica.SaveChanges();
                }
            }
        }
        
        private void GenerateMateriaPropia(EncuestaEstadisticaUploadModel encuestaEstadisticasUpload, Establecimiento establecimiento, long IdVolumenProduccion)
        {
            bool ciiuEncontrado = false;
            foreach (var ciiu in establecimiento.Ciius)
            {
                if (ciiu.Ciiu.Codigo == encuestaEstadisticasUpload.CodigoCIIU)
                {
                    ciiuEncontrado = true;
                    break;
                }
            }

            if (ciiuEncontrado == false)
            {
                var ciiu = Manager.Ciiu.Get(x => x.Codigo == encuestaEstadisticasUpload.CodigoCIIU).FirstOrDefault();
                Manager.Establecimiento.AddCiiu(ciiu.Id, establecimiento.Id);
                Manager.Establecimiento.SaveChanges();
            }

            var lineaProducto = Manager.LineaProducto.Get(t => t.Codigo == encuestaEstadisticasUpload.CodigoLineaProducto).FirstOrDefault();
            var unidadMedida = Manager.UnidadMedida.Get(t => t.Abreviatura == encuestaEstadisticasUpload.AbreviaturaUM).FirstOrDefault();

            bool lineaProductoEncontrado = false;
            foreach (var lineaProductoEst in establecimiento.LineasProductoEstablecimiento)
            {
                if (lineaProducto.Id == lineaProductoEst.IdLineaProducto)
                {
                    lineaProductoEncontrado = true;
                    break;
                }
            }

            if (lineaProductoEncontrado == false)
            {
                Manager.LineaProductoEstablecimiento.Add(new LineaProductoEstablecimiento()
                {
                    IdEstablecimiento = establecimiento.Id,
                    IdLineaProducto = lineaProducto.Id
                });
                Manager.LineaProductoEstablecimiento.SaveChanges();
            }

            var lineaUnidadMedida = Manager.LineaProductoUnidadMedidaManager.Get(x => x.id_linea_producto == lineaProducto.Id && x.id_unidad_medida == unidadMedida.Id);

            if (lineaUnidadMedida == null)
            {
                Manager.LineaProductoUnidadMedidaManager.Add(new LineaProductoUnidadMedida()
                {
                    id_unidad_conversion = null,
                    factor_conversion = 0,
                    id_unidad_medida = unidadMedida.Id,
                    id_linea_producto = lineaProducto.Id
                });
                Manager.LineaProductoUnidadMedidaManager.SaveChanges();
            }

            var mp = new MateriaPropia()
            {
                IdLineaProducto = lineaProducto.Id,
                IdUnidadMedida = unidadMedida.Id,
                IdVolumenProduccion = IdVolumenProduccion,
                ValorUnitario = encuestaEstadisticasUpload.ValorUnitario,
                Existencia = encuestaEstadisticasUpload.Existencia,
                Produccion = encuestaEstadisticasUpload.Produccion,
                VentasPais = encuestaEstadisticasUpload.VentasPais,
                VentasExtranjero = encuestaEstadisticasUpload.VentasExtranjero,
                OtrasSalidas = encuestaEstadisticasUpload.OtrasSalidas
            };

            Manager.MateriaPropiaManager.Add(mp);
            Manager.MateriaPropiaManager.SaveChanges();

            UpdateExistenciasEncuestas(IdVolumenProduccion, lineaProducto.Id);
        }

        private void GenerateMateriaTercero(EncuestaEstadisticaUploadModel encuestaEstadisticasUpload, long IdVolumenProduccion)
        {
            var lineaProducto = Manager.LineaProducto.Get(t => t.Codigo == encuestaEstadisticasUpload.CodigoLineaProducto).FirstOrDefault();
            var unidadMedida = Manager.UnidadMedida.Get(t => t.Abreviatura == encuestaEstadisticasUpload.AbreviaturaUM).FirstOrDefault();

            MateriaTerceros materia = new MateriaTerceros();
            materia.IdVolumenProduccion = IdVolumenProduccion;
            materia.IdLineaProducto = lineaProducto.Id;
            materia.IdUnidadMedida = unidadMedida.Id;
            materia.UnidadProduccion = encuestaEstadisticasUpload.Produccion.ToString();

            var manager = Manager;
            var valid = !Manager.MateriaTercerosManager.ValidarProduccion(IdVolumenProduccion, materia.IdLineaProducto,
                   decimal.Parse(materia.UnidadProduccion));
            if (valid && (string.IsNullOrEmpty(materia.justificacion_produccion) || string.IsNullOrWhiteSpace(materia.justificacion_produccion)))
            {
                return;
            }

            manager.MateriaTercerosManager.Add(materia);
            manager.MateriaTercerosManager.SaveChanges();
            manager.ValorProduccionManager.Generate(IdVolumenProduccion);
        }
    }
}
