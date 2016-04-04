using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Domain.Managers;
using Entity;
using WebApplication.Models;
using Seguridad.PRODUCE;

namespace WebApplication.Controllers
{
    /*[Authorize]
    [Autorizacion]*/
    public class LineaProductoController : BaseController<LineaProducto>
    {
        public Query<LineaProductoUnidadMedida> UmQuery { get; set; }
        public LineaProducto LineaProducto
        {
            get {
                if (Session["LineaProducto"] != null)
                {
                    return (LineaProducto) Session["LineaProducto"];
                }
                else
                {
                    return null;
                }
            }
            set {
                Session["LineaProducto"] = value;
            }
        }

        public ActionResult GetDorpDown(string id, string nombre = "IdLineaProducto", string @default = null, long idCiuu = 0, long idEstablecimiento = 0)
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
                            t.Activado &&
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

        public ActionResult GetDorpDownCiiu(string id, string nombre = "IdLineaProducto", string @default = null, long idCiuu = 0, long idEstablecimiento = 0)
        {
            Func<LineaProducto, bool> filter = t => t.Activado && t.IdCiiu == idCiuu && t.Codigo.Length == 7;
            if (idEstablecimiento > 0)
            {
                filter = t => t.Activado && t.IdCiiu == idCiuu && t.LineasProductoEstablecimiento.Any(h => h.IdEstablecimiento == idEstablecimiento) && t.Codigo.Length == 7;
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
            Query = GetQuery();

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
            UmQuery = GetUmQuery();
            var linea = Manager.LineaProducto.Find(idLineaProducto);
            if (linea == null) return HttpNotFound("Linea de Producto no encontrada");

            ViewBag.LineaProducto = linea.Nombre;
            ViewBag.LineaProductoSeleccionada = linea.Nombre;

            LineaProducto = linea;            

            Manager.UnidadMedida.GetAsignadas(UmQuery, idLineaProducto);
            
            return PartialView("_UnidadesMedida", UmQuery);
        }

        public ActionResult ToggleUnidadMedida(long idUndadMedida, long? idUnidadConversion = null, decimal? factorConversion = null)
        {
            if (LineaProducto == null) return HttpNotFound("Linea de Producto no encontrada");

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
            
            var result = Manager.LineaProducto.ToggleUnidadMedida(LineaProducto.Id, idUndadMedida, idUnidadConversion, factorConversion);

            if (!result)
            {
                var res = new
                {
                    Errors = new List<string>()
                    {
                       "No se puede eliminar la unidad de medida, está en uso por el año base."
                    }
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }

            return GetUnidadesMedida(LineaProducto.Id);
        }

        public ActionResult PageUnidadesMedida(int page)
        {
            if (LineaProducto == null) return HttpNotFound("Linea de Producto no encontrada");
            Session["PageUM"] = page;
            ModelState.Clear();
            return RedirectToAction("GetUnidadesMedida", new { idLineaProducto = LineaProducto.Id });
        }

        public Query<LineaProductoUnidadMedida> GetUmQuery()
        {
            UmQuery = UmQuery ?? new Query<LineaProductoUnidadMedida>();
            UmQuery = UmQuery.Validate();
            UmQuery.Criteria = new LineaProductoUnidadMedida();
            UmQuery.Paginacion = UmQuery.Paginacion ?? new Paginacion();
            UmQuery.Paginacion.ItemsPerPage = 10;

            if (Session["CriteriaUM"] != null)
            {
                if (Session["CriteriaUM"] is LineaProductoUnidadMedida)
                {
                    UmQuery.Criteria = (LineaProductoUnidadMedida)Session["CriteriaUM"];
                }
                else
                {
                    Session["CriteriaUM"] = null;
                    Session["PageUM"] = null;
                    Session["OrdenUM"] = null;
                }
            }

            if (Session["PageUM"] != null)
            {
                UmQuery.Paginacion.Page = (int)Session["PageUM"];
            }

            if (Session["OrdenUM"] != null)
            {
                UmQuery.Order = (Order<LineaProductoUnidadMedida>)Session["OrderUM"];
            }

            UmQuery.BuildFilter();

            return UmQuery;
        }
    }
}
