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
    /*[Authorize]
    [Autorizacion]*/
    public class EncuestaEmpresarialController : BaseController<EncuestaEmpresarial>
    {
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

        public UsuarioIntranet UsuarioActual
        {
            get
            {
                var user = this.GetLogued();
                return user != null ? Manager.Usuario.FindUsuarioIntranet((int)user.Identificador) : null;
            }
        }

        //public ActionResult Encuestas(long id)
        //{
        //    int userId;
        //    if (Session["uid"] == null)
        //    {
        //        return HttpNotFound("Usuario no encontrado");
        //    }
        //    if (int.TryParse(Session["uid"].ToString(), out userId) == false)
        //    {
        //        return HttpNotFound("Usuario no encontrado");
        //    }

        //    //var user = this.GetLogued();
        //    var user = Manager.Usuario.FindUsuarioIntranet(userId);
        //    if (user == null) return HttpNotFound("Usuario no encontrado");

        //    var manager = Manager;
        //    IdEstablecimiento = id;
        //    var establecimiento = Manager.Establecimiento.Find(id);
        //    if (establecimiento == null)
        //        return HttpNotFound("Establecimiento no encontrado");
        //    ViewBag.Establecimiento = establecimiento;
        //    manager.EncuestaEmpresarial.GenerateCurrent(IdEstablecimiento);
        //    Query = Query ?? new Query<EncuestaEmpresarial>();
        //    Query = Query.Validate();
        //    Query.Criteria = Query.Criteria ?? new EncuestaEmpresarial();
        //    Query.Criteria.IdEstablecimiento = IdEstablecimiento;
        //    Query.Criteria.IdInformante = user.Identificador;
        //    Query.Paginacion = Query.Paginacion ?? new Paginacion();
        //    Query.Paginacion.Page = 1;
        //    Query.BuildFilter();
        //    Manager.EncuestaEmpresarial.GetAsignadosInformante(Query);
        //    ModelState.Clear();
        //    return View("Index", Query);
        //}

        public ActionResult EncuestasAnalista(long id, UserInformation user)
        {
            //brb
            //var idUsuario = user.Id;
            //endbrb

            var idUsuario = this.GetLogued().Identificador;

            var establecimiento = Manager.Establecimiento.Find(id);
            if (establecimiento == null) return HttpNotFound("Establecimiento no encontrado");
            ViewBag.Establecimiento = establecimiento;

            bool FirstLoad = false;
            EncuestaEmpresarial criteria = new EncuestaEmpresarial();

            if (Session[CriteriaSesion] != null)
            {
                if (Session[CriteriaSesion] is EncuestaEmpresarial == false)
                {
                    FirstLoad = true;
                }
            }
            else
            {
                FirstLoad = true;
            }

            if (FirstLoad)
            {
                criteria.IdEstablecimiento = id;
                criteria.Year = DateTime.Now.Year;
                Session[CriteriaSesion] = criteria;
            }
            else
            {
                criteria = (EncuestaEmpresarial)Session[CriteriaSesion];
                criteria.IdEstablecimiento = id;
            }

            Session[CriteriaSesion] = criteria;

            Query = GetQuery();
            
            Manager.EncuestaEmpresarial.GetAsignadosAnalista(Query, (long)idUsuario);

            ModelState.Clear();
            return View("IndexAnalista", Query);
        }

        [HttpPost]
        public ActionResult BuscarEncuestaAnalista(EncuestaEmpresarial criteria)
        {
            var idEstablecimiento = ((EncuestaEmpresarial)Session[CriteriaSesion]).IdEstablecimiento;
            Session[CriteriaSesion] = criteria;
            Session[PageSesion] = 1;

            return RedirectToAction("EncuestasAnalista", new { id = idEstablecimiento });
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
        
        //public ActionResult Encuesta(long idEncuesta = 0)
        //{
        //    var manager = Manager;
        //    var encuesta = manager.EncuestaEmpresarial.Find(idEncuesta);
        //    if (encuesta == null) return HttpNotFound("Encuesta No encontrada");
        //    if (encuesta.EstadoEncuesta == EstadoEncuesta.Observada || encuesta.EstadoEncuesta == EstadoEncuesta.NoEnviada)
        //        return View("Encuesta", encuesta);
        //    return View("VerEncuesta", encuesta);
        //}

        //public ActionResult VerEncuesta(long idEncuesta = 0)
        //{
        //    var manager = Manager;
        //    var encuesta = manager.EncuestaEmpresarial.Find(idEncuesta);
        //    if (encuesta == null) return HttpNotFound("Encuestano encontrda");
        //    if (encuesta.EstadoEncuesta == EstadoEncuesta.NoEnviada)
        //    {
        //        encuesta.EstadoEncuesta = EstadoEncuesta.Observada;
        //        manager.EncuestaEmpresarial.Modify(encuesta);
        //        manager.EncuestaEmpresarial.SaveChanges();
        //    }
        //    return View("VerEncuesta", encuesta);

        //}

        //[HttpPost]
        //public ActionResult BuscarEncuestaInformante(EncuestaEmpresarial criteria)
        //{
        //    Query = Query ?? new Query<EncuestaEmpresarial>().Validate();
        //    Query.Criteria = criteria;
        //    Query.Paginacion = Query.Paginacion ?? new Paginacion();
        //    Query.Paginacion.Page = 1;
        //    Query.Criteria.IdEstablecimiento = IdEstablecimiento;
        //    //Query.BuildFilter();
        //    return RedirectToAction("Encuestas", new { id = IdEstablecimiento });
        //}
    }
}
