﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain;
using Domain.Managers;
using Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using SelectPdf;
using ClosedXML.Excel;
using Spire.Xls;

namespace WebApplication.Controllers
{
    public class BaseController<T> : Controller where T : class, new()
    {
        private string SuffixSession {
            get {
                return typeof(T).Name;
            }
        }

        public string CriteriaSesion { get { return "Criteria" + SuffixSession; } }
        public string PageSesion { get { return "Page" + SuffixSession; } }
        public string OrderSesion { get { return "Order" + SuffixSession; } }
        
        public Query<T> Query { get; set; }

        public virtual Manager Manager
        {
            get
            {
                return Tools.GetManager(this.User.Identity.Name);
            }
        }

        public virtual GenericManager<T> OwnManager
        {
            get
            {
                return Manager.GetManager<T>();
            }
        }

        public virtual ActionResult Index()
        {
            Query = GetQuery();
            OwnManager.Get(Query);
            ModelState.Clear();
            return View("Index", Query);
        }

        [HttpPost]
        public virtual ActionResult Buscar(T criteria)
        {
            Session[CriteriaSesion] = criteria;
            Session[PageSesion] = 1;
            ModelState.Clear();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public virtual JsonResult CreatePost(T element, params string[] properties)
        {
            //if (ModelState.IsValid)
            //{
            Query = GetQuery();
            var manager = OwnManager;
            var op = IsNew(element) ?
                manager.Add(element) :
                manager.Modify(element, properties);
            if (op.Success)
            {
                manager.SaveChanges();
                //this.WriteMessage(string.Format("Actualizando datos del ciiu {0}", ciiu.Nombre));
                OwnManager.Get(Query);
                var c = RenderRazorViewToString("_Table", Query);
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
            //}
            //else
            //{
            //    var list = new List<string>();
            //    foreach (var v in ModelState.Values)
            //        list.AddRange(v.Errors.Select(t => t.ErrorMessage));
            //    var result = new
            //    {
            //        Success = false,
            //        Errors = list
            //    };
            //    return Json(result, JsonRequestBehavior.AllowGet);

            //}
        }

        public virtual JsonResult Delete(long id)
        {
            Query = GetQuery();
            var manager = OwnManager;
            var op = manager.Delete(id);
            if (op.Success)
            {
                manager.SaveChanges();
                manager.Get(Query);
                var c = RenderRazorViewToString("_Table", Query);
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
                    Errors = new List<string>() { op.Errors[0] }
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public virtual ActionResult Details(long id)
        {
            var manager = OwnManager;
            var element = manager.Find(id);
            if (element != null)
            {
                return View("Details", element);
            }
            else
            {                
                ModelState.Clear();
                ModelState.AddModelError("Error", "No se pudo encontrar el elemento.");

                return RedirectToAction("Index");
            }            
        }

        public virtual ActionResult Page(int page)
        {
            Session[PageSesion] = page;
            ModelState.Clear();
            return RedirectToAction("Index");
        }

        public virtual ActionResult Edit(long id)
        {
            var element = OwnManager.Find(id);
            element = element ?? new T();
            return PartialView("_Create", element);
        }

        public string RenderRazorViewToString(string view, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, view);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public virtual bool IsNew(T element)
        {
            var value = Domain.Tools.GetKeyValue(element);
            return value == 0;
        }

        public virtual FileResult ExportContent(string text)
        {
            var converter = new HtmlToPdf();
            var doc = converter.ConvertHtmlString(text);
            var stream = new MemoryStream();
            doc.Save(stream);
            doc.Close();
            return File(stream, "application/pdf");
        }

        public virtual FileResult Export(string url = null,bool vertical=false)
        {
            if (Request.UrlReferrer == null && url == null)
            {
                throw new FileNotFoundException("Operación Inválida");
            }
            
            var downloads = HttpContext.Server.MapPath("../TempPrint");
            var name = Guid.NewGuid().ToString() + ".pdf";
            var path = Path.Combine(downloads, name);
            var now = DateTime.Now;
            var files =
                Directory.GetFiles(downloads)
                    .Select(t => new FileInfo(t))
                    .Where(t => now.Subtract(t.CreationTime).TotalMinutes > 1)
                    .Select(t => t.FullName);
            foreach (var file in files)
            {
                System.IO.File.Delete(file);
            }
            url = url ?? Request.UrlReferrer.AbsoluteUri;
            url = string.Format(url.Contains("?") 
                ? "{0}&report=true" 
                : "{0}?report=true", url);
            var converter = new HtmlToPdf();
            //converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.ShrinkOnly;
            //converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.ShrinkOnly;
           
            converter.Options.KeepTextsTogether = true;
            if (vertical) converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
            var doc = converter.ConvertUrl(url);

            doc.Save(path);
            doc.Close();

            return File(path, "application/pdf");
        }

        public virtual FileResult ExportExcel<TK>(IList<TK> source, string nombreHoja, string nombreReporte)
        {
            var downloads = HttpContext.Server.MapPath("../TempPrint");
            var name = Guid.NewGuid().ToString() + ".xlsx";
            var path = Path.Combine(downloads, name);
            var now = DateTime.Now;
            var files =
                Directory.GetFiles(downloads)
                    .Select(t => new FileInfo(t))
                    .Where(t => now.Subtract(t.CreationTime).TotalMinutes > 1)
                    .Select(t => t.FullName);
            foreach (var file in files)
            {
                System.IO.File.Delete(file);
            }
            source.ToList().ExportToExcel(nombreHoja, nombreReporte, path);

            return File(path, "application/vnd.ms-excel");
        }

        protected Query<T> GetQuery()
        {
            Query = Query ?? new Query<T>();
            Query = Query.Validate();

            if (typeof(T) != typeof(ParametrizacionEnvio))
            {
                Query.Criteria = new T();
            }
            
            Query.Paginacion = Query.Paginacion ?? new Paginacion();
            
            if (Session[CriteriaSesion] != null)
            {
                if (Session[CriteriaSesion] is T)
                {
                    Query.Criteria = (T)Session[CriteriaSesion];
                }
                else
                {
                    Session[CriteriaSesion] = null;
                    Session[PageSesion] = null;
                    Session[OrderSesion] = null;
                }
            }

            if (Session[PageSesion] != null)
            {
                Query.Paginacion.Page = (int) Session[PageSesion];
            }

            if (Session[OrderSesion] != null)
            {
                Query.Order = (Order<T>)Session[OrderSesion];
            }

            Query.BuildFilter();

            return Query;
        }

        public class ExcelResult : ActionResult
        {   
            private readonly XLWorkbook _workbook;
            private readonly string _fileName;

            public ExcelResult(XLWorkbook workbook, string fileName)
            {
                _workbook = workbook;
                _fileName = fileName;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                var response = context.HttpContext.Response;
                response.Clear();
                response.ContentType = "application/vnd.openxmlformats-officedocument."
                                     + "spreadsheetml.sheet";
                response.AddHeader("content-disposition",
                                   "attachment;filename=\"" + _fileName + ".xlsx\"");

                using (var memoryStream = new MemoryStream())
                {
                    _workbook.SaveAs(memoryStream);
                    memoryStream.WriteTo(response.OutputStream);
                }
                response.End();
            }
        }

        public class ExcelResult2 : ActionResult
        {
            private readonly Workbook _workbook;
            private readonly string _fileName;

            public ExcelResult2(Workbook workbook, string fileName)
            {
                _workbook = workbook;
                _fileName = fileName;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                var response = context.HttpContext.Response;
                response.Clear();
                response.ContentType = "application/vnd.openxmlformats-officedocument."
                                     + "spreadsheetml.sheet";
                response.AddHeader("content-disposition",
                                   "attachment;filename=\"" + _fileName + ".xls\"");

                using (var memoryStream = new MemoryStream())
                {
                    _workbook.SaveToStream(memoryStream);
                    memoryStream.WriteTo(response.OutputStream);
                }
                response.End();
            }
        }
    }
}