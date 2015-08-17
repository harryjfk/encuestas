using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using Domain;
using Domain.Managers;
using Entity;
using PagedList;
using WebApplication.Models;
using Seguridad.PRODUCE;

namespace WebApplication.Controllers
{
    //[Authorize]
    //[Autorizacion]
    public class EncuestaEstadisticaController : BaseController<EncuestaEstadistica>
    {
        private static Query<Contacto> QueryContacto { get; set; }
        public static long IdEncuestaInformante { get; set; }
        private static long IdEstablecimiento { get; set; }
        private static EncuestaEstadistica Model { get; set; }

        public ActionResult GetDorpDown(string id, string nombre = "IdEncuestaEstadistica", string @default = null)
        {
            var list = OwnManager.Get(t => true).Select(t => new SelectListItem()
             {
                 Text = t.ToString(),
                 Value = t.Id.ToString(),
                 Selected = t.Id.ToString() == id
             }).ToList();
            if (@default != null)
                list.Insert(0, new SelectListItem()
                {
                    Selected = id == "0",
                    Value = "0",
                    Text = @default
                });
            return View("_DropDown", Tuple.Create<IEnumerable<SelectListItem>, string>(list, nombre));
        }

        public ActionResult Encuestas(long id)
        {
            int s_userId;
            if (Session["uid"] == null)
            {
                return HttpNotFound("Usuario no encontrado");
            }
            if (int.TryParse(Session["uid"].ToString(), out s_userId) == false)
            {
                return HttpNotFound("Usuario no encontrado");
            }

            //var user = this.GetLogued();
            var user = Manager.Usuario.FindUsuarioExtranet(s_userId);
            if (user == null) return HttpNotFound("Usuario no encontrado");

            var manager = Manager;
            IdEstablecimiento = id;
            var establecimiento = Manager.Establecimiento.Find(id);
            if (establecimiento == null)
                return HttpNotFound("Establecimiento no encontrado");
            ViewBag.Establecimiento = establecimiento;
            manager.EncuestaEstadistica.GenerateCurrent(IdEstablecimiento);
            Query = Query ?? new Query<EncuestaEstadistica>();
            Query = Query.Validate();
            Query.Criteria = Query.Criteria ?? new EncuestaEstadistica();
            Query.Criteria.IdEstablecimiento = IdEstablecimiento;
            Query.Criteria.IdInformante = user.Identificador;
            Query.Paginacion = Query.Paginacion ?? new Paginacion();
            Query.Paginacion.Page = 1;
            Query.BuildFilter();
            Manager.EncuestaEstadistica.GetAsignadosInformante(Query);
            ModelState.Clear();
            return View("Index", Query);
        }

        public ActionResult EncuestasAnalista(long id)
        {
            int s_userId;
            if (Session["uid"] == null)
            {
                return HttpNotFound("Usuario no encontrado");
            }
            if (int.TryParse(Session["uid"].ToString(), out s_userId) == false)
            {
                return HttpNotFound("Usuario no encontrado");
            }

            //var user = this.GetLogued();
            var user = Manager.Usuario.FindUsuarioIntranet(s_userId);
            if (user == null) return HttpNotFound("Usuario no encontrado");

            var manager = Manager;
            IdEstablecimiento = id;
            var establecimiento = Manager.Establecimiento.Find(id);
            if (establecimiento == null)
                return HttpNotFound("Establecimiento no encontrado");
            ViewBag.Establecimiento = establecimiento;
            //manager.EncuestaEmpresarial.GenerateCurrent(IdEstablecimiento);
            Query = Query ?? new Query<EncuestaEstadistica>();
            Query = Query.Validate();
            Query.Criteria = Query.Criteria ?? new EncuestaEstadistica();
            Query.Criteria.IdEstablecimiento = IdEstablecimiento;
           // Query.Criteria.IdAnalista = user.Identificador;
            Query.Paginacion = Query.Paginacion ?? new Paginacion();
            Query.Paginacion.Page = 1;
            Query.BuildFilter();
            Manager.EncuestaEstadistica.GetAsignadosAnalista(Query, (long)user.Identificador);
            ModelState.Clear();
            return View("IndexAnalista", Query);
        }

        public ActionResult Encuesta(long idEncuesta = 0)
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(idEncuesta);
            if (encuesta == null) return HttpNotFound("Encuesta No encontrada");
            Model = encuesta;
            if (encuesta.EstadoEncuesta == EstadoEncuesta.Observada || encuesta.EstadoEncuesta == EstadoEncuesta.NoEnviada)
                return View("Encuesta", encuesta);
            return View("VerEncuesta", encuesta);
        }

        public ActionResult EncuestaAnalista(long idEncuesta = 0)
        {
            var user = this.GetLogued();
            
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(idEncuesta);
            if (encuesta == null) return HttpNotFound("Encuesta No encontrada");
            var analist = encuesta.CAT_ENCUESTA_ANALISTA.FirstOrDefault(t => t.id_analista == user.Identificador);
            if (analist == null || analist.IsWaiting) return HttpNotFound("Encuesta No encontrada");
            if (encuesta.EstadoEncuesta == EstadoEncuesta.Enviada || analist.IsCurrent)
                return View("EncuestaAnalista", encuesta);
            return View("VerEncuesta", encuesta);
        }

        public ActionResult VerEncuesta(long idEncuesta = 0)
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(idEncuesta);
            if (encuesta == null) return HttpNotFound("Encuestano encontrda");
            if (encuesta.EstadoEncuesta == EstadoEncuesta.NoEnviada)
            {
                encuesta.EstadoEncuesta = EstadoEncuesta.Observada;
                manager.EncuestaEstadistica.Modify(encuesta);
                manager.EncuestaEstadistica.SaveChanges();
            }
            return View("VerEncuesta", encuesta);

        }

        [HttpPost]
        public JsonResult Enviar(EncuestaEstadistica encuesta)
        {
            var manager = Manager;
            var errors = manager.EncuestaEstadistica.Enviar(encuesta, true);
            var saved = manager.EncuestaEstadistica.FindById(encuesta.Id);
            if (errors.Count == 0)
            {

                var data = RenderRazorViewToString("Agradecimiento", saved);
                var res = new
                {
                    Data = data,
                    Success = true
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ModelState.AddModelError("Error", "Error al enviar la Encuesta");
                var data = RenderRazorViewToString("Encuesta", saved);
                var res = new
                {
                    Success = false,
                    Data = data,
                    Errors = errors
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult Observar(long id, string observacion)
        {
            Manager.EncuestaEstadistica.Observar(id, observacion);
            return RedirectToAction("EncuestasAnalista", new { id = IdEstablecimiento });
        }

        public ActionResult Validar(long id)
        {
            Manager.EncuestaEstadistica.Validar(id);
            return RedirectToAction("EncuestasAnalista", new { id = IdEstablecimiento });
        }

        public ActionResult GetNombreDistrito(string idDistrito)
        {
            var distrito = Manager.Distrito.Find(idDistrito);
            var name = "...";
            if (distrito != null)
                name = distrito.Nombre;
            return PartialView("_TextView", name);
        }
        public ActionResult GetNombreDepartamento(string idDepartamento)
        {
            var departamento = Manager.Departamento.Find(idDepartamento);
            var name = "...";
            if (departamento != null)
                name = departamento.Nombre;
            return PartialView("_TextView", name);            
        }

        public ActionResult GetNombreProvincia(string idProvincia)
        {            
            var provincia = Manager.Provincia.Find(idProvincia);
            var name = "...";
            if (provincia != null)
                name = provincia.Nombre;
            return PartialView("_TextView", name);            
        }

        public JsonResult DeleteProdMatTer(long id)
        {
            var manager = Manager;
            var element = manager.MateriaTercerosManager.Find(id);
            if (element == null)
            {
                var res = new
                {
                    Success = false,
                    Errors = new List<string>() { "Elemento no encontrado" }
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            var idVolumen = element.VolumenProduccion.Identificador;
            var op = manager.MateriaTercerosManager.Delete(id);
            manager.MateriaTercerosManager.SaveChanges();
            if (op.Success)
            {
                var elements = Manager.MateriaTercerosManager.Get(t => t.VolumenProduccion.Identificador == idVolumen);
                var data = RenderRazorViewToString("_TableProductoMateriaPrimaTerceros", elements);
                var res = new
                {
                    Success = true,
                    Data = data
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var res = new
                {
                    Success = false,
                    Errors = new List<string>() { "Elemento no encontrado" }
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "Informante")]
        public ActionResult EditProdMatTer(long id = 0)
        {
            var producto = Manager.MateriaTercerosManager.Find(id);
            producto = producto ?? new MateriaTerceros();
            return PartialView("_ProductoMateriaPrimaTerceros", producto);
        }
        [HttpPost]
        [Authorize(Roles = "Informante")]
        public JsonResult CreateProdMatTer(MateriaTerceros producto)
        {
            if (ModelState.IsValid)
            {
                var manager = Manager;
                producto.IdVolumenProduccion = Model.VolumenProduccionMensual.Identificador;
                var op = producto.Id == 0 ?
                    manager.MateriaTercerosManager.Add(producto) :
                    manager.MateriaTercerosManager.Modify(producto);
                if (op.Success)
                {
                    manager.MateriaTercerosManager.SaveChanges();
                    var elements = Manager.MateriaTercerosManager.Get(t => t.IdVolumenProduccion == Model.VolumenProduccionMensual.Identificador);
                    var data = RenderRazorViewToString("_TableProductoMateriaPrimaTerceros", elements);
                    var res = new
                    {
                        Success = true,
                        Data = data
                    };
                    return Json(res, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var result = new
                    {
                        Success = false,
                        Errors = new List<string>() { op.Errors[0] }
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var list = new List<string>();
                foreach (var v in ModelState.Values)
                    list.AddRange(v.Errors.Select(t => t.ErrorMessage));
                var result = new
                {
                    Success = false,
                    Errors = list
                };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public ActionResult BuscarEncuestaInformante(EncuestaEstadistica criteria)
        {
            Query = Query ?? new Query<EncuestaEstadistica>().Validate();
            Query.Criteria = criteria;
            Query.Paginacion = Query.Paginacion ?? new Paginacion();
            Query.Paginacion.Page = 1;
            Query.Criteria.IdEstablecimiento = IdEstablecimiento;
            //Query.BuildFilter();
            return RedirectToAction("Encuestas", new { id = IdEstablecimiento });
        }
        [HttpPost]
        public ActionResult BuscarEncuestaAnalista(EncuestaEstadistica criteria)
        {
            Query = Query ?? new Query<EncuestaEstadistica>().Validate();
            Query.Criteria = criteria;
            Query.Paginacion = Query.Paginacion ?? new Paginacion();
            Query.Paginacion.Page = 1;
            Query.Criteria.IdEstablecimiento = IdEstablecimiento;
            //Query.BuildFilter();
            return RedirectToAction("EncuestasAnalista", new { id = IdEstablecimiento });
        }

        public ActionResult ContactosEncuesta(long id)
        {
            //var id = IdEstablecimiento;
            var manager = Manager;
            var establecimiento = manager.Establecimiento.Find(id);
            if (establecimiento == null) return HttpNotFound("Establecimiento no encontrado");
            QueryContacto = QueryContacto ?? new Query<Contacto>().Validate();
            QueryContacto.Criteria = QueryContacto.Criteria ?? new Contacto();            
            QueryContacto.Criteria.IdEstablecimiento = id;
            QueryContacto.Paginacion.ItemsPerPage = 5;
            QueryContacto.BuildFilter();
            Manager.Contacto.Get(QueryContacto);
            return PartialView("_ContactosEncuesta", Tuple.Create<Contacto, Query<Contacto>>(null, QueryContacto));
        }
        [HttpPost]
        public virtual JsonResult CreateNuevoContacto(Contacto element)
        {
            if (ModelState.IsValid)
            {
                var id = IdEstablecimiento;
                var establecimiento = Manager.Establecimiento.Find(id);
                if (establecimiento == null)
                {
                    var result = new
                    {
                        Success = false,
                        Errors = new List<string>() { "Establecimiento no encontrado" }
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                element.IdEstablecimiento = IdEstablecimiento;
                QueryContacto = QueryContacto ?? new Query<Contacto>().Validate();
                QueryContacto.Criteria = QueryContacto.Criteria ?? new Contacto();
                QueryContacto.Criteria.IdEstablecimiento = IdEstablecimiento;
                QueryContacto.BuildFilter();
                var manager = Manager.Contacto;
                var op = manager.Add(element);

                if (op.Success)
                {
                    manager.SaveChanges();
                    //this.WriteMessage(string.Format("Actualizando datos del ciiu {0}", ciiu.Nombre));
                    Manager.Contacto.Get(QueryContacto);
                    var c = RenderRazorViewToString("_ContactosEncuesta", Tuple.Create<Contacto, Query<Contacto>>(null, QueryContacto));
                    var result = new
                    {
                        Success = true,
                        Data = c
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = new
                    {
                        Success = false,
                        Errors = op.Errors.Select(t => t).ToList()
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var list = new List<string>();
                foreach (var v in ModelState.Values)
                    list.AddRange(v.Errors.Select(t => t.ErrorMessage));
                var result = new
                {
                    Success = false,
                    Errors = list
                };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }
        public virtual ActionResult PageContactoEncuesta(int page)
        {
            QueryContacto = QueryContacto ?? new Query<Contacto>().Validate();
            QueryContacto.Paginacion.Page = page;
            return RedirectToAction("ContactosEncuesta");
        }
        public ActionResult ToggleContacto(int id)
        {
            Manager.Contacto.EstablecerPredeterminado(id);
            Model = Manager.EncuestaEstadistica.FindById(Model.Id);
            return PartialView("_IdentificacionEstablecimiento", Model);
        }

        [HttpPost]
        public JsonResult SaveMateriaPropia(List<MateriaPropia> materias)
        {
            var manager = Manager;
            var produccion = new List<long>();
            var valorUnitario = new List<long>();
            var ventaPais = new List<long>();
            var ventaExtranjero = new List<long>();
            var errors = new List<string>();
            foreach (var materia in materias)
            {
                var mat = manager.MateriaPropiaManager.Find(materia.Id);
                if (mat != null)
                {
                    if (!materia.IsValid)
                    {
                        errors.Add(string.Format("Los valores insertados para la línea de producto {0} no son correctos. (Existencias + Produccion + Otros Ingresos) ≥ (VentasPais + Ventas Extranjeros + Otras Salidas)", mat.LineaProducto.Nombre));
                        continue;
                    }
                    if (mat.IsFirst)
                    {
                        mat.Existencia = materia.Existencia;
                        // manager.EncuestaEstadistica.UpdateExistenciasEncuestas(mat.VolumenProduccion.Encuesta.Id,mat.IdLineaProducto);
                        //MODIFICAR RESTO DE LAS ENCUESTA AQUI
                    }
                    mat.IdUnidadMedida = materia.IdUnidadMedida > 0 ? materia.IdUnidadMedida : mat.IdUnidadMedida;
                    mat.JustificacionProduccion = materia.JustificacionProduccion;
                    mat.JustificacionValorUnitario = materia.JustificacionValorUnitario;
                    mat.justificacion_venta_extranjero = materia.justificacion_venta_extranjero;
                    mat.justificacion_venta_pais = materia.justificacion_venta_pais;
                    mat.OtrasSalidas = materia.OtrasSalidas;
                    mat.OtrosIngresos = materia.OtrosIngresos;
                    mat.Produccion = materia.Produccion;
                    mat.ValorUnitario = materia.ValorUnitario;
                    mat.VentasExtranjero = materia.VentasExtranjero;
                    mat.VentasPais = materia.VentasPais;
                    manager.MateriaPropiaManager.Modify(mat);
                    manager.MateriaPropiaManager.SaveChanges();
                    var validProduccion = !Manager.MateriaPropiaManager.ValidarProduccion(materia.Id, materia.Produccion);
                    var validValorUnitario = !Manager.MateriaPropiaManager.ValidarValorUnitario(materia.Id, materia.ValorUnitario);
                    var validVentasPais = !Manager.MateriaPropiaManager.ValidarVentasPais(materia.Id, materia.VentasPais);
                    var validVentasExtranjero = !Manager.MateriaPropiaManager.ValidarVentasExtranjero(materia.Id, materia.VentasExtranjero);

                    if (validProduccion && materia.Produccion != null)
                        produccion.Add(materia.Id);
                    if (validValorUnitario && materia.ValorUnitario != null)
                        valorUnitario.Add(materia.Id);
                    if (validVentasPais && materia.VentasPais != null)
                        ventaPais.Add(materia.Id);
                    if (validVentasExtranjero && materia.VentasExtranjero != null)
                        ventaExtranjero.Add(materia.Id);

                }
                //else
                //{
                //    var result = new
                //    {
                //        Success = false
                //    };
                //    return Json(result, JsonRequestBehavior.AllowGet);
                //}
                Manager.EncuestaEstadistica.UpdateExistenciasEncuestas(mat.VolumenProduccion.Encuesta.Id, mat.IdLineaProducto);
            }
            if (errors.Any())
            {
                var res = new
                {
                    Success = false,
                    Errors = errors
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            var model = Manager.EncuestaEstadistica.FindById(Model.Id);
            manager.ValorProduccionManager.Generate(Model.Id);
            var d = this.RenderRazorViewToString("_ValorProduccionEstablecimiento", model);
            var e = this.RenderRazorViewToString("_ValorVentasProductosEstablecimiento", model);
            var result = new
            {
                Success = true,
                ValorUnitario = valorUnitario,
                Produccion = produccion,
                ValorProduccion = d,
                Ventas = e,
                VentasPais = ventaPais,
                VentasExtranjero = ventaExtranjero
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveValorProduccion(List<ValorProduccion> valores)
        {
            var manager = Manager;
            var list = new List<long>();
            var errors = new List<string>();
            foreach (var materia in valores)
            {

                var valor = manager.ValorProduccionManager.Find(materia.Id);
                if (valor != null)
                {
                    if (valor.MateriaPrimaTercerosActivada && materia.ProductosMateriaTerceros == null)
                    {
                        errors.Add(string.Format("Debe ingresar un valor en  Materia prima de terceros para el CIIU {0}", valor.CAT_CIIU.Nombre));
                        continue;
                    }
                    //valor.ProductosMateriaPropia = materia.ProductosMateriaPropia;
                    valor.ProductosMateriaTerceros = materia.ProductosMateriaTerceros;
                    valor.justificacion_materia_terc = materia.justificacion_materia_terc;
                    var validmateriaTerceros = !Manager.ValorProduccionManager.ValidarMateriaTerceros(materia.Id, materia.ProductosMateriaTerceros, materia.id_ciiu);
                    manager.ValorProduccionManager.Modify(valor);
                    manager.ValorProduccionManager.SaveChanges();
                    if (validmateriaTerceros && (string.IsNullOrEmpty(valor.justificacion_materia_terc) || string.IsNullOrWhiteSpace(valor.justificacion_materia_terc)))
                    {
                        list.Add(materia.Id);
                    }

                }
            }
            if (errors.Count > 0)
            {
                var res = new
                {
                    Elements = new List<long>(),
                    Success = false,
                    Errors = errors
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            if (list.Count > 0)
            {
                var res = new
                {
                    Elements = list,
                    Success = false,
                    Errors = new List<string>() { "Debe establecer una justificación para el valor de materia prima de terceros." }
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            var result = new
            {
                Success = true
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveVentas(List<VentasPaisExtranjero> ventas)
        {
            var manager = Manager;
            var ext = new List<long>();
            var pais = new List<long>();
            var errors = new List<string>();
            foreach (var venta in ventas)
            {
                var valor = manager.VentasPaisExtranjeroManager.Find(venta.Id);
                var s = false;
                if (valor != null)
                {

                    if (valor.VentaExtranjeroActivado && venta.VentaExtranjero == null)
                    {
                        errors.Add(string.Format("Debe ingresar un valor en  Venta  extranjero para el CIIU {0}", valor.CAT_CIIU.Nombre));
                        s = true;
                    }
                    if (valor.VentaPaisActivado && venta.VentaPais == null)
                    {
                        errors.Add(string.Format("Debe ingresar un valor en  Venta  pais para el CIIU {0}", valor.CAT_CIIU.Nombre));
                        s = true;
                    }
                    if (s) continue;

                    valor.VentaExtranjero = venta.VentaExtranjero;
                    valor.VentaPais = venta.VentaPais;
                    valor.justificacion_venta_ext = venta.justificacion_venta_ext;
                    valor.justificacion_venta_pais = venta.justificacion_venta_pais;

                    manager.VentasPaisExtranjeroManager.Modify(valor);
                    manager.VentasPaisExtranjeroManager.SaveChanges();

                    var validExt =
                       !manager.VentasPaisExtranjeroManager.ValidarVentaExtranjero(venta.Id, venta.VentaExtranjero);
                    var validPais =
                        !manager.VentasPaisExtranjeroManager.ValidarVentaPais(venta.Id, venta.VentaPais);
                    if (validExt && (string.IsNullOrWhiteSpace(valor.justificacion_venta_ext) || string.IsNullOrEmpty(valor.justificacion_venta_ext)))
                        ext.Add(venta.Id);
                    if (validPais && (string.IsNullOrWhiteSpace(valor.justificacion_venta_pais) || string.IsNullOrEmpty(valor.justificacion_venta_pais)))
                        pais.Add(venta.Id);
                }
            }
            if (errors.Count > 0)
            {
                var res = new
                {
                    Pais = new List<long>(),
                    Extranjero = new List<long>(),
                    Success = false,
                    Errors = errors
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            if (ext.Count > 0 || pais.Count > 0)
            {
                var res = new
                {
                    Pais = pais,
                    Extranjero = ext,
                    Success = false,
                    Errors = new List<string>() { "Debe proporcionar una justificación para los valores de ventas en el pais y ventas en el extranjero." }
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            var result = new
            {
                Success = true
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDorpDownLineaProducto(string id, long idCiiu = 0, string nombre = "IdLineaProducto", string @default = null)
        {
            var list = Manager.LineaProducto.Get(t => t.Activado
                && t.MateriaTercero.All(h => h.VolumenProduccion.Encuesta.Id != Model.Id)
                /*&& t.Ciiu.Establecimientos.Any(h=>h.Id==IdEstablecimiento*/
                && t.IdCiiu == idCiiu && t.LineasProductoEstablecimiento.All(h => h.IdEstablecimiento != IdEstablecimiento))
                .Select(t => new SelectListItem()
                {
                    Text = t.ToString(),
                    Value = t.Id.ToString(),
                    Selected = t.Id.ToString() == id
                }).ToList();
            if (@default != null)
                list.Insert(0, new SelectListItem()
                {
                    Selected = id == "0",
                    Value = "0",
                    Text = @default
                });
            return View("_DropDown", Tuple.Create<IEnumerable<SelectListItem>, string>(list, nombre));
        }

        public ActionResult GetDorpDownLineaProductoNotIncluded(string id, long idCiiu = 0, string nombre = "IdLineaProducto", string @default = null)
        {
            var list = Manager.LineaProducto.Get(t => t.Activado && t.LineasProductoUnidadMedida.Any()
                && t.MateriaTercero.All(h => h.VolumenProduccion.Encuesta.Id != Model.Id)
                /*&& t.Ciiu.Establecimientos.Any(h=>h.Id==IdEstablecimiento*/
                && t.IdCiiu == idCiiu && t.LineasProductoEstablecimiento.All(h => h.IdEstablecimiento != IdEstablecimiento))
                .Select(t => new SelectListItem()
                {
                    Text = t.ToString(),
                    Value = t.Id.ToString(),
                    Selected = t.Id.ToString() == id
                }).ToList();
            if (@default != null)
                list.Insert(0, new SelectListItem()
                {
                    Selected = id == "0",
                    Value = "0",
                    Text = @default
                });
            return View("_DropDown", Tuple.Create<IEnumerable<SelectListItem>, string>(list, nombre));
        }

        public ActionResult GetDorpDownLineaProductop(string id, long idCiiu = 0, string nombre = "IdLineaProducto", string @default = null)
        {
            var list = Manager.LineaProducto.Get(t => t.Activado && t.LineasProductoUnidadMedida.Any()
                && t.MateriasPropia.All(h => h.VolumenProduccion.Encuesta.Id != Model.Id)
                /*&& t.Ciiu.Establecimientos.Any(h=>h.Id==IdEstablecimiento*/
                && t.IdCiiu == idCiiu && t.LineasProductoEstablecimiento.All(h => h.IdEstablecimiento != IdEstablecimiento))
                .Select(t => new SelectListItem()
                {
                    Text = t.ToString(),
                    Value = t.Id.ToString(),
                    Selected = t.Id.ToString() == id
                }).ToList();
            if (@default != null)
                list.Insert(0, new SelectListItem()
                {
                    Selected = id == "0",
                    Value = "0",
                    Text = @default
                });
            return View("_DropDown", Tuple.Create<IEnumerable<SelectListItem>, string>(list, nombre));
        }

        public ActionResult GetDorpDownCiiu(string id = "0", string nombre = "IdCiiu", string @default = null)
        {
            var list = Manager.Ciiu.Get(t => t.Activado
                && t.Establecimientos.Any(h => h.Id == IdEstablecimiento))
                .Select(t => new SelectListItem()
                {
                    Text = t.ToString(),
                    Value = t.Id.ToString(),
                    Selected = t.Id.ToString() == id
                }).ToList();
            if (@default != null)
                list.Insert(0, new SelectListItem()
                {
                    Selected = id == "0",
                    Value = "0",
                    Text = @default
                });
            return View("_DropDown", Tuple.Create<IEnumerable<SelectListItem>, string>(list, nombre));
        }

        public ActionResult GetDorpDownAllCiiu(string id = "0", string nombre = "IdCiiu", string @default = null)
        {
            var list = Manager.Ciiu.Get(t => t.Activado
                /* && t.Establecimientos.Any(h => h.Id == IdEstablecimiento)*/)
                .Select(t => new SelectListItem()
                {
                    Text = t.ToString(),
                    Value = t.Id.ToString(),
                    Selected = t.Id.ToString() == id
                }).ToList();
            if (@default != null)
                list.Insert(0, new SelectListItem()
                {
                    Selected = id == "0",
                    Value = "0",
                    Text = @default
                });
            return View("_DropDown", Tuple.Create<IEnumerable<SelectListItem>, string>(list, nombre));
        }

        public ActionResult NuevaMateriaTerceros(long id = 0)
        {
            var mat = Manager.MateriaTercerosManager.Find(id);
            mat = mat ?? new MateriaTerceros();
            return PartialView("_NuevaMateriaTerceros", mat);
        }

        public ActionResult NuevaMateriaPropia(long id = 0)
        {
            var mat = Manager.MateriaPropiaManager.Find(id);
            mat = mat ?? new MateriaPropia();
            return PartialView("_NuevaMateriaPropia", mat);
        }

        [HttpPost]
        public JsonResult NuevaMateriaTerceros(MateriaTerceros materia)
        {
            materia.IdVolumenProduccion = Model.VolumenProduccionMensual.Identificador;
            var manager = Manager;
            var valid = !Manager.MateriaTercerosManager.ValidarProduccion(Model.Id, materia.IdLineaProducto,
                   decimal.Parse(materia.UnidadProduccion));
            if (valid && (string.IsNullOrEmpty(materia.justificacion_produccion) || string.IsNullOrWhiteSpace(materia.justificacion_produccion)))
            {
                var res = new
                {
                    Success = false,
                    Invalid = true,
                    Errors = new List<string>() { "Debe justificar el valor de producción" }
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            var op = materia.Id == 0
                ? manager.MateriaTercerosManager.Add(materia)
                : manager.MateriaTercerosManager.Modify(materia);
            if (op.Success)
            {
                manager.MateriaTercerosManager.SaveChanges();
                manager.ValorProduccionManager.Generate(Model.Id);
                var model = Manager.EncuestaEstadistica.FindById(Model.Id);
                var list = model.VolumenProduccionMensual.MateriasTercero;
                var c = this.RenderRazorViewToString("_TableProductoMateriaPrimaTerceros", list);
                var d = this.RenderRazorViewToString("_ValorProduccionEstablecimiento", model);
                var e = this.RenderRazorViewToString("_ValorVentasProductosEstablecimiento", model);

                var res = new
                {
                    Success = true,
                    Data = c,
                    ValorProduccion = d,
                    Ventas = e,
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var res = new
                {
                    Success = false,
                    Errors = op.Errors
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult NuevaMateriaPropia(MateriaPropia materia)
        {
            //adicionar linea de producto al establecimiento
            materia.IdVolumenProduccion = Model.VolumenProduccionMensual.Identificador;
            var manager = Manager;
            var op = materia.Id == 0
                ? manager.MateriaPropiaManager.Add(materia)
                : manager.MateriaPropiaManager.Modify(materia);
            var lin = Manager.LineaProducto.Find(materia.IdLineaProducto);
            if (op.Success && lin != null)
            {
                manager.EncuestaEstadistica.AddLineaProducto(Model.Id, new LineaProducto() { IdCiiu = lin.IdCiiu, Id = materia.IdLineaProducto }, false);
                manager.MateriaPropiaManager.SaveChanges();
                manager.ValorProduccionManager.Generate(Model.Id);
                var model = Manager.EncuestaEstadistica.FindById(Model.Id);
                var list = model.VolumenProduccionMensual.MateriasPropia;
                var c = this.RenderRazorViewToString("_TableProductoMateriaPrimaPropia", list);
                var d = this.RenderRazorViewToString("_ValorProduccionEstablecimiento", model);
                var e = this.RenderRazorViewToString("_ValorVentasProductosEstablecimiento", model);
                var res = new
                {
                    Success = true,
                    Data = c,
                    ValorProduccion = d,
                    Ventas = e
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var res = new
                {
                    Success = false,
                    Errors = op.Errors
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AsignarNuevaLineaProducto(LineaProducto linea)
        {
            var manager = Manager;
            manager.EncuestaEstadistica.AddLineaProducto(Model.Id, linea);
            var op = new List<string>();
            if (linea == null)
                op.Add("Datos Erroneos");
            if (linea != null && linea.Id < 1)
                op.Add("Debe seleccionar una Línea de Producto");
            if (linea != null && linea.IdCiiu < 1)
                op.Add("Debe seleccionar un CIIU");
            if (!op.Any())
            {
                manager.MateriaPropiaManager.SaveChanges();
                manager.ValorProduccionManager.Generate(Model.Id);
                var model = Manager.EncuestaEstadistica.FindById(Model.Id);
                var list = model.VolumenProduccionMensual.MateriasPropia;
                var c = this.RenderRazorViewToString("_TableProductoMateriaPrimaPropia", list);
                var d = this.RenderRazorViewToString("_ValorProduccionEstablecimiento", model);
                var e = this.RenderRazorViewToString("_ValorVentasProductosEstablecimiento", model);
                var res = new
                {
                    Success = true,
                    Data = c,
                    ValorProduccion = d,
                    Ventas = e
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var res = new
                {
                    Success = false,
                    Errors = op
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult EliminarMateriaTerceros(long id)
        {
            var manager = Manager;
            var op = manager.MateriaTercerosManager.Delete(id);
            if (op.Success)
            {
                manager.MateriaTercerosManager.SaveChanges();
                manager.ValorProduccionManager.Generate(Model.Id);
                var model = Manager.EncuestaEstadistica.FindById(Model.Id);
                var list = model.VolumenProduccionMensual.MateriasTercero;
                var c = this.RenderRazorViewToString("_TableProductoMateriaPrimaTerceros", list);
                var d = this.RenderRazorViewToString("_ValorProduccionEstablecimiento", model);
                var e = this.RenderRazorViewToString("_ValorVentasProductosEstablecimiento", model);
                var res = new
                {
                    Success = true,
                    Data = c,
                    ValorProduccion = d,
                    Ventas = e
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var res = new
                {
                    Success = false,
                    Errors = op.Errors
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetDorpDownUnidadMedida(long idLineaProducto, string id, string nombre = "IdUnidadMedida", string @default = null)
        {
            var list = Manager.UnidadMedida.Get(t =>
                t.LineasProductoUnidadMedida.Any(h => h.id_linea_producto == idLineaProducto)).Select(t => new SelectListItem()
                {
                    Text = t.ToString(),
                    Value = t.Id.ToString(),
                    Selected = t.Id.ToString() == id
                }).ToList();
            if (@default != null)
                list.Insert(0, new SelectListItem()
                {
                    Selected = id == "0",
                    Value = "0",
                    Text = @default
                });
            return View("_DropDown", Tuple.Create<IEnumerable<SelectListItem>, string>(list, nombre));
        }

        public ActionResult FillFactoresIncremento()
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(Model.Id);
            if (encuesta == null) return HttpNotFound("Encuesta no encontrada");
            var factores = encuesta.FactorProduccion.CAT_FACTOR1.Select(t => t.Id);
            encuesta.FactorProduccion.IncrementoB = true;
            manager.EncuestaEstadistica.Modify(encuesta);
            manager.EncuestaEstadistica.SaveChanges();
            var allFactores = Manager.Factor.Get(t => t.Activado);
            var incremento = new List<SelectListItem>();
            var enumerable = factores as IList<long> ?? factores.ToList();
            foreach (var f in allFactores.Where(t => t.TipoFactor == TipoFactor.Incremento))
            {
                incremento.Add(new SelectListItem()
                {
                    Text = f.Nombre,
                    Value = f.Id.ToString(),
                    Selected = enumerable.Any(t => t == f.Id)
                });
            }
            return PartialView("_FactoresIncremento", incremento);
        }
        public ActionResult FillFactoresIncrementoReadOnly()
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(Model.Id);
            if (encuesta == null) return HttpNotFound("Encuesta no encontrada");
            var factores = encuesta.FactorProduccion.CAT_FACTOR1.Select(t => t.Id);
            encuesta.FactorProduccion.IncrementoB = true;
            manager.EncuestaEstadistica.Modify(encuesta);
            manager.EncuestaEstadistica.SaveChanges();
            var allFactores = Manager.Factor.Get(t => t.Activado);
            var incremento = new List<SelectListItem>();
            var enumerable = factores as IList<long> ?? factores.ToList();
            foreach (var f in allFactores.Where(t => t.TipoFactor == TipoFactor.Incremento))
            {
                incremento.Add(new SelectListItem()
                {
                    Text = f.Nombre,
                    Value = f.Id.ToString(),
                    Selected = enumerable.Any(t => t == f.Id)
                });
            }
            return PartialView("_FactoresIncrementoReadOnly", incremento);
        }
        public ActionResult FillFactoresDecrecimiento()
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(Model.Id);
            if (encuesta == null) return HttpNotFound("Encuesta no encontrada");
            var factores = encuesta.FactorProduccion.CAT_FACTOR1.Select(t => t.Id);
            var allFactores = Manager.Factor.Get(t => t.Activado);
            var disminucion = new List<SelectListItem>();
            var enumerable = factores as IList<long> ?? factores.ToList();
            encuesta.FactorProduccion.DecrecimientoB = true;
            manager.EncuestaEstadistica.Modify(encuesta);
            manager.EncuestaEstadistica.SaveChanges();
            foreach (var f in allFactores.Where(t => t.TipoFactor == TipoFactor.Disminución))
            {

                disminucion.Add(new SelectListItem()
                {
                    Text = f.Nombre,
                    Value = f.Id.ToString(),
                    Selected = enumerable.Any(t => t == f.Id)
                });
            }
            return PartialView("_FactoresIncremento", disminucion);
        }

        public ActionResult FillFactoresDecrecimientoReadOnly()
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(Model.Id);
            if (encuesta == null) return HttpNotFound("Encuesta no encontrada");
            var factores = encuesta.FactorProduccion.CAT_FACTOR1.Select(t => t.Id);
            var allFactores = Manager.Factor.Get(t => t.Activado);
            var disminucion = new List<SelectListItem>();
            var enumerable = factores as IList<long> ?? factores.ToList();
            encuesta.FactorProduccion.DecrecimientoB = true;
            manager.EncuestaEstadistica.Modify(encuesta);
            manager.EncuestaEstadistica.SaveChanges();
            foreach (var f in allFactores.Where(t => t.TipoFactor == TipoFactor.Disminución))
            {

                disminucion.Add(new SelectListItem()
                {
                    Text = f.Nombre,
                    Value = f.Id.ToString(),
                    Selected = enumerable.Any(t => t == f.Id)
                });
            }
            return PartialView("_FactoresIncrementoReadOnly", disminucion);
        }

        public void ProduccionNormal()
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(Model.Id);
            if (encuesta == null) return;
            encuesta.FactorProduccion.ProduccionNormalB = true;
            manager.EncuestaEstadistica.Modify(encuesta);
            manager.EncuestaEstadistica.SaveChanges();
        }

        public void SetService(bool value)
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(Model.Id);
            if (encuesta == null) return;
            encuesta.VentasProductosEstablecimiento.ServiciosActivados = value;
            manager.VentasProductosEstablecimientoManager.Modify(encuesta.VentasProductosEstablecimiento);
            manager.VentasProductosEstablecimientoManager.SaveChanges();
        }

        public JsonResult SaveTrabajadoresDiasTrabajados(int? diasTrabajados, int? trabajadoresProduccion,
             int? administrativoPlanta)
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(Model.Id);
            if (encuesta == null)
            {
                var res = new
                {
                    Success = false
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            };
            var errors = new List<string>();
            if (trabajadoresProduccion == null)
            {
                errors.Add("Debe introducir la cantidad de trabajadores en producción");
            }
            else if (trabajadoresProduccion < 1)
            {
                errors.Add("La cantidad de trabajadores en producción debe ser mayor que cero");
            }

            if (administrativoPlanta == null)
            {
                errors.Add("Debe introducir los administrativos en planta");
            }
            else
                if (administrativoPlanta < 1)
                {
                    errors.Add("La cantidad de administrativos en planta debe ser mayor que cero");
                }
            var count = DateTime.DaysInMonth(encuesta.Fecha.Year, encuesta.Fecha.Month);
            if (diasTrabajados == null)
            {
                errors.Add("Debe introducir la cantidad de días trabajados");
            }
            else
                if (diasTrabajados < 1)
                {
                    errors.Add("La cantidad de días trabajados debe ser mayor que cero");
                }
            if (diasTrabajados > count)
            {
                errors.Add("La cantidad de días trabajados debe ser menor que " + count);
            }
            if (errors.Count > 0)
            {
                var res = new
                {
                    Success = false,
                    Errors = errors
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            encuesta.TrabajadoresDiasTrabajados.TrabajadoresProduccion = trabajadoresProduccion;
            encuesta.TrabajadoresDiasTrabajados.DiasTrabajados = diasTrabajados;
            encuesta.TrabajadoresDiasTrabajados.AdministrativosPlanta = administrativoPlanta;
            manager.TrabajadoresDiasTrabajadosManager.Modify(encuesta.TrabajadoresDiasTrabajados);
            manager.TrabajadoresDiasTrabajadosManager.SaveChanges();
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveServicio(List<VentasServicioManufactura> services)
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(Model.Id);
            var errors = new List<string>();
            if (encuesta == null)
            {
                var res = new
                {
                    Success = false,
                    Errors = new List<string>() { "No se pudo encontrar la encuesta" }
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            };
            var pais = new List<long>();
            var ext = new List<long>();
            if (encuesta.VentasProductosEstablecimiento.ServiciosActivados)
            {
                var any = false;
                foreach (var it in services)
                {
                    if (it.venta != null)
                    {
                        any = true;
                        break;
                    }
                }
                if (!any)
                    errors.Add("Usted señaló que el establecimiento brindó servicios a terceros, debe llenar al menos uno.");
            }
            foreach (var se in services)
            {
                var servicio =
               manager.VentaServicioManufacturaManager.Find(se.Id);
                if (servicio != null)
                {

                    servicio.venta_extranjero = se.venta_extranjero;
                    servicio.venta = se.venta;
                    servicio.justificacion_venta_ext = se.justificacion_venta_ext;
                    servicio.justificacion_venta_pais = se.justificacion_venta_pais;
                    manager.VentaServicioManufacturaManager.Modify(servicio);
                    manager.VentaServicioManufacturaManager.SaveChanges();

                    var validPais = !manager.VentaServicioManufacturaManager.ValidarVentaPais(se.Id, se.venta);
                    var validExt = !manager.VentaServicioManufacturaManager.ValidarVentaExtranjero(se.Id, se.venta_extranjero);

                    if (validExt && (string.IsNullOrEmpty(se.justificacion_venta_ext) || string.IsNullOrWhiteSpace(se.justificacion_venta_ext)))
                        ext.Add(se.Id);
                    if (validPais && (string.IsNullOrEmpty(se.justificacion_venta_pais) || string.IsNullOrWhiteSpace(se.justificacion_venta_pais)))
                        pais.Add(se.Id);
                }
            }
            if (pais.Count > 0 || ext.Count > 0)
            {
                var te = new List<string>()
                {
                    "Debe justificar los valores de ventas en el pais y ventas en el extranjero"
                };
                te.AddRange(errors);
                var re = new
                {
                    Success = false,
                    Errors = te,
                    Pais = pais,
                    Extranjero = ext
                };
                return Json(re, JsonRequestBehavior.AllowGet);
            }
            if (errors.Any())
            {
                var re = new
                {
                    Success = false,
                    Errors = errors,
                    Pais = new List<long>(),
                    Extranjero = new List<long>()
                };
                return Json(re, JsonRequestBehavior.AllowGet);
            }
            var resp = new
            {
                Success = true,
            };
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveInformante(EncuestaEstadistica encuesta)
        {
            var list = Manager.EncuestaEstadistica.SalvarContacto(encuesta);

            if (list.Count == 0)
            {
                var res = new
                {
                    Success = true,

                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            var resp = new
            {
                Success = false,
                Errors = list
            };
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        public void SetFactorProduccion(long id, bool put)
        {
            var manager = Manager;
            var factor = manager.Factor.Find(id);
            if (factor == null)
                return;
            var encuesta = manager.EncuestaEstadistica.FindById(Model.Id);
            if (encuesta == null)
                return;
            if (put)
            {
                encuesta.FactorProduccion.CAT_FACTOR1.Add(factor);
                manager.EncuestaEstadistica.SaveChanges();
            }
            else
            {
                var fr = encuesta.FactorProduccion.CAT_FACTOR1.FirstOrDefault(t => t.Id == id);
                if (fr != null)
                {
                    encuesta.FactorProduccion.CAT_FACTOR1.Remove(fr);
                    manager.FactorProducccionManager.SaveChanges();
                }
            }

        }

        public ActionResult GetHistoricoProduccion(long id)
        {
            var historico = Manager.MateriaPropiaManager.GetHistoryProduccion(id);
            return PartialView("_TableNumbers", historico);
        }
        public ActionResult GetHistoricoValorUnitario(long id)
        {
            var historico = Manager.MateriaPropiaManager.GetHistoryValorUnitario(id);
            return PartialView("_TableNumbers", historico);
        }
        public ActionResult GetHistoricoVentasPais(long id)
        {
            var historico = Manager.MateriaPropiaManager.GetHistoryVentasPais(id);
            return PartialView("_TableNumbers", historico);
        }
        public ActionResult GetHistoricoVentasExtranjero(long id)
        {
            var historico = Manager.MateriaPropiaManager.GetHistoryVentasExtranjero(id);
            return PartialView("_TableNumbers", historico);
        }
        public ActionResult GetHistoricoProduccionTerceros(long id)
        {
            var historico = Manager.MateriaTercerosManager.GetHistoryProduccion(id);
            return PartialView("_TableNumbers", historico);
        }
        public ActionResult GetHistoricoMateriaPrimaTerceros(long id)
        {
            var historico = Manager.ValorProduccionManager.GetHistoryMateriaPrimaTerceros(id);
            return PartialView("_TableNumbers", historico);
        }
        public ActionResult GetHistoricoValorVentasVentasPais(long id)
        {
            var historico = Manager.VentasPaisExtranjeroManager.GetHistoryVentasPais(id);
            return PartialView("_TableNumbers", historico);
        }
        public ActionResult GetHistoricoValorVentasVentasExtranjero(long id)
        {
            var historico = Manager.VentasPaisExtranjeroManager.GetHistoryVentasExtranjero(id);
            return PartialView("_TableNumbers", historico);
        }

        public ActionResult GetHistoricoValorServicioVentasPais(long id)
        {
            var historico = Manager.VentaServicioManufacturaManager.GetHistoryVentasPais(id);
            return PartialView("_TableNumbers", historico);
        }
        public ActionResult GetHistoricoValorServicioVentasExtranjero(long id)
        {
            var historico = Manager.VentaServicioManufacturaManager.GetHistoryVentasExtranjero(id);
            return PartialView("_TableNumbers", historico);
        }
    }
}
