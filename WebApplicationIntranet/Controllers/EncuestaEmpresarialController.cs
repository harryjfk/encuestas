using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Domain.Managers;
using Entity;
using PagedList;
using Seguridad.PRODUCE;

namespace WebApplication.Controllers
{
    //[Authorize]
    //[Autorizacion]
    public class EncuestaEmpresarialController : BaseController<EncuestaEmpresarial>
    {
        private static long IdEstablecimiento { get; set; }
        public ActionResult GetDorpDown(string id, string nombre = "IdEncuestaEmpresarial", string @default = null)
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
            int userId;
            if (Session["uid"] == null)
            {
                return HttpNotFound("Usuario no encontrado");
            }
            if (int.TryParse(Session["uid"].ToString(), out userId) == false)
            {
                return HttpNotFound("Usuario no encontrado");
            }

            //var user = this.GetLogued();
            var user = Manager.Usuario.FindUsuarioIntranet(userId);
            if (user == null) return HttpNotFound("Usuario no encontrado");
            
            var manager = Manager;
            IdEstablecimiento = id;
            var establecimiento = Manager.Establecimiento.Find(id);
            if (establecimiento == null)
                return HttpNotFound("Establecimiento no encontrado");
            ViewBag.Establecimiento = establecimiento;
            manager.EncuestaEmpresarial.GenerateCurrent(IdEstablecimiento);
            Query = Query ?? new Query<EncuestaEmpresarial>();
            Query = Query.Validate();
            Query.Criteria = Query.Criteria ?? new EncuestaEmpresarial();
            Query.Criteria.IdEstablecimiento = IdEstablecimiento;
            Query.Criteria.IdInformante = user.Identificador;
            Query.Paginacion = Query.Paginacion ?? new Paginacion();
            Query.Paginacion.Page = 1;
            Query.BuildFilter();
            Manager.EncuestaEmpresarial.GetAsignadosInformante(Query);
            ModelState.Clear();
            return View("Index", Query);
        }
        public ActionResult EncuestasAnalista(long id)
        {
            int userId;
            if (Session["uid"] == null)
            {
                return HttpNotFound("Usuario no encontrado");
            }
            if (int.TryParse(Session["uid"].ToString(), out userId) == false)
            {
                return HttpNotFound("Usuario no encontrado");
            }

            //var user = this.GetLogued();
            var user = Manager.Usuario.FindUsuarioIntranet(userId);
            if (user == null) return HttpNotFound("Usuario no encontrado");

            var manager = Manager;
            IdEstablecimiento = id;
            var establecimiento = Manager.Establecimiento.Find(id);
            if (establecimiento == null)
                return HttpNotFound("Establecimiento no encontrado");
            ViewBag.Establecimiento = establecimiento;
            //manager.EncuestaEmpresarial.GenerateCurrent(IdEstablecimiento);
            Query = Query ?? new Query<EncuestaEmpresarial>();
            Query = Query.Validate();
            Query.Criteria = Query.Criteria ?? new EncuestaEmpresarial();
            Query.Criteria.IdEstablecimiento = IdEstablecimiento;
            //Query.Criteria.IdAnalista = user.Identificador;
            Query.Paginacion = Query.Paginacion ?? new Paginacion();
            Query.Paginacion.Page = 1;
            Query.BuildFilter();
            Manager.EncuestaEmpresarial.GetAsignadosAnalista(Query,userId);
            ModelState.Clear();
            return View("IndexAnalista", Query);
        }

        public ActionResult Encuesta(long idEncuesta = 0)
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEmpresarial.Find(idEncuesta);
            if (encuesta == null) return HttpNotFound("Encuesta No encontrada");
            if (encuesta.EstadoEncuesta == EstadoEncuesta.Observada || encuesta.EstadoEncuesta == EstadoEncuesta.NoEnviada)
                return View("Encuesta", encuesta);
            return View("VerEncuesta", encuesta);
        }

        public ActionResult EncuestaAnalista(long idEncuesta = 0)
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEmpresarial.Find(idEncuesta);
            if (encuesta == null) return HttpNotFound("Encuesta No encontrada");
            if (encuesta.EstadoEncuesta == EstadoEncuesta.Enviada)
                return View("EncuestaAnalista", encuesta);
            return View("VerEncuesta", encuesta);
        }

        public ActionResult VerEncuesta(long idEncuesta = 0)
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEmpresarial.Find(idEncuesta);
            if (encuesta == null) return HttpNotFound("Encuestano encontrda");
            if (encuesta.EstadoEncuesta == EstadoEncuesta.NoEnviada)
            {
                encuesta.EstadoEncuesta = EstadoEncuesta.Observada;
                manager.EncuestaEmpresarial.Modify(encuesta);
                manager.EncuestaEmpresarial.SaveChanges();
            }
            return View("VerEncuesta", encuesta);

        }

        [HttpPost]
        public JsonResult Enviar(EncuestaEmpresarial encuesta)
        {
            var manager = Manager;
            var errors = manager.EncuestaEmpresarial.ValidarEncuesta(encuesta);
            if (errors.Count > 0)
            {
                ModelState.AddModelError("Error", "cc");
                var data = RenderRazorViewToString("Encuesta",  encuesta);
                var res = new
                {
                    Success = false,
                    Data = data,
                    Errors = errors
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            var saved = manager.EncuestaEmpresarial.Enviar(encuesta, true);
            if (ModelState.IsValid && saved != null)
            {
                var data = RenderRazorViewToString("VerEncuesta", saved);
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
                var data = RenderRazorViewToString("Encuesta", encuesta);
                var res = new
                {
                    Success = false,
                    Data = data,
                    Errors = new List<string>() { "Error al enviar la Encuesta" }
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public JsonResult Guardar(EncuestaEmpresarial encuesta)
        {
            var manager = Manager;
            var saved = manager.EncuestaEmpresarial.Enviar(encuesta, false);
            if (ModelState.IsValid && saved != null)
            {
                var data = RenderRazorViewToString("Encuesta", manager.EncuestaEmpresarial.Find(encuesta.Id));
                var res = new
                {
                    Success = true,
                    Data = data,
                    Errors = new List<string>() { }
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ModelState.AddModelError("Error", "Error al enviar la Encuesta");
                var data = RenderRazorViewToString("Encuesta", manager.EncuestaEmpresarial.Find(encuesta.Id));
                var res = new
                {
                    Success = false,
                    Data = data,
                    Errors = new List<string>() { "Error al enviar la Encuesta" }
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult Observar(long id, string observacion)
        {
            Manager.EncuestaEmpresarial.Observar(id, observacion);
            return RedirectToAction("EncuestasAnalista", new { id = IdEstablecimiento });
        }

        public ActionResult Validar(long id)
        {
            Manager.EncuestaEmpresarial.Validar(id);
            return RedirectToAction("EncuestasAnalista", new { id = IdEstablecimiento });
        }
        //        public JsonResult Toggle(long id)
        //        {
        //            var manager = OwnManager;
        //            var element = manager.Find(id);
        //            if (element != null)
        //            {
        //                element.Activado = !element.Activado;
        //                manager.Modify(element);
        //                manager.SaveChanges();
        //            }
        //            OwnManager.Get(Query);
        //            var c = RenderRazorViewToString("_Table", Query);
        //            var result = new
        //            {
        //                Success = true,
        //                Data = c
        //            };
        //            return Json(result, JsonRequestBehavior.AllowGet);
        //        }
        [HttpPost]
        public ActionResult BuscarEncuestaInformante(EncuestaEmpresarial criteria)
        {
            Query = Query ?? new Query<EncuestaEmpresarial>().Validate();
            Query.Criteria = criteria;
            Query.Paginacion = Query.Paginacion ?? new Paginacion();
            Query.Paginacion.Page = 1;
            Query.Criteria.IdEstablecimiento = IdEstablecimiento;
            //Query.BuildFilter();
            return RedirectToAction("Encuestas", new { id = IdEstablecimiento });
        }
        [HttpPost]
        public ActionResult BuscarEncuestaAnalista(EncuestaEmpresarial criteria)
        {
            Query = Query ?? new Query<EncuestaEmpresarial>().Validate();
            Query.Criteria = criteria;
            Query.Paginacion = Query.Paginacion ?? new Paginacion();
            Query.Paginacion.Page = 1;
            Query.Criteria.IdEstablecimiento = IdEstablecimiento;
            //Query.BuildFilter();
            return RedirectToAction("EncuestasAnalista", new { id = IdEstablecimiento });
        }
    }
}
