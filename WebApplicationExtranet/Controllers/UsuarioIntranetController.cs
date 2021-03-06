﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Domain.Managers;
using Entity;
using Seguridad.PRODUCE;

namespace WebApplication.Controllers
{
    /*[Authorize]
    [Autorizacion]*/
    public class UsuarioIntranetController : Controller
    {
        public UsuarioIntranet UsuarioActual
        {
            get
            {
                //return new UsuarioIntranet();
                //return Manager.Usuario.FindUsuarioIntranetByLogin(this.User.Identity.Name);
                var user = this.GetLogued();
                return user != null ? Manager.Usuario.FindUsuarioIntranet((int)user.Identificador) : null;
            }
        }
        public static Query<UsuarioIntranet> QueryAnalista { get; set; }
        public static Query<UsuarioIntranet> QueryAdministrador { get; set; }
        public static Query<EstablecimientoAnalista> QueryEstablecimientosAsignados { get; set; }
        public static Query<Establecimiento> QueryEstablecimientosNoAsignados { get; set; }
        public static UsuarioIntranet Analista { get; set; }
        public virtual Manager Manager
        {
            get
            {
                return Tools.GetManager();
            }
        }
        public ActionResult GetDorpDown(string id, string nombre = "IdUsuarioIntranet", string @default = null)
        {
            var list = Manager.Usuario.GetUsuariosIntranet().Select(t => new SelectListItem()
            {
                Text = t.ToString(),
                Value = t.Identificador.ToString(),
                Selected = t.Identificador.ToString() == id
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
        public virtual ActionResult IndexAnalista()
        {
            QueryAnalista = QueryAnalista ?? new Query<UsuarioIntranet>();
            QueryAnalista = QueryAnalista.Validate();
            //Query.Order = new Order<Ciiu>() { Func = t => t.Nombre };
            //Manager.Usuario.GetUsuariosIntranet(QueryAnalista);
            Manager.Usuario.GetUsuariosIntranetAnalista(QueryAnalista);
            ModelState.Clear();
            return View("IndexAnalista", QueryAnalista);
        }
        [HttpPost]
        public virtual ActionResult BuscarAnalista(UsuarioIntranet criteria)
        {
            QueryAnalista = QueryAnalista ?? new Query<UsuarioIntranet>().Validate();
            QueryAnalista.Criteria = criteria;
            QueryAnalista.Paginacion = QueryAnalista.Paginacion ?? new Paginacion();
            QueryAnalista.Paginacion.Page = 1;
            QueryAnalista.BuildFilter();
            return RedirectToAction("IndexAnalista");
        }
        public virtual ActionResult PageAnalista(int page)
        {
            QueryAnalista = QueryAnalista ?? new Query<UsuarioIntranet>().Validate();
            QueryAnalista.Paginacion.Page = page;
            return RedirectToAction("IndexAnalista");
        }
        public virtual ActionResult IndexAdministrador()
        {
            QueryAdministrador = QueryAdministrador ?? new Query<UsuarioIntranet>();
            QueryAdministrador = QueryAdministrador.Validate();
            //Query.Order = new Order<Ciiu>() { Func = t => t.Nombre };
            Manager.Usuario.GetUsuariosIntranet(QueryAdministrador);
            //Manager.Usuario.GetUsuariosIntranetAdministrador(QueryAdministrador);
            ModelState.Clear();
            return View("IndexAdministrador", QueryAdministrador);
        }
        [HttpPost]
        public virtual ActionResult BuscarAdministrador(UsuarioIntranet criteria)
        {
            QueryAdministrador = QueryAdministrador ?? new Query<UsuarioIntranet>().Validate();
            QueryAdministrador.Criteria = criteria;
            QueryAdministrador.Paginacion = QueryAdministrador.Paginacion ?? new Paginacion();
            QueryAdministrador.Paginacion.Page = 1;
            QueryAdministrador.BuildFilter();
            return RedirectToAction("IndexAdministrador");
        }
        public virtual ActionResult PageAdministrador(int page)
        {
            QueryAdministrador = QueryAdministrador ?? new Query<UsuarioIntranet>().Validate();
            QueryAdministrador.Paginacion.Page = page;
            return RedirectToAction("IndexAdministrador");
        }
        public JsonResult SetAdministrador(int id)
        {
            var manager = Manager;
            manager.Usuario.MarcarAdministrador(id);
            manager.Usuario.GetUsuariosIntranet(QueryAdministrador);
            var c = this.RenderRazorViewToString("_TableAdministrador", QueryAdministrador);
            var result = new
            {
                Success = true,
                Data = c
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult Establecimientos(int id)
        //{
        //    var user = Manager.Usuario.FindUsuarioIntranet(id);
        //    if (user == null) return HttpNotFound("Analista no encontrado");
        //    Analista = user;
        //    QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<Establecimiento>();
        //    QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
        //    QueryEstablecimientosAsignados.Criteria = QueryEstablecimientosAsignados.Criteria ?? new Establecimiento();
        //    QueryEstablecimientosAsignados.Criteria.IdAnalista = id;
        //    ViewBag.NombreAnalista = user.Trabajador;
        //    QueryEstablecimientosAsignados.BuildFilter();
        //    QueryEstablecimientosAsignados.Paginacion.ItemsPerPage = 10;
        //    Manager.Establecimiento.Get(QueryEstablecimientosAsignados);
        //    return View("Establecimientos", QueryEstablecimientosAsignados);
        //}

        //
        public ActionResult Establecimientos(int id, int idRol)
        {
            var user = Manager.Usuario.FindUsuarioIntranet(id);
            //var user = Manager.Usuario.FindUsuarioIntranet(id, idRol);

            if (user == null) return HttpNotFound("Analista no encontrado");
            Analista = user;
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<EstablecimientoAnalista>();
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
            QueryEstablecimientosAsignados.Criteria = QueryEstablecimientosAsignados.Criteria ?? new EstablecimientoAnalista();
            QueryEstablecimientosAsignados.Criteria.id_analista = id;
            ViewBag.NombreAnalista = user.Trabajador;
            QueryEstablecimientosAsignados.BuildFilter();
            QueryEstablecimientosAsignados.Paginacion.ItemsPerPage = 10;
            Manager.EstablecimientoAnalistaManager.Get(QueryEstablecimientosAsignados);
            return View("Establecimientos", QueryEstablecimientosAsignados);
        }
        //

        [HttpPost]
        public virtual ActionResult BuscarEstablecimientosAsignados(EstablecimientoAnalista criteria)
        {
            if (Analista == null) return HttpNotFound("Analista no encontrado");
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<EstablecimientoAnalista>();
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
            QueryEstablecimientosAsignados.Criteria = criteria;
            QueryEstablecimientosAsignados.Paginacion = QueryEstablecimientosAsignados.Paginacion ?? new Paginacion();
            QueryEstablecimientosAsignados.Paginacion.Page = 1;
            QueryEstablecimientosAsignados.BuildFilter();
            return RedirectToAction("Establecimientos", new { id = Analista.Identificador, idrol = Analista.IdRol });
        }
        public ActionResult PageEstablecimientosAsignados(int page)
        {
            if (Analista == null) return HttpNotFound("Analista no encontrado");
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<EstablecimientoAnalista>();
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
            QueryEstablecimientosAsignados.Paginacion.Page = page;
            return RedirectToAction("Establecimientos", new { id = Analista.Identificador, idrol = Analista.IdRol });
        }
        public ActionResult EstablecimientosNoAsignados()
        {
            var user = Analista;
            if (user == null) return HttpNotFound("Analista no encontrado");
            QueryEstablecimientosNoAsignados = QueryEstablecimientosNoAsignados ?? new Query<Establecimiento>();
            QueryEstablecimientosNoAsignados = QueryEstablecimientosNoAsignados.Validate();
            QueryEstablecimientosNoAsignados.BuildFilter();
            QueryEstablecimientosNoAsignados.Paginacion.ItemsPerPage = 10;
            Manager.Establecimiento.GetNoAsignadosAnalistas(QueryEstablecimientosNoAsignados, (long)user.Identificador);
            return PartialView("EstablecimientosNoAsignados", QueryEstablecimientosNoAsignados);
        }
        public JsonResult AsignarEstablecimiento(long id, long idCiiu, int orden)
        {
            //UsuarioIntranet usuarioIntranet = new UsuarioIntranet();
            //usuarioIntranet = Manager.Usuario.FindUsuarioIntranetByLogin(this.User.Identity.Name.ToString());
            //if (usuarioIntranet == null)
            if (Analista == null)
            {
                return Json(new { Success = false, Errors = new List<string>() { "Analista no encontrado" } }, JsonRequestBehavior.AllowGet);
            }

            //Manager.Usuario.AsignarEstablecimientoAnalista(Analista, id);
            Manager.Usuario.AsignarEstablecimientoAnalista((int)Analista.Identificador, id, idCiiu, orden);

            QueryEstablecimientosNoAsignados = QueryEstablecimientosNoAsignados ?? new Query<Establecimiento>();
            QueryEstablecimientosNoAsignados = QueryEstablecimientosNoAsignados.Validate();
            QueryEstablecimientosNoAsignados.BuildFilter();
            Manager.Establecimiento.GetNoAsignadosAnalistas(QueryEstablecimientosNoAsignados, (int)Analista.Identificador);
            var noAsignados = this.RenderRazorViewToString("EstablecimientosNoAsignados", QueryEstablecimientosNoAsignados);

            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<EstablecimientoAnalista>();
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
            QueryEstablecimientosAsignados.Criteria = QueryEstablecimientosAsignados.Criteria ?? new EstablecimientoAnalista();
            //QueryEstablecimientosAsignados.Criteria.IdAnalista = Analista.Identificador;
            QueryEstablecimientosAsignados.BuildFilter();
            Manager.EstablecimientoAnalistaManager.Get(QueryEstablecimientosAsignados);
            var asignados = this.RenderRazorViewToString("_TableEstablecimientosAsignados", QueryEstablecimientosAsignados);

            var result = new
            {
                Success = true,
                Asignados = asignados,
                NoAsignados = noAsignados
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EliminarEstablecimiento(long id)
        {
            if (Analista == null)
                return Json(new { Success = false, Errors = new List<string>() { "Analista no encontrado" } }, JsonRequestBehavior.AllowGet);
            Manager.Usuario.EliminarEstablecimientoAnalista(id);
            return RedirectToAction("Establecimientos", new { id = Analista.Identificador, idrol = Analista.IdRol });

        }
        public ActionResult PageEstablecimientosNoAsignados(int page)
        {
            QueryEstablecimientosNoAsignados = QueryEstablecimientosNoAsignados ?? new Query<Establecimiento>();
            QueryEstablecimientosNoAsignados = QueryEstablecimientosNoAsignados.Validate();
            QueryEstablecimientosNoAsignados.Paginacion.Page = page;
            return RedirectToAction("EstablecimientosNoAsignados");
        }
        [HttpPost]
        public virtual ActionResult BuscarEstablecimientosNoAsignados(Establecimiento criteria)
        {
            if (Analista == null) return HttpNotFound("Analista no encontrado");
            QueryEstablecimientosNoAsignados = QueryEstablecimientosNoAsignados ?? new Query<Establecimiento>();
            QueryEstablecimientosNoAsignados = QueryEstablecimientosNoAsignados.Validate();
            QueryEstablecimientosNoAsignados.Criteria = criteria;
            QueryEstablecimientosNoAsignados.Paginacion = QueryEstablecimientosNoAsignados.Paginacion ?? new Paginacion();
            QueryEstablecimientosNoAsignados.Paginacion.Page = 1;
            QueryEstablecimientosNoAsignados.BuildFilter();
            return RedirectToAction("EstablecimientosNoAsignados");
        }


        public ActionResult EstablecimientosEncuestaEmpresarial(UserInformation user)
        {
            Analista = UsuarioActual;
            //Analista = Manager.Usuario.FindUsuarioIntranet(user.Id);         
            if (Analista == null) return this.HttpNotFound("No se pudo encontrar el analista");
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<EstablecimientoAnalista>();
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
            QueryEstablecimientosAsignados.Criteria = QueryEstablecimientosAsignados.Criteria ?? new EstablecimientoAnalista();
            //QueryEstablecimientosAsignados.Criteria.IdAnalista = Analista.Identificador;
            QueryEstablecimientosAsignados.BuildFilter();
            QueryEstablecimientosAsignados.Paginacion.ItemsPerPage = 10;
            Manager.EstablecimientoAnalistaManager.Get(QueryEstablecimientosAsignados);
            if (QueryEstablecimientosAsignados.Elements.Count == 1 && QueryEstablecimientosAsignados.Filter == null)
            {
                var establecimiento = QueryEstablecimientosAsignados.Elements.FirstOrDefault();
                return RedirectToAction("Encuestas", "EncuestaEmpresarialAnalista", new { id = establecimiento.id_establecimiento });
            }
            // ViewBag.UserId = user.Id;
            ViewBag.UserId = Analista.Identificador;
            return View("EstablecimientosEncuestaEmpresarial", QueryEstablecimientosAsignados);
        }
        [HttpPost]
        public virtual ActionResult BuscarEstablecimientosEncuestaEmpresarial(EstablecimientoAnalista criteria)
        {
            if (Analista == null) return HttpNotFound("Analista no encontrado");
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<EstablecimientoAnalista>();
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
            QueryEstablecimientosAsignados.Criteria = criteria;
            QueryEstablecimientosAsignados.Paginacion = QueryEstablecimientosAsignados.Paginacion ?? new Paginacion();
            QueryEstablecimientosAsignados.Paginacion.Page = 1;
            QueryEstablecimientosAsignados.BuildFilter();
            return RedirectToAction("EstablecimientosEncuestaEmpresarial", new { id = Analista.Identificador });
        }
        public ActionResult PageEstablecimientosEncuestaEmpresarial(int page)
        {
            if (Analista == null) return HttpNotFound("Analista no encontrado");
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<EstablecimientoAnalista>();
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
            QueryEstablecimientosAsignados.Paginacion.Page = page;
            return RedirectToAction("EstablecimientosEncuestaEmpresarial", new { id = Analista.Identificador });
        }
    }
}
