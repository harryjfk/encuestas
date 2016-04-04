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
    /*[Authorize]
    [Autorizacion]*/
    public class ContactoController : BaseController<Contacto>
    {
        private static long IdEstablecimiento { get; set; }

        public ActionResult GetDorpDown(string id, string nombre = "IdContacto", string @default = null)
        {
           var list =OwnManager.Get(t => t.Activado).Select(t => new SelectListItem()
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
       
        public JsonResult Toggle(long id)
        {
            var manager = OwnManager;
            var element = manager.Find(id);
            if (element != null)
            {
               Manager.Contacto.EstablecerPredeterminado(element.Id);
                manager.SaveChanges();
            }
            OwnManager.Get(Query);
            var c = RenderRazorViewToString("_Table", Query);
            var result = new
            {
                Success = true,
                Data = c
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public override ActionResult Index()
        {
            Query = Query ?? new Query<Contacto>().Validate();
            Query.Criteria = Query.Criteria ?? new Contacto();
            Query.Criteria.IdEstablecimiento = IdEstablecimiento;
            Query.BuildFilter();
            return base.Index();
        }
        public ActionResult Get(long id)
        {
            var establecimiento = Manager.Establecimiento.Find(id);
            if (establecimiento == null) return HttpNotFound("El establecimiento no existe");
            IdEstablecimiento = id;
            Query = Query ?? new Query<Contacto>().Validate();
            Query.Criteria = Query.Criteria ?? new Contacto();
            Query.Criteria.Establecimiento = establecimiento;
            return RedirectToAction("Index");
        }

        public override JsonResult CreatePost(Contacto element,params string [] properties)
        {
            element.IdEstablecimiento = IdEstablecimiento;
            return base.CreatePost(element);
        }

        public override ActionResult Buscar(Contacto criteria)
        {
            Query = Query ?? new Query<Contacto>().Validate();
            Query.Criteria = Query.Criteria ?? new Contacto();
            criteria.Establecimiento = Query.Criteria.Establecimiento;
            return base.Buscar(criteria);
        }
    }
}
