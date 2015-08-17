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
using System.Configuration;
using System.Net.Mail;
using System.Globalization;

namespace WebApplication.Controllers
{
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
            //var user = this.GetLogued();
            var user = Manager.Usuario.FindUsuarioExtranet(10854);
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
            var user = this.GetLogued();
            //var user = Manager.Usuario.FindUsuarioIntranet(4600);
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
            //Query.Criteria.id = user.Identificador;
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

            long iden = encuesta.Id;
            int mp = Model.VolumenProduccionMensual.MateriasPropia.Count();

            if (encuesta.EstadoEncuesta == EstadoEncuesta.Observada || encuesta.EstadoEncuesta == EstadoEncuesta.NoEnviada)
                return View("Encuesta", encuesta);
            return View("VerEncuesta", encuesta);
        }

        public ActionResult EncuestaAnalista(long idEncuesta = 0)
        {
           

            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(idEncuesta);
            if (encuesta == null) return HttpNotFound("Encuesta No encontrada");
            var user = this.GetLogued();
            if (user == null) return View("VerEncuesta", encuesta);
            var analist = encuesta.CAT_ENCUESTA_ANALISTA.FirstOrDefault(t => t.id_analista == user.Identificador);
            if (analist == null || analist.IsWaiting) return HttpNotFound("Encuesta No encontrada");
            if (encuesta.EstadoEncuesta == EstadoEncuesta.Enviada && analist.IsCurrent)
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
            Encuesta encuesta = Manager.Encuesta.Find(id);
            if (encuesta != null)
            {
                Usuario informante = encuesta.Informante;
                if (informante != null)
                {
                    UsuarioExtranet informanteExtranet = Manager.Usuario.FindUsuarioExtranet((int)informante.Identificador);
                    if (informanteExtranet != null)
                    {
                        string correo = informanteExtranet.Email;
                        //string correo = ConfigurationManager.AppSettings["MailRecepcion"];
                        if (!string.IsNullOrEmpty(correo))
                        {
                            try
                            {
                                var appSettings = ConfigurationManager.AppSettings;
                                MailMessage ContenedorEMail = new MailMessage();
                                ContenedorEMail.From = new MailAddress(appSettings["MailDireccion"]);
                                ContenedorEMail.Subject = string.Format(appSettings["Asunto"]);
                                ContenedorEMail.To.Add(correo);
                                //ContenedorEMail.Body = "Su encuesta ha sido observada. "
                                //    + "&lt;br/>&lt;br/>Estadística Industrial Mensual " + encuesta.Fecha.ToString("yyyy", CultureInfo.GetCultureInfo("ES")).ToUpper()
                                //    + "&lt;br/>Información del mes " + encuesta.Fecha.ToString("MMMM", CultureInfo.GetCultureInfo("ES")).ToUpper()
                                //    + "&lt;br/>Enviada el " + encuesta.fecha_ultimo_envio.GetValueOrDefault().ToString("D", CultureInfo.GetCultureInfo("es"))
                                //    + "&lt;br/>Establecimiento: " + encuesta.Establecimiento.Nombre
                                //    + "&lt;br/>Observación: " + observacion;
                                ContenedorEMail.Body = string.Format(appSettings["Body"], encuesta.Fecha.ToString("yyyy", CultureInfo.GetCultureInfo("ES")).ToUpper(),
                                    encuesta.Fecha.ToString("MMMM", CultureInfo.GetCultureInfo("ES")).ToUpper(),
                                    encuesta.fecha_ultimo_envio.GetValueOrDefault().ToString("D", CultureInfo.GetCultureInfo("es")),
                                    encuesta.Establecimiento.Nombre, observacion);
                                //ContenedorEMail.Body = string.Format(appSettings["Body"], "Parametro 1", "Parametro 2", "Parametro 3");
                                ContenedorEMail.IsBodyHtml = true;
                                ContenedorEMail.Priority = MailPriority.High;
                                SmtpClient smtp = new SmtpClient(appSettings["SMTP"], Convert.ToInt32(appSettings["PortSMTP"]));
                                smtp.Credentials = new System.Net.NetworkCredential(appSettings["MailDireccion"], appSettings["MailClave"]);
                                smtp.EnableSsl = Convert.ToBoolean(appSettings["SSL"]);
                                smtp.Send(ContenedorEMail);
                            }
                            catch (Exception)
                            {

                                //throw;
                            }
                        }
                    }
                }
            }
            Manager.EncuestaEstadistica.Observar(id, observacion);
            return RedirectToAction("EncuestasAnalista", new { id = IdEstablecimiento });
        }

        //private bool enviarCorreo(Reserva reserva, Contacto contacto)
        //{
        //    bool success = false;
        //    try
        //    {
        //        var appSettings = ConfigurationManager.AppSettings;
        //        string tipo = string.Empty;
        //        if (reserva != null) tipo = "Reserva";
        //        if (contacto != null) tipo = "Contacto";

        //        // correo para empresa
        //        MailMessage ContenedorEMail = new MailMessage();
        //        ContenedorEMail.From = new MailAddress(appSettings["MailDireccion"]);
        //        ContenedorEMail.Subject = string.Format(appSettings[tipo + "." + "Asunto"]);
        //        //ContenedorEMail.To.Add(correo);
        //        ContenedorEMail.To.Add(appSettings[tipo + "." + "MailRecepcion"]);

        //        if (reserva != null)
        //        {
        //            ContenedorEMail.Body = string.Format(appSettings[tipo + "." + "Body"],
        //            reserva.Nombre, reserva.Apellido, reserva.Telefono, reserva.Email, reserva.Pais, reserva.Ciudad,
        //            reserva.Entrada, reserva.Salida, reserva.Adultos, reserva.Ninos, reserva.Hotel, reserva.Oferta, reserva.Mensaje);
        //        }
        //        else    // contacto != null
        //        {
        //            ContenedorEMail.Body = string.Format(appSettings[tipo + "." + "Body"],
        //            contacto.Nombre, contacto.Empresa, contacto.Telefono, contacto.Email, contacto.Asunto);
        //        }

        //        ContenedorEMail.IsBodyHtml = true;
        //        ContenedorEMail.Priority = MailPriority.High;
        //        SmtpClient smtp = new SmtpClient(appSettings["SMTP"], Convert.ToInt32(appSettings["PortSMTP"]));
        //        smtp.Credentials = new System.Net.NetworkCredential(appSettings["MailDireccion"], appSettings["MailClave"]);
        //        smtp.EnableSsl = Convert.ToBoolean(appSettings["SSL"]);
        //        smtp.Send(ContenedorEMail);
        //        success = true;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }


        //    // correo para usuario
        //    //MailMessage ContenedorEMail_usuario = new MailMessage();
        //    //ContenedorEMail_usuario.From = new MailAddress(appSettings["MailDireccion"]);
        //    //ContenedorEMail_usuario.Subject = "PERUMODA - Gracias por escribirnos";
        //    //ContenedorEMail_usuario.To.Add(correo);
        //    //ContenedorEMail_usuario.Body = string.Format(appSettings["BodyContactenos_usuario"], nombre);
        //    //ContenedorEMail_usuario.IsBodyHtml = true;
        //    //ContenedorEMail_usuario.Priority = MailPriority.High;
        //    //SmtpClient smtp_usuario = new SmtpClient(appSettings["SMTP"], Convert.ToInt32(appSettings["PortSMTP"]));
        //    //smtp_usuario.Credentials = new System.Net.NetworkCredential(appSettings["MailDireccion"], appSettings["MailClave"]);
        //    //smtp_usuario.EnableSsl = Convert.ToBoolean(appSettings["SSL"]);
        //    //smtp_usuario.Send(ContenedorEMail_usuario);
        //    return success;
        //}

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

        public ActionResult ContactosEncuesta()
        {
            var id = IdEstablecimiento;
            var manager = Manager;
            var establecimiento = manager.Establecimiento.Find(id);
            if (establecimiento == null) return HttpNotFound("Establecimiento no encontrado");
            QueryContacto = QueryContacto ?? new Query<Contacto>().Validate();
            QueryContacto.Criteria = QueryContacto.Criteria ?? new Contacto();
            QueryContacto.Criteria.IdEstablecimiento = IdEstablecimiento;
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
            Model = Manager.EncuestaEstadistica.Find(Model.Id);
            return PartialView("_IdentificacionEstablecimiento", Model);
        }

        [HttpPost]
        public JsonResult SaveMateriaPropia(List<MateriaPropia> materias)
        {
            var manager = Manager;
            var produccion = new List<long>();
            var valorUnitario = new List<long>();
            foreach (var materia in materias)
            {
                var mat = manager.MateriaPropiaManager.Find(materia.Id);
                if (mat != null)
                {
                    if (mat.IsFirst)
                    {
                        mat.Existencia = materia.Existencia;
                    }
                    mat.IdUnidadMedida = materia.IdUnidadMedida > 0 ? materia.IdUnidadMedida : mat.IdUnidadMedida;
                    mat.JustificacionProduccion = materia.JustificacionProduccion;
                    mat.JustificacionValorUnitario = materia.JustificacionValorUnitario;
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
                    if (validProduccion && materia.Produccion != null)
                        produccion.Add(materia.Id);
                    if (validValorUnitario && materia.ValorUnitario != null)
                        valorUnitario.Add(materia.Id);

                }
                //else
                //{
                //    var result = new
                //    {
                //        Success = false
                //    };
                //    return Json(result, JsonRequestBehavior.AllowGet);
                //}
            }
            var model = Manager.EncuestaEstadistica.Find(Model.Id);
            manager.ValorProduccionManager.Generate(Model.Id);
            var d = this.RenderRazorViewToString("_ValorProduccionEstablecimiento", model);
            var e = this.RenderRazorViewToString("_ValorVentasProductosEstablecimiento", model);
            var result = new
            {
                Success = true,
                ValorUnitario = valorUnitario,
                Produccion = produccion,
                ValorProduccion = d,
                Ventas = e
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveValorProduccion(List<ValorProduccion> valores)
        {
            var manager = Manager;
            foreach (var materia in valores)
            {
                var valor = manager.ValorProduccionManager.Find(materia.Id);
                if (valor != null)
                {
                    //valor.ProductosMateriaPropia = materia.ProductosMateriaPropia;
                    valor.ProductosMateriaTerceros = materia.ProductosMateriaTerceros;

                    manager.ValorProduccionManager.Modify(valor);
                    manager.ValorProduccionManager.SaveChanges();
                }
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
            foreach (var venta in ventas)
            {
                var valor = manager.VentasPaisExtranjeroManager.Find(venta.Id);
                if (valor != null)
                {
                    valor.VentaExtranjero = venta.VentaExtranjero;
                    valor.VentaPais = venta.VentaPais;

                    manager.VentasPaisExtranjeroManager.Modify(valor);
                    manager.VentasPaisExtranjeroManager.SaveChanges();
                }
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
        public ActionResult NuevaMateriaTerceros(long id = 0)
        {
            var mat = Manager.MateriaTercerosManager.Find(id);
            mat = mat ?? new MateriaTerceros();
            return PartialView("_NuevaMateriaTerceros", mat);
        }
        [HttpPost]
        public JsonResult NuevaMateriaTerceros(MateriaTerceros materia)
        {
            materia.IdVolumenProduccion = Model.VolumenProduccionMensual.Identificador;
            var manager = Manager;
            var op = materia.Id == 0
                ? manager.MateriaTercerosManager.Add(materia)
                : manager.MateriaTercerosManager.Modify(materia);
            if (op.Success)
            {
                manager.MateriaTercerosManager.SaveChanges();
                manager.ValorProduccionManager.Generate(Model.Id);
                var model = Manager.EncuestaEstadistica.Find(Model.Id);
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

        public JsonResult EliminarMateriaTerceros(long id)
        {
            var manager = Manager;
            var op = manager.MateriaTercerosManager.Delete(id);
            if (op.Success)
            {
                manager.MateriaTercerosManager.SaveChanges();
                manager.ValorProduccionManager.Generate(Model.Id);
                var model = Manager.EncuestaEstadistica.Find(Model.Id);
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
            var encuesta = manager.EncuestaEstadistica.Find(Model.Id);
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
            if (Session["idEncuesta"] == null) return HttpNotFound("Encuesta no encontrada");
            //var encuesta = manager.EncuestaEstadistica.Find(Model.Id);
            var encuesta = manager.EncuestaEstadistica.Find((long)Session["idEncuesta"]);
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
            if (Session["idEncuesta"] == null) return HttpNotFound("Encuesta no encontrada");
            //var encuesta = manager.EncuestaEstadistica.Find(Model.Id);
            var encuesta = manager.EncuestaEstadistica.Find((long)Session["idEncuesta"]);
            Session["idEncuesta"] = null;
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
            if (Session["idEncuesta"] == null) return HttpNotFound("Encuesta no encontrada");
            //var encuesta = manager.EncuestaEstadistica.Find(Model.Id);
            var encuesta = manager.EncuestaEstadistica.Find((long)Session["idEncuesta"]);
            Session["idEncuesta"] = null;
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
            var encuesta = manager.EncuestaEstadistica.Find(Model.Id);
            if (encuesta == null) return;
            encuesta.FactorProduccion.ProduccionNormalB = true;
            manager.EncuestaEstadistica.Modify(encuesta);
            manager.EncuestaEstadistica.SaveChanges();
        }

        public void SetService(bool value)
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.Find(Model.Id);
            if (encuesta == null) return;
            encuesta.VentasProductosEstablecimiento.ServiciosActivados = value;
            manager.VentasProductosEstablecimientoManager.Modify(encuesta.VentasProductosEstablecimiento);
            manager.VentasProductosEstablecimientoManager.SaveChanges();
        }

        public JsonResult SaveTrabajadoresDiasTrabajados(int? diasTrabajados, int? trabajadoresProduccion,
            int? administrativoPlanta)
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.Find(Model.Id);
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
            var encuesta = manager.EncuestaEstadistica.Find(Model.Id);
            if (encuesta == null)
            {
                var res = new
                {
                    Success = false,
                    Errors = new List<string>() { "No se pudo encontrar la encuesta" }
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            };
            foreach (var se in services)
            {
                var servicio =
               manager.VentaServicioManufacturaManager.Find(se.Id);
                if (servicio != null)
                {
                    servicio.venta_extranjero = se.venta_extranjero;
                    servicio.venta = se.venta;
                    manager.VentaServicioManufacturaManager.Modify(servicio);
                    manager.VentaServicioManufacturaManager.SaveChanges();
                }
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
            var encuesta = manager.EncuestaEstadistica.Find(Model.Id);
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
