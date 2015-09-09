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
        public static Query<LineaProductoUnidadMedida> UmQuery { get; set; }
        public static LineaProducto LineaProducto { get; set; }

        public ActionResult GetDorpDown(string id, string nombre = "IdLineaProducto", string @default = null,long idCiuu=0,long idEstablecimiento=0)
        {
            Func<LineaProducto, bool> filter = t => t.Activado;
            if (idCiuu > 0)
            {
                if (idEstablecimiento > 0)
                {
                    filter =
                        t =>
                            t.Activado && t.IdCiiu == idCiuu &&
                            t.LineasProductoEstablecimiento.Any(h => h.IdEstablecimiento == idEstablecimiento);
                }
                else
                {
                    filter = t => t.Activado && t.IdCiiu == idCiuu;
                }
            }
            else
            {
                if (idEstablecimiento > 0)
                {
                    filter =
                        t =>
                            t.Activado  &&
                            t.LineasProductoEstablecimiento.Any(h => h.IdEstablecimiento == idEstablecimiento);
                }
            }
            
            var list = OwnManager.Get(filter).Select(t => new SelectListItem()
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
        public ActionResult GetDorpDownCiiu(string id, string nombre = "IdLineaProducto", string @default = null, long idCiiu = 0, long idEstablecimiento = 0)
        {
                Func<LineaProducto, bool> filter = t => t.Activado && t.IdCiiu == idCiiu;
            if (idEstablecimiento > 0)
            {
                filter = t => t.Activado && t.IdCiiu == idCiiu&& t.LineasProductoEstablecimiento.Any(h => h.IdEstablecimiento == idEstablecimiento);
            }
            var list = OwnManager.Get(filter).Select(t => new SelectListItem()
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
            UmQuery = UmQuery ?? new Query<LineaProductoUnidadMedida>();
            UmQuery = UmQuery.Validate();
            UmQuery.Criteria = UmQuery.Criteria ?? new LineaProductoUnidadMedida();
            UmQuery.Paginacion.ItemsPerPage = 10;

            Manager.UnidadMedida.GetAsignadas(UmQuery, idLineaProducto);
            ViewBag.LineaProductoSeleccionada = LineaProducto.Nombre;
            return PartialView("_UnidadesMedida", UmQuery);
        }
        public ActionResult ToggleUnidadMedida(long idUndadMedida, long? idUnidadConversion = null, decimal? factorConversion = null)
        {
            var linea = LineaProducto;
            if ((idUnidadConversion == null || idUnidadConversion.GetValueOrDefault() < 1) &&
                (factorConversion != null && factorConversion.GetValueOrDefault() > 0))
            {
                var res = new
                {
                    Errors = new List<string>()
                    {
                        "Debe especificar una unidad de conversión"                  
                    }
                };
                return Json(res,JsonRequestBehavior.AllowGet);

            }
            if ((factorConversion == null || factorConversion.GetValueOrDefault() < 1) &&
                (idUnidadConversion != null && idUnidadConversion.GetValueOrDefault() > 0))
            {
                var res = new
                {
                    Errors = new List<string>()
                    {
                       "Debe especificar un factor de conversión"                
                    }
                };
                return Json(res, JsonRequestBehavior.AllowGet);

            }
            if (linea == null) return HttpNotFound("Linea de Producto no encontrada");
            Manager.LineaProducto.ToggleUnidadMedida(linea.Id, idUndadMedida, idUnidadConversion, factorConversion);
            return GetUnidadesMedida(linea.Id);
        }
        public ActionResult PageUnidadesMedida(int page)
        {
            var linea = LineaProducto;
            if (linea == null) return HttpNotFound("Linea de Producto no encontrada");
            UmQuery = UmQuery ?? new Query<LineaProductoUnidadMedida>();
            UmQuery = UmQuery.Validate();
            UmQuery.Paginacion.Page = page;
            return RedirectToAction("GetUnidadesMedida", new { idLineaProducto = linea.Id });
        }

    }
}
