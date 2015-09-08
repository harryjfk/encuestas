using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;
using Entity.Reportes;
using WebApplication.Binders;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class ReporteController : BaseController<PorcentajeEncuestaEstadistica>
    {
        public static ExportGeneralEncuestaEstadistica ExportGeneralEncuestaEstadistica { get; set; }

        public ActionResult GeneralEncuestaEstadistica()
        {
            return View("GeneralEncuestasEstadisticas/Index");
        }
        public ActionResult GetCiiuReporteGeneralEncuestaEstadistica(string type)
        {
            var user = this.GetLogued();
            if (user == null) return HttpNotFound("Usuario no autenticado");
            var establecimientos = user.EstablecimientosInformante;
            var model = new List<SelectListItem>();
            switch (type.ToLower())
            {
                case "establecimiento":
                    model =
                        establecimientos
                            .Select(t => new SelectListItem() { Text = t.Nombre, Value = t.Id.ToString() })
                            .ToList();
                    break;
                case "ciiu":
                    model =
                        establecimientos.SelectMany(t => t.Ciius).Distinct()
                            .Select(t => new SelectListItem() { Text = t.Nombre, Value = t.Id.ToString() })
                            .ToList();
                    break;
                case "lineaproducto":
                    model =
                        establecimientos.SelectMany(t => t.Ciius.SelectMany(h => h.LineasProducto)).Distinct()
                            .Select(t => new SelectListItem() { Text = t.ToString(), Value = t.Id.ToString() })
                            .ToList();
                    break;
            }
            return PartialView("GeneralEncuestasEstadisticas/_MultiSelectListView", model);
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "PDF")]
        public ActionResult PDF(ExportGeneralEncuestaEstadistica model)
        {
            ExportGeneralEncuestaEstadistica = model;
            var url = Url.Action("RedirectExport", null, null, Request.Url.Scheme);
            return base.Export(url,true);
        }
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Excel")]
        public ActionResult Excel(ExportGeneralEncuestaEstadistica model)
        {
            ExportGeneralEncuestaEstadistica = model;
            var url = Url.Action("RedirectExport", null, null, Request.Url.Scheme);
            return base.Export(url, true);
        }

        public ActionResult RedirectExport()
        {
            if (ExportGeneralEncuestaEstadistica == null) return HttpNotFound("Datos invalidos");
            return View("GeneralEncuestasEstadisticas/_ReporteGeneralEstadistico", ExportGeneralEncuestaEstadistica);
        }
    }
}