using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Domain.Managers;
using Entity;

namespace WebApplication.Controllers
{
    public class UnidadMedidaController : BaseController<UnidadMedida>
    {
        public ActionResult GetDorpDown(string id, string nombre = "IdUnidadMedida", string @default = null)
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

        public ActionResult GetDorpDownLineaProducto(long idLineaProducto,string id, string nombre = "IdUnidadMedida", string @default = null)
        {
            var list = OwnManager.Get(t => 
                t.LineasProductoUnidadMedida.Any(h=>h.id_linea_producto==idLineaProducto)).Select(t => new SelectListItem()
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
    }
}
