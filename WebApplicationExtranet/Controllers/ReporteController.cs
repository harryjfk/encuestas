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
using Seguridad.PRODUCE;

namespace WebApplication.Controllers
{
    public class ReporteController : BaseController<PorcentajeEncuestaEstadistica>
    {
        public static IndiceVariacion IndiceVariacion { get; set; }

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
                            .Select(t => new SelectListItem() { Text = string.Format("{0}-{1}", t.IdentificadorInterno, t.Nombre), Value = t.Id.ToString() })
                            .ToList();
                    break;
                case "ciiu":
                    model =
                        establecimientos.SelectMany(t => t.Ciius).Distinct()
                            .Select(t => new SelectListItem() { Text = t.Ciiu.Nombre, Value = t.IdCiiu.ToString() })
                            .ToList();
                    break;
                case "producto":
                    model =
                        establecimientos.SelectMany(t => t.LineasProductoEstablecimiento).Distinct()
                            .Select(t => new SelectListItem() { Text = t.LineaProducto.Nombre, Value = t.IdLineaProducto.ToString() })
                            .ToList();
                    break;
                case "lineaproducto":
                    model =
                        establecimientos.SelectMany(t => t.LineasProductoEstablecimiento).Distinct()
                            .Select(t => new SelectListItem() { Text = t.LineaProducto.Nombre, Value = t.IdLineaProducto.ToString() })
                            .ToList();
                    break;
            }
            return PartialView("GeneralEncuestasEstadisticas/_MultiSelectListView", model);
        }

        public ActionResult IndiceVariacionIndex(UserInformation usuario)
        {
            var manager = Manager;

            var user = this.GetLogued();

            //brb
            //var user = usuario;
            //endbrb

            if (user == null && IndiceVariacion == null) return HttpNotFound("Usuario no autenticado");
            var report = IndiceVariacion ?? new IndiceVariacion()
            {
                IsVariacion = false,
                Year = DateTime.Now.Year,
                IdInformante = (long)user.Identificador
                //brb
                //IdInformante = (long)user.Id
                //endbrb
            };
            report = manager.EncuestaEstadistica.IndiceVariacion(report.IdInformante, report.Year, !report.IsVariacion);
            IndiceVariacion = report;
            return View("IndiceVariacion/Index", report);
        }
        
        //
        // GET: /PorcentajeEncuestaEstadistica/
        [HttpPost]
        public ActionResult BuscarIndiceVariacion(IndiceVariacion filter)
        {
            var user = this.GetLogued();
            if (user == null) return HttpNotFound("Usuario no autenticado");
            filter = filter ?? new IndiceVariacion()
            {
                IsVariacion = false,
                Year = DateTime.Now.Year,
                IdInformante = (long)user.Identificador

            };
            filter.IdInformante = (long)user.Identificador;
            filter.IsVariacion = Convert.ToBoolean(filter.IsVariacionStr);
            var report = Manager.EncuestaEstadistica.IndiceVariacion(filter.IdInformante, filter.Year, !filter.IsVariacion);
            IndiceVariacion = report;
            return View("IndiceVariacion/Index", report);
        }

        public ActionResult IndiceVariacionToExcel()
        {
            if (IndiceVariacion == null || IndiceVariacion.Indice.Count == 0)
                return HttpNotFound("No hay datos para mostrar");
            var result = new List<ExportIndiceVariacionToExcel>();
            var array = new[]
            {
                "Sector Farril Total",
                "Manufactura Primaria",
                "Manufactura no Primaria",
                "Bienes de Consumo",
                "Bienes Intermedios",
                "Bienes de Capital",
                "Servicio",
                "CIIU del Informante"
            };
            foreach (var s in array)
            {
                var nId = s.GetHashCode();
                var n = IndiceVariacion.Indice.FirstOrDefault(t => t.Id == nId);
                if (n != null)
                {
                    result.Add(new ExportIndiceVariacionToExcel()
                    {
                        Sector_División_Grupo_Clase = s,
                        Enero = n.Values.FirstOrDefault(t => t.Number == 1) != null ? n.Values.FirstOrDefault(t => t.Number == 1).Value : 0,
                        Febrero = n.Values.FirstOrDefault(t => t.Number == 2) != null ? n.Values.FirstOrDefault(t => t.Number == 2).Value : 0,
                        Marzo = n.Values.FirstOrDefault(t => t.Number == 3) != null ? n.Values.FirstOrDefault(t => t.Number == 3).Value : 0,
                        Abril = n.Values.FirstOrDefault(t => t.Number == 4) != null ? n.Values.FirstOrDefault(t => t.Number == 4).Value : 0,
                        Mayo = n.Values.FirstOrDefault(t => t.Number == 5) != null ? n.Values.FirstOrDefault(t => t.Number == 5).Value : 0,
                        Junio = n.Values.FirstOrDefault(t => t.Number == 6) != null ? n.Values.FirstOrDefault(t => t.Number == 6).Value : 0,
                        Julio = n.Values.FirstOrDefault(t => t.Number == 7) != null ? n.Values.FirstOrDefault(t => t.Number == 7).Value : 0,
                        Agosto = n.Values.FirstOrDefault(t => t.Number == 8) != null ? n.Values.FirstOrDefault(t => t.Number == 8).Value : 0,
                        Septiembre = n.Values.FirstOrDefault(t => t.Number == 9) != null ? n.Values.FirstOrDefault(t => t.Number == 9).Value : 0,
                        Octubre = n.Values.FirstOrDefault(t => t.Number == 10) != null ? n.Values.FirstOrDefault(t => t.Number == 10).Value : 0,
                        Noviembre = n.Values.FirstOrDefault(t => t.Number == 11) != null ? n.Values.FirstOrDefault(t => t.Number == 11).Value : 0,
                        Diciembre = n.Values.FirstOrDefault(t => t.Number == 12) != null ? n.Values.FirstOrDefault(t => t.Number == 12).Value : 0,
                    });
                }
            }
            var ciius = IndiceVariacion.Indice.Where(t => t.IsCiiuInformante);
            foreach (var n in ciius)
            {
                result.Add(new ExportIndiceVariacionToExcel()
                {
                    Sector_División_Grupo_Clase = n.Name,
                    Enero = n.Values.FirstOrDefault(t => t.Number == 1) != null ? n.Values.FirstOrDefault(t => t.Number == 1).Value : 0,
                    Febrero = n.Values.FirstOrDefault(t => t.Number == 2) != null ? n.Values.FirstOrDefault(t => t.Number == 2).Value : 0,
                    Marzo = n.Values.FirstOrDefault(t => t.Number == 3) != null ? n.Values.FirstOrDefault(t => t.Number == 3).Value : 0,
                    Abril = n.Values.FirstOrDefault(t => t.Number == 4) != null ? n.Values.FirstOrDefault(t => t.Number == 4).Value : 0,
                    Mayo = n.Values.FirstOrDefault(t => t.Number == 5) != null ? n.Values.FirstOrDefault(t => t.Number == 5).Value : 0,
                    Junio = n.Values.FirstOrDefault(t => t.Number == 6) != null ? n.Values.FirstOrDefault(t => t.Number == 6).Value : 0,
                    Julio = n.Values.FirstOrDefault(t => t.Number == 7) != null ? n.Values.FirstOrDefault(t => t.Number == 7).Value : 0,
                    Agosto = n.Values.FirstOrDefault(t => t.Number == 8) != null ? n.Values.FirstOrDefault(t => t.Number == 8).Value : 0,
                    Septiembre = n.Values.FirstOrDefault(t => t.Number == 9) != null ? n.Values.FirstOrDefault(t => t.Number == 9).Value : 0,
                    Octubre = n.Values.FirstOrDefault(t => t.Number == 10) != null ? n.Values.FirstOrDefault(t => t.Number == 10).Value : 0,
                    Noviembre = n.Values.FirstOrDefault(t => t.Number == 11) != null ? n.Values.FirstOrDefault(t => t.Number == 11).Value : 0,
                    Diciembre = n.Values.FirstOrDefault(t => t.Number == 12) != null ? n.Values.FirstOrDefault(t => t.Number == 12).Value : 0,
                });
            }
            return ExportExcel(result, "Detalles", "Indice y Variacion");
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