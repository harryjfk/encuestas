using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Domain.Managers;
using Entity;

namespace WebApplication.Controllers
{
    public class IpmIppController : BaseController<IpmIpp>
    {
        public ActionResult GetDorpDown(string id, string nombre = "IdIpmIpp", string @default = null)
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
           
            Query = Query ?? new Query<IpmIpp>();
            Query.Criteria = Query.Criteria ?? new IpmIpp();
            Query.Criteria.Año = Query.Criteria.Año ?? DateTime.Now.Year.ToString();
            var idCiiu = Query.Criteria.id_ciiu > 0 ? Query.Criteria.id_ciiu : Manager.Ciiu.Get().FirstOrDefault().Id;
            var año = DateTime.Now.Year;
            if (Query.Criteria.Año != null)
            {
                año = int.Parse(Query.Criteria.Año);
            }
            Manager.IpmIppManager.Generate(idCiiu, año);
            Query.Order = Query.Order ?? new Order<IpmIpp>();
            Query.Criteria.id_ciiu =idCiiu ;
            Query.Order.Func = t => t.fecha;
            Query.BuildFilter();
            return base.Index();
        }
        public override ActionResult Buscar(IpmIpp criteria)
        {
            Manager.IpmIppManager.Generate(criteria.id_ciiu,int.Parse(criteria.Año));
            return base.Buscar(criteria);
        }

        public override JsonResult CreatePost(IpmIpp element, params string[] properties)
        {
            return base.CreatePost(element, "fecha", "id_ciiu", "ipp");
        }
    }
}
