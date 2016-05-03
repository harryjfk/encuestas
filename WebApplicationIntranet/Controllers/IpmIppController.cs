using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Domain.Managers;
using Entity;
using System.Globalization;
using Seguridad.PRODUCE;

namespace WebApplication.Controllers
{
    /*[Authorize]
    [Autorizacion]*/
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

        public override ActionResult Index()
        {
            bool FirstLoad = false;
            
            if (Session[CriteriaSesion] != null)
            {
                if (Session[CriteriaSesion] is IpmIpp == false)
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
                IpmIpp criteria = new IpmIpp();
                criteria.id_ciiu = -1;
                criteria.Año = DateTime.Now.Year.ToString();
                Session[CriteriaSesion] = criteria;
            }

            Order<IpmIpp> order = new Order<IpmIpp>();
            order.Func = t => t.fecha;
            Session[OrderSesion] = order;

            return base.Index();
        }

        public override ActionResult Buscar(IpmIpp criteria)
        {
            Manager.IpmIppManager.Generate(criteria.id_ciiu, int.Parse(criteria.Año));
            return base.Buscar(criteria);
        }

        public override JsonResult CreatePost(IpmIpp element, params string[] properties)
        {
            return base.CreatePost(element, "fecha", "id_ciiu", "ipp");
        }

        #region LoadIpp
        public ActionResult IndexCarga()
        {
            List<DateTime> fechas = new List<DateTime>();
            for (int i = 1; i <= 12; ++i)
            {
                fechas.Add(new DateTime(2015, i, 1));
            }

            CultureInfo esEs = CultureInfo.CreateSpecificCulture("es-ES");
            List<string> meses = new List<string>();
            foreach (var fecha in fechas)
            {
                meses.Add(fecha.Month.ToString() + "," + fecha.ToString("MMMM", esEs).Substring(0, 1).ToUpper() + fecha.ToString("MMMM", esEs).Substring(1, fecha.ToString("MMMM", esEs).Length - 1));
            }

            ViewBag.Meses = meses;
            return View();
        }

        [HttpGet]
        public JsonResult GetIpp(int anio, int mes)
        {
            return Json(Manager.IpmIppManager.GetValorIppPorAnioMes(anio, mes), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProcessIpp(int anio, int mes)
        {
            Manager.IpmIppManager.ProcessIpp(anio, mes);
            return Json(true);
        }
        #endregion 
    }
}
