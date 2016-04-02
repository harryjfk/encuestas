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
    public class CiiuController : BaseController<Ciiu>
    {
        public ActionResult GetDorpDown(string id, string nombre = "IdCiiu", string @default = null)
        {

            var list = OwnManager.Get(t => t.Activado).OrderBy(t => t.ToString()).Select(t => new SelectListItem()
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

        public ActionResult GetDorpDownIpmIpp(string id, string nombre = "IdCiiu", string @default = null)
        {

            var list = OwnManager.Get(t => t.Activado
                && t.CAT_METODO_CALCULO!=null &&(
                t.CAT_METODO_CALCULO.nombre == "VD-IIP" 
                || t.CAT_METODO_CALCULO.nombre == "VD-IPM"
                || t.CAT_METODO_CALCULO.nombre == "Consumo Aparente")
                ).OrderBy(t => t.ToString()).Select(t => new SelectListItem()
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

        public ActionResult GetDorpDownEstablecimiento(string id, string nombre = "IdCiiu", string @default = null, long idEstablecimiento = 0,bool showAllCollection=true)
        {
            Func<Ciiu, bool> filter = t => t.Activado;
            Func<Ciiu, bool> filter2 = t => t.Activado;

            if (idEstablecimiento > 0||!showAllCollection)
            {                
                filter2 = t => filter(t) && t.Establecimientos.Any(h => h.IdEstablecimiento == idEstablecimiento);
            }
            var list = OwnManager.Get(filter2).OrderBy(t => t.ToString()).Select(t => new SelectListItem()
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

        public JsonResult GetMetodoCalculo(long idCiiu)
        {
            return Json(OwnManager.Get(t => t.Id == idCiiu).FirstOrDefault().id_metodo_calculo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Toggle(long id)
        {
            Query = GetQuery();
            var manager = Manager.Ciiu;
            var element = manager.Find(id);
            if (element != null)
            {
                element.Activado = !element.Activado;
                manager.Modify(element);
                manager.SaveChanges();
            }
            Manager.Ciiu.Get(Query);
            var c = RenderRazorViewToString("_Table", Query);
            var result = new
            {
                Success = true,
                Data = c
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}