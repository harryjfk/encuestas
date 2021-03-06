﻿using System;
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
using Entity.Reportes;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
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
            return Repository.Get(t => t.Id == idEncuesta).FirstOrDefault();
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
        public bool GenerateCurrent(long idEstablecimiento, DateTime? date = null)
        {
            var now = date ?? DateTime.Now.Subtract(TimeSpan.FromDays(DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)));
            var establecimiento = Manager.Establecimiento.Find(idEstablecimiento);
            if (establecimiento == null || establecimiento.Informante == null) return false;
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
                foreach (var lp in establecimiento.LineasProductoEstablecimiento)
                {
                    //var temp = Manager.LineaProductoEstablecimiento.Find(lp.Id);
                    var um = lp.LineaProducto.LineasProductoUnidadMedida.FirstOrDefault();
                    if (um != null)
                    {
                        var mp = new MateriaPropia()
                        {
                            IdLineaProducto = lp.LineaProducto.Id,
                            IdUnidadMedida = um.id_unidad_medida.GetValueOrDefault(),
                            IdVolumenProduccion = volumenP.Identificador
                        };
                        var old = establecimiento.Encuestas.Where(t => t.Id != encuesta.Id && t.Fecha < encuesta.Fecha).OfType<EncuestaEstadistica>().OrderBy(t => t.Fecha).ToList();
                        // mp.IsFirst = true;
                        if (old.Count > 0)
                        {
                            var lastEnc = old.LastOrDefault();
                            if (lastEnc != null)
                            {
                                var lps =
                                    lastEnc.VolumenProduccionMensual.MateriasPropia.Where(
                                        t => t.IdLineaProducto == mp.IdLineaProducto);
                                var materiaPropias = lps as IList<MateriaPropia> ?? lps.ToList();
                                var allProd = materiaPropias.Sum(t => t.Produccion);
                                var allOtrosIngresos = materiaPropias.Sum(t => t.OtrosIngresos);
                                var allVentasPais = materiaPropias.Sum(t => t.VentasPais);
                                var allVentasExtranjero = materiaPropias.Sum(t => t.VentasExtranjero);
                                var allOtrasSalidas = materiaPropias.Sum(t => t.OtrasSalidas);
                                // mp.IsFirst = false;
                                mp.Existencia = (allProd + allOtrosIngresos) -
                                                (allVentasPais + allVentasExtranjero + allOtrasSalidas);
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
                return true;
            }
            return false;
        }

        public void UpdateExistenciasEncuestas(long idEncuesta, long idLineaProd)
        {
            var encuesta = Manager.EncuestaEstadistica.Find(idEncuesta);
            var lineaProd = Manager.LineaProducto.Find(idLineaProd);
            if (encuesta == null || lineaProd == null) return;
            var rest = Manager.EncuestaEstadistica.Get(t => t.Fecha > encuesta.Fecha).ToList();
            foreach (var encuestaEstadistica in rest.OrderBy(t => t.Fecha))
            {
                UpdateExistenciasEncuesta(encuestaEstadistica, lineaProd);
            }
        }
        private void UpdateExistenciasEncuesta(EncuestaEstadistica encuesta, LineaProducto lineaProd)
        {
            if (encuesta == null || lineaProd == null) return;
            var establecimiento = encuesta.Establecimiento;
            var old = establecimiento.Encuestas.Where(t => t.Id != encuesta.Id && t.Fecha < encuesta.Fecha).OfType<EncuestaEstadistica>().OrderBy(t => t.Fecha).ToList();
            // mp.IsFirst = true;
            if (old.Count > 0)
            {
                var mp =
                    encuesta.VolumenProduccionMensual.MateriasPropia.FirstOrDefault(t => t.IdLineaProducto == lineaProd.Id);
                if (mp == null) return;
                var lastEnc = old.LastOrDefault();
                if (lastEnc != null)
                {
                    var lps =
                        lastEnc.VolumenProduccionMensual.MateriasPropia.Where(
                            t => t.IdLineaProducto == lineaProd.Id);
                    var materiaPropias = lps as IList<MateriaPropia> ?? lps.ToList();
                    var allProd = materiaPropias.Sum(t => t.Produccion);
                    var allOtrosIngresos = materiaPropias.Sum(t => t.OtrosIngresos);
                    var allVentasPais = materiaPropias.Sum(t => t.VentasPais);
                    var allVentasExtranjero = materiaPropias.Sum(t => t.VentasExtranjero);
                    var allOtrasSalidas = materiaPropias.Sum(t => t.OtrasSalidas);
                    // mp.IsFirst = false;
                    mp.Existencia = (allProd + allOtrosIngresos) -
                                    (allVentasPais + allVentasExtranjero + allOtrasSalidas);
                    Manager.MateriaPropiaManager.Modify(mp);
                    Manager.MateriaPropiaManager.SaveChanges();
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
            var current = manager.EncuestaEstadistica.Find(encuesta.Id);
            if (current == null)
            {
                return null;
            }
            //CONTACTO
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
                current.EstadoEncuesta = EstadoEncuesta.Enviada;
                current.fecha_ultimo_envio = DateTime.Now;
                manager.EncuestaEstadistica.Modify(current);
                manager.EncuestaEstadistica.SaveChanges();
            }
            return list;
        }

        public List<string> SalvarContacto(EncuestaEstadistica encuesta)
        {
            var manager = Manager;
            var list = new List<string>();
            var current = manager.EncuestaEstadistica.Find(encuesta.Id);
            if (current == null)
            {
                list.Add("Problemas con los datos");
                return list;
            }
            //CONTACTO
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
            var encuesta = Manager.EncuestaEstadistica.Find(idEncuesta);
            var lineaprod = Manager.LineaProducto.Find(linea.Id);
            var ciiu = Manager.Ciiu.Find(linea.IdCiiu);
            if (ciiu != null && encuesta != null && lineaprod != null)
            {
                long idUM = 0;
                var first = lineaprod.LineasProductoUnidadMedida.FirstOrDefault();
                idUM = first != null ? first.id_unidad_medida.GetValueOrDefault() : 0;
                var establecimiento = encuesta.Establecimiento;
                if (establecimiento.Ciius.Any(t => t.Id == ciiu.Id))
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
                    establecimiento.Ciius.Add(ciiu);
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
                if (element.FactorProduccion.CAT_FACTOR1.Count(t => t.TipoFactor == TipoFactor.Incremento) == 0)
                {
                    list.Add("Usted señaló Incremento dentro de Factores que Afectaron la producción. Debe escoger al menos un Factor de tipo incremento");
                }
            }
            if (element.FactorProduccion.DecrecimientoB)
            {
                if (element.FactorProduccion.CAT_FACTOR1.Count(t => t.TipoFactor == TipoFactor.Disminución) == 0)
                {
                    list.Add("Usted señaló Disminución dentro de Factores que Afectaron la producción. Debe escoger al menos un Factor de tipo disminución");
                }
            }

            element = Manager.EncuestaEstadistica.Find(element.Id);
            var sum =
                element.VolumenProduccionMensual.MateriasPropia.Sum(
                    t =>
                        t.ValorUnitario.GetValueOrDefault() + t.Produccion.GetValueOrDefault() + t.OtrosIngresos.GetValueOrDefault() + t.OtrasSalidas.GetValueOrDefault() + t.VentasExtranjero.GetValueOrDefault() +
                        t.VentasPais.GetValueOrDefault());
            if (sum == 0)
            {
                list.Add("Debe ingresar los valores de al menos una línea de producto en \"Productos fabricados con materia prima propia para venta y/o autoconsumo del establecimiento\"");
            }
            foreach (var ter in element.CAT_VALOR_PROD_MENSUAL)
            {
                if (ter.MateriaPrimaTercerosActivada && ter.ProductosMateriaTerceros == null)
                {
                    list.Add("Debe ingresar todos los valores de materia prima de terceros de la producción mensual del establecimiento");
                    break;
                }
            }

            foreach (var venta in element.VentasProductosEstablecimiento.CAT_VENTAS_PAIS_EXTRANJERO)
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
                && t.CAT_ENCUESTA_ANALISTA.Any(h => h.id_analista == idAnalista && (h.IsCurrent || h.IsPast))
                && (t.EstadoEncuesta != EstadoEncuesta.NoEnviada));
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
            var encuesta = Manager.EncuestaEstadistica.Find(id);
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
            var encuesta = Manager.EncuestaEstadistica.Find(id);
            if (encuesta == null) return;

            var analistas = encuesta.CAT_ENCUESTA_ANALISTA.OrderBy(t => t.current);
            var tt = analistas.FirstOrDefault(t => t.IsCurrent);
            if (tt == null) return;
            var rest = analistas.Where(t => t.id_analista == tt.id_analista);
            foreach (var encuestaAnalista in rest)
            {
                encuestaAnalista.current = 2;
                encuestaAnalista.EstadoEncuesta = EstadoEncuesta.Validada;
                Manager.EncuestaAnalistaManager.Modify(encuestaAnalista);
            }
            var next = analistas.FirstOrDefault(t => t.orden > tt.orden && t.id_analista != tt.id_analista);
            if (next != null)
            {
                next.current = 1;
                Manager.EncuestaAnalistaManager.Modify(next);
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
            }
            Manager.EncuestaAnalistaManager.SaveChanges();
        }


        #region REPORTES

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
                    result.Elements.AddRange(CalculateIVF1(ciiu.Id, from, to));
                }
            }
            else
            {
                result.Elements.AddRange(CalculateIVF1(idCiiu.GetValueOrDefault(), from, to));
            }
            return result;
        }

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
        public List<IndiceValorFisicoProducidoItem> CalculateIVF2(long idCiiu, DateTime from, DateTime to)
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
                        .SelectMany(t => t.CAT_VALOR_PROD_MENSUAL);
                    return materias.ToList();
                case "ciiu":
                    var materias2 = Manager.EncuestaEstadistica.Get(
                        t => idEstablecimientos.Contains(t.IdEstablecimiento)
                             && t.Fecha.Year == year &&
                             month.Contains(t.Fecha.Month))
                        .SelectMany(t => t.CAT_VALOR_PROD_MENSUAL).Where(t => ids.Contains(t.id_ciiu));
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
                        .SelectMany(t => t.VentasProductosEstablecimiento.CAT_VENTAS_PAIS_EXTRANJERO);
                    return materias.ToList();
                case "ciiu":
                    var materias2 = Manager.EncuestaEstadistica.Get(
                        t => idEstablecimientos.Contains(t.IdEstablecimiento)
                             && t.Fecha.Year == year &&
                             month.Contains(t.Fecha.Month))
                        .SelectMany(t => t.VentasProductosEstablecimiento.CAT_VENTAS_PAIS_EXTRANJERO).Where(t => ids.Contains(t.id_ciiu));
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
        #endregion
    }
}
