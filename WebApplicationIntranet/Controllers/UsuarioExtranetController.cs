using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Domain.Managers;
using Entity;
using Seguridad.PRODUCE;

namespace WebApplication.Controllers
{
    //[Authorize]
    //[Autorizacion]
    public class UsuarioExtranetController : Controller
    {
        public UsuarioExtranet UsuarioActual
        {
            get
            {
                return new UsuarioExtranet();
                //var user = this.GetLogued();
                //return user != null ? Manager.Usuario.FindUsuarioExtranet((int)user.Identificador) : null;
            }
        }
        public static Query<UsuarioExtranet> QueryExtranet { get; set; }
        public static Query<Establecimiento> QueryEstablecimientosAsignados { get; set; }
        public static Query<Establecimiento> QueryEstablecimientosNoAsignados { get; set; }
        public static UsuarioExtranet Informante { get; set; }
        public static Establecimiento Establecimiento { get; set; }

        public virtual Manager Manager
        {
            get
            {
                return Tools.GetManager();
            }
        }
        public ActionResult GetDorpDown(string id, string nombre = "IdUsuarioExtranet", string @default = null)
        {
            var list = Manager.Usuario.GetUsuariosExtranet().Select(t => new SelectListItem()
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
        public virtual ActionResult IndexInformante(long id = 0)
        {
            var establecimiento = Manager.Establecimiento.Find(id);
            if (establecimiento == null) return HttpNotFound("Establecimiento no encontrado");
            Establecimiento = establecimiento;
            ViewBag.Establecimiento = Establecimiento.Nombre;
            QueryExtranet = QueryExtranet ?? new Query<UsuarioExtranet>();
            QueryExtranet = QueryExtranet.Validate();
            QueryExtranet.Criteria = QueryExtranet.Criteria ?? new UsuarioExtranet();
            //Query.Order = new Order<Ciiu>() { Func = t => t.Nombre };
            Manager.Usuario.GetUsuariosExtranetContacto(QueryExtranet, id);
            ModelState.Clear();
            return View("IndexInformante", QueryExtranet);
        }

        [HttpPost]
        public virtual ActionResult BuscarInformante(UsuarioExtranet criteria)
        {
            if (Establecimiento == null)
                return HttpNotFound("Establecimiento no encontrado");
            QueryExtranet = QueryExtranet ?? new Query<UsuarioExtranet>().Validate();
            QueryExtranet.Criteria = criteria;
            QueryExtranet.Paginacion = QueryExtranet.Paginacion ?? new Paginacion();
            QueryExtranet.Paginacion.Page = 1;
            QueryExtranet.BuildFilter();
            return RedirectToAction("IndexInformante", new { id = Establecimiento.Id });
        }
        public virtual ActionResult PageInformante(int page)
        {
            if (Establecimiento == null)
                return HttpNotFound("Establecimiento no encontrado");
            QueryExtranet = QueryExtranet ?? new Query<UsuarioExtranet>().Validate();
            QueryExtranet.Paginacion.Page = page;
            return RedirectToAction("IndexInformante",new{id=Establecimiento.Id});
        }

        public ActionResult Establecimientos(int id)
        {
            var user = Manager.Usuario.FindUsuarioExtranet(id);
            if (user == null) return HttpNotFound("Informante no encontrado");
            Informante = user;
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<Establecimiento>();
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
            QueryEstablecimientosAsignados.Criteria = QueryEstablecimientosAsignados.Criteria ?? new Establecimiento();
            QueryEstablecimientosAsignados.Criteria.IdInformante = id;
            QueryEstablecimientosAsignados.BuildFilter();
            QueryEstablecimientosAsignados.Paginacion.ItemsPerPage = 10;
            Manager.Establecimiento.Get(QueryEstablecimientosAsignados);
            return View("Establecimientos", QueryEstablecimientosAsignados);
        }
        [HttpPost]
        public virtual ActionResult BuscarEstablecimientosAsignados(Establecimiento criteria)
        {
            if (Informante == null) return HttpNotFound("Informante no encontrado");
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<Establecimiento>();
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
            QueryEstablecimientosAsignados.Criteria = criteria;
            QueryEstablecimientosAsignados.Paginacion = QueryEstablecimientosAsignados.Paginacion ?? new Paginacion();
            QueryEstablecimientosAsignados.Paginacion.Page = 1;
            QueryEstablecimientosAsignados.BuildFilter();
            return RedirectToAction("Establecimientos", new { id = Informante.Identificador });
        }
        public ActionResult PageEstablecimientosAsignados(int page)
        {
            if (Informante == null) return HttpNotFound("Informante no encontrado");
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<Establecimiento>();
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
            QueryEstablecimientosAsignados.Paginacion.Page = page;
            return RedirectToAction("Establecimientos", new { id = Informante.Identificador });
        }
        public ActionResult EstablecimientosNoAsignados()
        {
            var user = Informante;
            if (user == null) return HttpNotFound("Informante no encontrado");
            QueryEstablecimientosNoAsignados = QueryEstablecimientosNoAsignados ?? new Query<Establecimiento>();
            QueryEstablecimientosNoAsignados = QueryEstablecimientosNoAsignados.Validate();
            QueryEstablecimientosNoAsignados.BuildFilter();
            QueryEstablecimientosNoAsignados.Paginacion.ItemsPerPage = 10;
            Manager.Establecimiento.GetNoAsignadosInformantes(QueryEstablecimientosNoAsignados);
            return PartialView("EstablecimientosNoAsignados", QueryEstablecimientosNoAsignados);
        }
        public JsonResult AsignarEstablecimiento(long id)
        {
            if (Informante == null)
                return Json(new { Success = false, Errors = new List<string>() { "Informante no encontrado" } }, JsonRequestBehavior.AllowGet);
            Manager.Usuario.AsignarEstablecimientoInformante((int)Informante.Identificador, id);

            QueryEstablecimientosNoAsignados = QueryEstablecimientosNoAsignados ?? new Query<Establecimiento>();
            QueryEstablecimientosNoAsignados = QueryEstablecimientosNoAsignados.Validate();
            QueryEstablecimientosNoAsignados.BuildFilter();
            Manager.Establecimiento.GetNoAsignadosInformantes(QueryEstablecimientosNoAsignados);
            var noAsignados = this.RenderRazorViewToString("EstablecimientosNoAsignados", QueryEstablecimientosNoAsignados);

            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<Establecimiento>();
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
            QueryEstablecimientosAsignados.Criteria = QueryEstablecimientosAsignados.Criteria ?? new Establecimiento();
            QueryEstablecimientosAsignados.Criteria.IdInformante = Informante.Identificador;
            QueryEstablecimientosAsignados.BuildFilter();
            Manager.Establecimiento.Get(QueryEstablecimientosAsignados);
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
            if (Informante == null)
                return Json(new { Success = false, Errors = new List<string>() { "Informante no encontrado" } }, JsonRequestBehavior.AllowGet);
            Manager.Usuario.EliminarEstablecimientoInformante(id);
            return RedirectToAction("Establecimientos", new { id = Informante.Identificador });

        }
        public ActionResult PageEstablecimientosNoAsignados(int page)
        {
            QueryEstablecimientosNoAsignados = QueryEstablecimientosNoAsignados ?? new Query<Establecimiento>();
            QueryEstablecimientosNoAsignados = QueryEstablecimientosNoAsignados.Validate();
            QueryEstablecimientosNoAsignados.Paginacion.Page = page;
            return RedirectToAction("EstablecimientosNoAsignados");
        }
        public ActionResult AsignarContactoExterno(int idUsuario)
        {
            if (Establecimiento == null)
                return HttpNotFound("Establecimiento no encontrado");
            Manager.Contacto.ToggleFromUser(idUsuario,Establecimiento.Id);
            var id = Establecimiento.Id;
            QueryExtranet = QueryExtranet ?? new Query<UsuarioExtranet>();
            QueryExtranet = QueryExtranet.Validate();
            //Query.Order = new Order<Ciiu>() { Func = t => t.Nombre };
            Manager.Usuario.GetUsuariosExtranet(QueryExtranet, id);
            ModelState.Clear();
            return PartialView("_TableInformante", QueryExtranet);
        }
        public ActionResult EstablecimientosEncuestaEmpresarial(UserInformation user)
        {
            Informante = UsuarioActual;
            //Informante = Manager.Usuario.FindUsuarioExtranet(user.Id);
            if (Informante == null) return this.HttpNotFound("No se pudo encontrar el informante");
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<Establecimiento>();
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
            QueryEstablecimientosAsignados.Criteria = QueryEstablecimientosAsignados.Criteria ?? new Establecimiento();
            QueryEstablecimientosAsignados.Criteria.IdInformante = Informante.Identificador;
            QueryEstablecimientosAsignados.BuildFilter();
            QueryEstablecimientosAsignados.Paginacion.ItemsPerPage = 10;
            Manager.Establecimiento.Get(QueryEstablecimientosAsignados);
            if (QueryEstablecimientosAsignados.Elements.Count == 1&&QueryEstablecimientosAsignados.Filter==null)
            {
                var establecimiento = QueryEstablecimientosAsignados.Elements.FirstOrDefault();
                return RedirectToAction("Encuestas","EncuestaEmpresarial",new{id=establecimiento.Id});
            }
            return View("EstablecimientosEncuestaEmpresarial", QueryEstablecimientosAsignados);
        }
        [HttpPost]
        public virtual ActionResult BuscarEstablecimientosEncuestaEmpresarial(Establecimiento criteria)
        {
            if (Informante == null) return HttpNotFound("Informante no encontrado");
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<Establecimiento>();
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
            QueryEstablecimientosAsignados.Criteria = criteria;
            QueryEstablecimientosAsignados.Paginacion = QueryEstablecimientosAsignados.Paginacion ?? new Paginacion();
            QueryEstablecimientosAsignados.Paginacion.Page = 1;
            QueryEstablecimientosAsignados.BuildFilter();
            return RedirectToAction("EstablecimientosEncuestaEmpresarial", new { id = Informante.Identificador });
        }
        public ActionResult PageEstablecimientosEncuestaEmpresarial(int page)
        {
            if (Informante == null) return HttpNotFound("Informante no encontrado");
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<Establecimiento>();
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
            QueryEstablecimientosAsignados.Paginacion.Page = page;
            return RedirectToAction("EstablecimientosEncuestaEmpresarial", new { id = Informante.Identificador });
        }

        public ActionResult EstablecimientosEncuestaEstadistica()
        {
            Informante = UsuarioActual;
            if (Informante == null) return this.HttpNotFound("No se pudo encontrar el informante");
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<Establecimiento>();
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
            QueryEstablecimientosAsignados.Criteria = QueryEstablecimientosAsignados.Criteria ?? new Establecimiento();
            QueryEstablecimientosAsignados.Criteria.IdInformante = Informante.Identificador;
            QueryEstablecimientosAsignados.BuildFilter();
            QueryEstablecimientosAsignados.Paginacion.ItemsPerPage = 10;
            Manager.Establecimiento.Get(QueryEstablecimientosAsignados);
            if (QueryEstablecimientosAsignados.Elements.Count == 1 && QueryEstablecimientosAsignados.Filter == null)
            {
                var establecimiento = QueryEstablecimientosAsignados.Elements.FirstOrDefault();
                return RedirectToAction("Encuestas", "EncuestaEstadistica", new { id = establecimiento.Id });
            }
            return View("EstablecimientosEncuestaEstadistica", QueryEstablecimientosAsignados);
        }
        [HttpPost]
        public virtual ActionResult BuscarEstablecimientosEncuestaEstadistica(Establecimiento criteria)
        {
            if (Informante == null) return HttpNotFound("Informante no encontrado");
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<Establecimiento>();
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
            QueryEstablecimientosAsignados.Criteria = criteria;
            QueryEstablecimientosAsignados.Paginacion = QueryEstablecimientosAsignados.Paginacion ?? new Paginacion();
            QueryEstablecimientosAsignados.Paginacion.Page = 1;
            QueryEstablecimientosAsignados.BuildFilter();
            return RedirectToAction("EstablecimientosEncuestaEstadistica", new { id = Informante.Identificador });
        }
        public ActionResult PageEstablecimientosEncuestaEstadistica(int page)
        {
            if (Informante == null) return HttpNotFound("Informante no encontrado");
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<Establecimiento>();
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
            QueryEstablecimientosAsignados.Paginacion.Page = page;
            return RedirectToAction("EstablecimientosEncuestaEstadistica", new { id = Informante.Identificador });
        }
    }
}
