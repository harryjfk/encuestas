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
            Query = GetQuery();

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
        
        public ActionResult Get(long id)
        {
            bool FirstLoad = false;

            if (Session[CriteriaSesion] != null)
            {
                if (Session[CriteriaSesion] is Contacto == false)
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
                Contacto criteria = new Contacto();
                criteria.IdEstablecimiento = id;
                Session[CriteriaSesion] = criteria;
            }

            return RedirectToAction("Index");
        }

        public override JsonResult CreatePost(Contacto element,params string [] properties)
        {
            element.IdEstablecimiento = ((Contacto)Session[CriteriaSesion]).IdEstablecimiento;
            return base.CreatePost(element);
        }

        public override ActionResult Buscar(Contacto criteria)
        {
            criteria.IdEstablecimiento = ((Contacto)Session[CriteriaSesion]).IdEstablecimiento;
            return base.Buscar(criteria);
        }
    }
}
