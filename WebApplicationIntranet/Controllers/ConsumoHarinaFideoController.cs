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
    public class ConsumoHarinaFideoController : BaseController<ConsumoHarinaFideo>
    {
        public ActionResult GetDorpDown(string id, string nombre = "IdConsumo", string @default = null)
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
            Query = base.GetQuery();
            var manager = OwnManager;
            var element = manager.Find(id);
            if (element != null)
            {
                element.Activado = !element.Activado;
                manager.Modify(element);
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

        public override ActionResult Buscar(ConsumoHarinaFideo criteria)
        {
            Manager.ConsumoHarinaFideoManager.Generate(int.Parse(criteria.Año));
            return base.Buscar(criteria);
        }

        public override ActionResult Index()
        {
            bool FirstLoad = false;
            if (Session[CriteriaSesion] != null)
            {
                if (Session[CriteriaSesion] is ConsumoHarinaFideo == false)
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
                var yearNow = DateTime.Now.Year;
                Manager.ConsumoHarinaFideoManager.Generate(yearNow);

                ConsumoHarinaFideo criteria = new ConsumoHarinaFideo();
                criteria.Año = yearNow.ToString();
                Session[CriteriaSesion] = criteria;
            }

            Order<ConsumoHarinaFideo> order = new Order<ConsumoHarinaFideo>();
            order.Func = t => t.fecha;
            Session[OrderSesion] = order;

            return base.Index();
        }
        
        public override JsonResult CreatePost(ConsumoHarinaFideo element, params string[] properties)
        {
            return base.CreatePost(element, "fecha");
        }
    }
}
