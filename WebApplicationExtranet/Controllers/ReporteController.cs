using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain;
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
                            .Select(t => new SelectListItem() { Text = t.ToString(), Value = t.Id.ToString() })
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
            var user = this.GetLogued();
            if (user == null) return HttpNotFound("Usuario no autenticado");
            if (model == null)
                ModelState.AddModelError("", "Datos Incorrectos");
            if (model != null && (model.Values == null || !model.Values.Any()))
                ModelState.AddModelError("", "Debe seleccionar datos en \"Ámbito de Análisis\"");
            if (model != null && (string.IsNullOrEmpty(model.Output) || string.IsNullOrWhiteSpace(model.Output)))
                ModelState.AddModelError("", "Debe seleccionar una variable");
            if (model != null && (model.Year <= 0))
                ModelState.AddModelError("", "Debe seleccionar un año");
            if (model != null && (model.Month == null || !model.Month.Any()))
                ModelState.AddModelError("", "Debe seleccionar datos al menos un mes");
            if (!ModelState.IsValid)
                return View("GeneralEncuestasEstadisticas/Index");
            ExportGeneralEncuestaEstadistica = model;
            ExportGeneralEncuestaEstadistica.Establecimiento = user.EstablecimientosInformante.Select(t => t.Id);
            var url = Url.Action("RedirectExport", null, null, Request.Url.Scheme);
            return base.Export(url, true);
        }
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Excel")]
        public ActionResult Excel(ExportGeneralEncuestaEstadistica model)
        {
            var user = this.GetLogued();
            if (user == null) return HttpNotFound("Usuario no autenticado");
            if (model == null)
                ModelState.AddModelError("", "Datos Incorrectos");
            if (model != null && (model.Values == null || !model.Values.Any()))
                ModelState.AddModelError("", "Debe seleccionar datos en \"Ámbito de Análisis\"");
            if (model != null && (string.IsNullOrEmpty(model.Output) || string.IsNullOrWhiteSpace(model.Output)))
                ModelState.AddModelError("", "Debe seleccionar una variable");
            if (model != null && (model.Year <= 0))
                ModelState.AddModelError("", "Debe seleccionar un año");
            if (model != null && (model.Month == null || !model.Month.Any()))
                ModelState.AddModelError("", "Debe seleccionar datos al menos un mes");
            if (!ModelState.IsValid)
                return View("GeneralEncuestasEstadisticas/Index");
            ExportGeneralEncuestaEstadistica = model;
            if (ExportGeneralEncuestaEstadistica == null) return HttpNotFound("Datos invalidos");
            if (ExportGeneralEncuestaEstadistica.Output.ToUpper() == "VPMP")
            {
                ExportGeneralEncuestaEstadistica.VolumenProduccionMateriaPropia =
                    Manager.EncuestaEstadistica.GetVolumenProduccionMateriaPropia(
                        ExportGeneralEncuestaEstadistica.InputTypes, ExportGeneralEncuestaEstadistica.Year,
                        ExportGeneralEncuestaEstadistica.Month, ExportGeneralEncuestaEstadistica.Values,
                        ExportGeneralEncuestaEstadistica.Establecimiento);
                return ExportExcel(ExportGeneralEncuestaEstadistica.VolumenProduccionMateriaPropia.Convert(), "Detalles", "Volumen de produccion");
            }
            if (ExportGeneralEncuestaEstadistica.Output.ToUpper() == "VPMT")
            {
                ExportGeneralEncuestaEstadistica.VolumenProduccionMateriaTerceros =
                    Manager.EncuestaEstadistica.GetVolumenProduccionMateriaTerceros(
                        ExportGeneralEncuestaEstadistica.InputTypes, ExportGeneralEncuestaEstadistica.Year,
                        ExportGeneralEncuestaEstadistica.Month, ExportGeneralEncuestaEstadistica.Values,
                        ExportGeneralEncuestaEstadistica.Establecimiento);
                return ExportExcel(ExportGeneralEncuestaEstadistica.VolumenProduccionMateriaTerceros.Convert(), "Detalles", "Volumen de produccion");
            }
            if (ExportGeneralEncuestaEstadistica.Output.ToUpper() == "VAPMP" || ExportGeneralEncuestaEstadistica.Output.ToUpper() == "VAPMT")
            {
                ExportGeneralEncuestaEstadistica.ValorProduccion =
                    Manager.EncuestaEstadistica.GetValorProduccion(
                        ExportGeneralEncuestaEstadistica.InputTypes, ExportGeneralEncuestaEstadistica.Year,
                        ExportGeneralEncuestaEstadistica.Month, ExportGeneralEncuestaEstadistica.Values,
                        ExportGeneralEncuestaEstadistica.Establecimiento);
                return ExportExcel(ExportGeneralEncuestaEstadistica.ValorProduccion
                    .Convert(ExportGeneralEncuestaEstadistica.Output.ToUpper() == "VAPMP"), "Detalles", "Valor de produccion");
            }
            if (ExportGeneralEncuestaEstadistica.Output.ToUpper() == "VVP" || ExportGeneralEncuestaEstadistica.Output.ToUpper() == "VVE")
            {
                ExportGeneralEncuestaEstadistica.VentasPaisExtranjeros =
                    Manager.EncuestaEstadistica.GetValorVentas(
                        ExportGeneralEncuestaEstadistica.InputTypes, ExportGeneralEncuestaEstadistica.Year,
                        ExportGeneralEncuestaEstadistica.Month, ExportGeneralEncuestaEstadistica.Values,
                        ExportGeneralEncuestaEstadistica.Establecimiento);
                return ExportExcel(ExportGeneralEncuestaEstadistica.VentasPaisExtranjeros
                    .Convert(ExportGeneralEncuestaEstadistica.Output.ToUpper() == "VVP"), "Detalles", "Ventas en el pais y en el extranjero");
            }
            if (ExportGeneralEncuestaEstadistica.Output.ToUpper() == "NT")
            {
                ExportGeneralEncuestaEstadistica.TrabajadoresDiasTrabajadoses =
                    Manager.EncuestaEstadistica.GetTrabajadoresDiasTrabajadoses(
                        ExportGeneralEncuestaEstadistica.Year,
                        ExportGeneralEncuestaEstadistica.Month, ExportGeneralEncuestaEstadistica.Values);
                return ExportExcel(ExportGeneralEncuestaEstadistica.TrabajadoresDiasTrabajadoses.Convert(), "Detalles", "Trabajadores y dias trabajados");
            }

            return HttpNotFound("Error en los datos");
        }

        public ActionResult RedirectExport()
        {
            if (ExportGeneralEncuestaEstadistica == null) return HttpNotFound("Datos invalidos");
            if (ExportGeneralEncuestaEstadistica.Output.ToUpper() == "VPMP")
                ExportGeneralEncuestaEstadistica.VolumenProduccionMateriaPropia =
                    Manager.EncuestaEstadistica.GetVolumenProduccionMateriaPropia(
                        ExportGeneralEncuestaEstadistica.InputTypes, ExportGeneralEncuestaEstadistica.Year,
                        ExportGeneralEncuestaEstadistica.Month, ExportGeneralEncuestaEstadistica.Values,
                        ExportGeneralEncuestaEstadistica.Establecimiento);
            if (ExportGeneralEncuestaEstadistica.Output.ToUpper() == "VPMT")
                ExportGeneralEncuestaEstadistica.VolumenProduccionMateriaTerceros =
                    Manager.EncuestaEstadistica.GetVolumenProduccionMateriaTerceros(
                        ExportGeneralEncuestaEstadistica.InputTypes, ExportGeneralEncuestaEstadistica.Year,
                        ExportGeneralEncuestaEstadistica.Month, ExportGeneralEncuestaEstadistica.Values,
                        ExportGeneralEncuestaEstadistica.Establecimiento);
            if (ExportGeneralEncuestaEstadistica.Output.ToUpper() == "VAPMP" || ExportGeneralEncuestaEstadistica.Output.ToUpper() == "VAPMT")
                ExportGeneralEncuestaEstadistica.ValorProduccion =
                    Manager.EncuestaEstadistica.GetValorProduccion(
                        ExportGeneralEncuestaEstadistica.InputTypes, ExportGeneralEncuestaEstadistica.Year,
                        ExportGeneralEncuestaEstadistica.Month, ExportGeneralEncuestaEstadistica.Values,
                        ExportGeneralEncuestaEstadistica.Establecimiento);
            if (ExportGeneralEncuestaEstadistica.Output.ToUpper() == "VVP" || ExportGeneralEncuestaEstadistica.Output.ToUpper() == "VVE")
                ExportGeneralEncuestaEstadistica.VentasPaisExtranjeros =
                    Manager.EncuestaEstadistica.GetValorVentas(
                        ExportGeneralEncuestaEstadistica.InputTypes, ExportGeneralEncuestaEstadistica.Year,
                        ExportGeneralEncuestaEstadistica.Month, ExportGeneralEncuestaEstadistica.Values,
                        ExportGeneralEncuestaEstadistica.Establecimiento);
            if (ExportGeneralEncuestaEstadistica.Output.ToUpper() == "NT")
                ExportGeneralEncuestaEstadistica.TrabajadoresDiasTrabajadoses =
                    Manager.EncuestaEstadistica.GetTrabajadoresDiasTrabajadoses(
                        ExportGeneralEncuestaEstadistica.Year,
                        ExportGeneralEncuestaEstadistica.Month, ExportGeneralEncuestaEstadistica.Values);
            return View("GeneralEncuestasEstadisticas/_ReporteGeneralEstadistico", ExportGeneralEncuestaEstadistica);
        }
    }
}