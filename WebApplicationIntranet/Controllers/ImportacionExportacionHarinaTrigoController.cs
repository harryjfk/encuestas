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
    public class ImportacionExportacionHarinaTrigoController : BaseController<ExportacionHarinaTrigo>
    {
        public Query<ImportacionHarinaTrigo> QueryImportacion { get; set; }

        public ActionResult GetDorpDown(string id, string nombre = "IdExportacion", string @default = null)
        {
            var list = OwnManager.Get(t => t.Activado).Select(t => new SelectListItem()
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

        public JsonResult ToggleImportacion(long id)
        {
            var managerImportacion = Manager.ImportacionHarinaTrigoManager;
            QueryImportacion = GetQueryImportacion();
            
            var element = managerImportacion.Find(id);
            if (element != null)
            {
                element.Activado = !element.Activado;
                managerImportacion.Modify(element);
                managerImportacion.SaveChanges();
            }
            Manager.ImportacionHarinaTrigoManager.Get(QueryImportacion);
            var c = RenderRazorViewToString("_TableImportacion", QueryImportacion);
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
                if (Session[CriteriaSesion] is ExportacionHarinaTrigo == false)
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
                Manager.ExportacionHarinaTrigoManager.Generate(yearNow);
                Manager.ImportacionHarinaTrigoManager.Generate(yearNow);

                ExportacionHarinaTrigo criteria = new ExportacionHarinaTrigo();
                criteria.Año = yearNow.ToString();
                Session[CriteriaSesion] = criteria;

                ImportacionHarinaTrigo criteriaImportacion = new ImportacionHarinaTrigo();
                criteriaImportacion.Año = yearNow.ToString();
                Session["criteriaImportacion"] = criteriaImportacion;
            }

            Order<ExportacionHarinaTrigo> order = new Order<ExportacionHarinaTrigo>();
            order.Func = t => t.fecha;
            Session[OrderSesion] = order;

            Order<ImportacionHarinaTrigo> orderImportacion = new Order<ImportacionHarinaTrigo>();
            orderImportacion.Func = t => t.fecha;
            Session["OrdenImportacion"] = orderImportacion;

            Query = base.GetQuery();
            QueryImportacion = GetQueryImportacion();

            Manager.ExportacionHarinaTrigoManager.Get(Query);
            Manager.ImportacionHarinaTrigoManager.Get(QueryImportacion);

            return View("Index", new Models.ImportacionExportacionHarinaTrigo() { ExportacionHarinaTrigo = Query, ImportacionHarinaTrigo = QueryImportacion });
        }

        public override ActionResult Buscar(ExportacionHarinaTrigo criteria)
        {
            Manager.ExportacionHarinaTrigoManager.Generate(int.Parse(criteria.Año));
            Manager.ImportacionHarinaTrigoManager.Generate(int.Parse(criteria.Año));
            
            ImportacionHarinaTrigo criteriaImportacion = new ImportacionHarinaTrigo();
            criteriaImportacion.Año = criteria.Año;            

            Session[CriteriaSesion] = criteria;
            Session[PageSesion] = 1;

            Session["CriteriaImportacion"] = criteriaImportacion;
            Session["PageImportacion"] = 1;

            ModelState.Clear();            

            return RedirectToAction("Index");
        }

        public ActionResult PageImportacion(int page)
        {
            Session["PageImportacion"] = page;
            return RedirectToAction("Index");
        }

        public override JsonResult CreatePost(ExportacionHarinaTrigo element, params string[] properties)
        {
            return base.CreatePost(element, "fecha");
        }

        public JsonResult CreatePostImportacion(ImportacionHarinaTrigo element, params string[] properties)
        {
            if (ModelState.IsValid)
            {
                QueryImportacion = GetQueryImportacion();
                var manager = Manager.ImportacionHarinaTrigoManager;
                var op = manager.Modify(element, "fecha");
                if (op.Success)
                {
                    manager.SaveChanges();
                    //this.WriteMessage(string.Format("Actualizando datos del ciiu {0}", ciiu.Nombre));
                    Manager.ImportacionHarinaTrigoManager.Get(QueryImportacion);
                    var c = RenderRazorViewToString("_TableImportacion", QueryImportacion);
                    var result = new
                    {
                        Success = true,
                        Data = c
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = new
                    {
                        Success = false,
                        Errors = op.Errors.Select(t => t).ToList()
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var list = new List<string>();
                foreach (var v in ModelState.Values)
                    list.AddRange(v.Errors.Select(t => t.ErrorMessage));
                var result = new
                {
                    Success = false,
                    Errors = list
                };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult EditImportacion(long id)
        {
            var element = Manager.ImportacionHarinaTrigoManager.Find(id);
            element = element ?? new ImportacionHarinaTrigo();
            return PartialView("_CreateImportacion", element);
        }

        public ActionResult DetailsImportaccion(long id)
        {
            var manager = Manager.ImportacionHarinaTrigoManager;
            var element = manager.Find(id);
            if (element != null)
            {
                return View("DetailsImportacion", element);
            }
            else
            {
                ModelState.Clear();
                ModelState.AddModelError("Error", "No se pudo encontrar el elemento.");

                return RedirectToAction("Index");
            }            
        }

        public JsonResult GetTipoCambioVenta(long id)
        {
            return Json(Manager.ExportacionHarinaTrigoManager.GetTipoCambioVenta(id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTipoCambioCompra(long id)
        {
            return Json(Manager.ImportacionHarinaTrigoManager.GetTipoCambioVenta(id), JsonRequestBehavior.AllowGet);
        }

        public Query<ImportacionHarinaTrigo> GetQueryImportacion()
        {
            QueryImportacion = QueryImportacion ?? new Query<ImportacionHarinaTrigo>();
            QueryImportacion = QueryImportacion.Validate();
            QueryImportacion.Paginacion = QueryImportacion.Paginacion ?? new Paginacion();

            if (Session["CriteriaImportacion"] != null)
            {
                if (Session["CriteriaImportacion"] is ImportacionHarinaTrigo)
                {
                    QueryImportacion.Criteria = (ImportacionHarinaTrigo)Session["CriteriaImportacion"];
                }
                else
                {
                    Session["CriteriaImportacion"] = null;
                    Session["PageImportacion"] = null;
                    Session["OrdenImportacion"] = null;
                }
            }

            if (Session["PageImportacion"] != null)
            {
                QueryImportacion.Paginacion.Page = (int)Session["PageImportacion"];
            }

            if (Session["OrdenImportacion"] != null)
            {
                QueryImportacion.Order = (Order<ImportacionHarinaTrigo>)Session["OrderImportacion"];
            }

            QueryImportacion.BuildFilter();

            return QueryImportacion;
        }
    }
}
