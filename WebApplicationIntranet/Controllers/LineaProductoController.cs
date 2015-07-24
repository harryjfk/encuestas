using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Domain.Managers;
using Entity;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class LineaProductoController : BaseController<LineaProducto>
    {
        public static Query<UnidadMedida> UmQuery { get; set; }
        public static LineaProducto LineaProducto { get; set; }

        public ActionResult GetDorpDown(string id, string nombre = "IdLineaProducto", string @default = null)
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

        public ActionResult GetUnidadesMedida(long idLineaProducto)
        {
            var linea = Manager.LineaProducto.Find(idLineaProducto);
            if (linea == null) return HttpNotFound("Linea de Producto no encontrada");
            LineaProducto = linea;
            ViewBag.LineaProducto = LineaProducto.Nombre;
            UmQuery = UmQuery ?? new Query<UnidadMedida>();
            UmQuery = UmQuery.Validate();
            UmQuery.Criteria = UmQuery.Criteria ?? new UnidadMedida();
            UmQuery.Paginacion.ItemsPerPage = 10;
            Manager.UnidadMedida.GetAsignadas(UmQuery,idLineaProducto);
            ViewBag.LineaProductoSeleccionada = LineaProducto.Nombre;
            return PartialView("_UnidadesMedida", UmQuery);
        }
        public ActionResult ToggleUnidadMedida(long idUndadMedida)
        {
            var linea = LineaProducto;
            if (linea == null) return HttpNotFound("Linea de Producto no encontrada"); 
            Manager.LineaProducto.ToggleUnidadMedida(linea.Id,idUndadMedida);
            return GetUnidadesMedida(  linea.Id);
        }
        public ActionResult PageUnidadesMedida(int page)
        {
            var linea = LineaProducto;
            if (linea == null) return HttpNotFound("Linea de Producto no encontrada"); 
            UmQuery = UmQuery ?? new Query<UnidadMedida>();
            UmQuery = UmQuery.Validate();
            UmQuery.Paginacion.Page = page;
            return RedirectToAction("GetUnidadesMedida",new {idLineaProducto = linea.Id});
        }

    }
}
