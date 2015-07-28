using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Domain.Managers;
using Entity;

namespace WebApplication.Controllers
{
    public class ImportacionExportacionHarinaTrigoController : BaseController<ExportacionHarinaTrigo>
    {
        public static Query<ImportacionHarinaTrigo> QueryImportacion { get; set; }
        public ActionResult GetDorpDown(string id, string nombre = "IdExportacion", string @default = null)
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
            var año = DateTime.Now.Year;
            Query = Query ?? new Query<ExportacionHarinaTrigo>();
            Query.Criteria = Query.Criteria ?? new ExportacionHarinaTrigo();
            Query.Criteria.Año = Query.Criteria.Año ?? DateTime.Now.Year.ToString();
            if (Query.Criteria.Año != null)
            {
                año = int.Parse(Query.Criteria.Año);
            }
            Manager.ExportacionHarinaTrigoManager.Generate(año);
            Query.Order = Query.Order ?? new Order<ExportacionHarinaTrigo>();
            Query.Order.Func = t => t.fecha;
            Query.BuildFilter();
            Manager.ExportacionHarinaTrigoManager.Get(Query);
            
            //ImportacionHarinaTrigo
            QueryImportacion = QueryImportacion ?? new Query<ImportacionHarinaTrigo>();
            QueryImportacion.Criteria = QueryImportacion.Criteria ?? new ImportacionHarinaTrigo();
            QueryImportacion.Criteria.Año = QueryImportacion.Criteria.Año ?? DateTime.Now.Year.ToString();
            if (QueryImportacion.Criteria.Año != null)
            {
                año = int.Parse(QueryImportacion.Criteria.Año);
            }
            Manager.ImportacionHarinaTrigoManager.Generate(año);
            QueryImportacion.Order = QueryImportacion.Order ?? new Order<ImportacionHarinaTrigo>();
            QueryImportacion.Order.Func = t => t.fecha;
            QueryImportacion.BuildFilter();
            Manager.ImportacionHarinaTrigoManager.Get(QueryImportacion);
            return View("Index", new Models.ImportacionExportacionHarinaTrigo() { ExportacionHarinaTrigo = Query, ImportacionHarinaTrigo = QueryImportacion });
        }
        public override ActionResult Buscar(ExportacionHarinaTrigo criteria)
        {
            Manager.ExportacionHarinaTrigoManager.Generate(int.Parse(criteria.Año));
            Manager.ImportacionHarinaTrigoManager.Generate(int.Parse(criteria.Año));

            Query = Query ?? new Query<ExportacionHarinaTrigo>().Validate();
            Query.Criteria = criteria;
            Query.Paginacion = Query.Paginacion ?? new Paginacion();
            Query.Paginacion.Page = 1;
            Query.Paginacion.ItemsPerPage = 12;
            Query.BuildFilter();

            //Importacion
            QueryImportacion = QueryImportacion ?? new Query<ImportacionHarinaTrigo>().Validate();
            QueryImportacion.Criteria = QueryImportacion.Criteria ?? new ImportacionHarinaTrigo();
            QueryImportacion.Criteria.Año = criteria.Año;
            QueryImportacion.Paginacion = QueryImportacion.Paginacion ?? new Paginacion();
            QueryImportacion.Paginacion.ItemsPerPage = 12;
            QueryImportacion.Paginacion.Page = 1;
            QueryImportacion.BuildFilter();
            return RedirectToAction("Index");
        }
        public  ActionResult PageImportacion(int page)
        {
            QueryImportacion = QueryImportacion ?? new Query<ImportacionHarinaTrigo>().Validate();
            QueryImportacion.Paginacion.Page = page;
            return RedirectToAction("Index");
        }
        public override JsonResult CreatePost(ExportacionHarinaTrigo element, params string[] properties)
        {
            return base.CreatePost(element, "fecha");
        }
        public  JsonResult CreatePostImportacion(ImportacionHarinaTrigo element, params string[] properties)
        {
            if (ModelState.IsValid)
            {
                QueryImportacion = QueryImportacion ?? new Query<ImportacionHarinaTrigo>().Validate();
                var manager = Manager.ImportacionHarinaTrigoManager;
                var op = IsNew(element) ?
                    manager.Add(element) :
                    manager.Modify(element, properties);
                if (op.Success)
                {
                    manager.SaveChanges();
                    //this.WriteMessage(string.Format("Actualizando datos del ciiu {0}", ciiu.Nombre));
                    manager.Get(QueryImportacion);
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
        public  ActionResult EditImportacion(long id)
        {
            var element = Manager.ImportacionHarinaTrigoManager.Find(id);
            element = element ?? new ImportacionHarinaTrigo();
            return PartialView("_CreateImportacion", element);
        }
    }
}
