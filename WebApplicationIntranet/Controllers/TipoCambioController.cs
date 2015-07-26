using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Domain.Managers;
using Entity;

namespace WebApplication.Controllers
{
    public class TipoCambioController : BaseController<TipoCambio>
    {
        public ActionResult GetDorpDown(string id, string nombre = "IdTipoCambio", string @default = null)
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

        public override ActionResult Index()
        {
            Manager.TipoCambioManager.Generate();
            Query = Query ?? new Query<TipoCambio>();
            Query.Criteria = Query.Criteria ?? new TipoCambio();
            Query.Criteria.Año = Query.Criteria.Año ?? DateTime.Now.Year.ToString();
            Query.Order = Query.Order ?? new Order<TipoCambio>();
            Query.Order.Func = t => t.fecha;
            Query.BuildFilter();
            return base.Index();
        }
    }
}
