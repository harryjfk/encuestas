using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;
using Entity.Reportes;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class ReporteController : BaseController<PorcentajeEncuestaEstadistica>
    {
        public static PorcentajeEncuestaEstadistica PocentajeEncuestaEstadistica { get; set; }

        public ActionResult PorcentajeEncuestaEstadistica()
        {
            var filter = new PorcentajeEncuestaEstadisticaFilter()
            {
                IsAnnual = true,
                Year = DateTime.Now.Year,
                Estado = EstadoEncuesta.Todos,
                From = 1,
                To = 12,
            };
            var report =PocentajeEncuestaEstadistica?? new PorcentajeEncuestaEstadistica()
            {
                Filter = filter
            };
            report = Manager.EncuestaEstadistica.PorcentajeEncuestaEstadistica(report.Filter);
            PocentajeEncuestaEstadistica = report;
            return View("PorcentajeEncuestaEstadistica/Index", report);
        }
        //
        // GET: /PorcentajeEncuestaEstadistica/
        [HttpPost]
        public ActionResult BuscarPorcentajeEncuestaEstadistica(PorcentajeEncuestaEstadisticaFilter filter)
        {
            filter = filter ?? new PorcentajeEncuestaEstadisticaFilter()
            {
                IsAnnual = true,
                Year = DateTime.Now.Year,
                Estado = EstadoEncuesta.Todos,
                From = 1,
                To = 12
            };
            var report = Manager.EncuestaEstadistica.PorcentajeEncuestaEstadistica(filter);
            PocentajeEncuestaEstadistica = report;
            return View("PorcentajeEncuestaEstadistica/Index", report);
        }

        public ActionResult GraphicPorcentajeEstadistica()
        {
            if (PocentajeEncuestaEstadistica == null || PocentajeEncuestaEstadistica.Elements.Count == 0)
                return HttpNotFound("No hay datos para mostrar");
            var data = new GraphicModel()
            {
                Title = "Porcentaje de Encuestas Estad&iacute;sticas",
                YTitle = "Porciento",
                Serie = new Dictionary<string, double>()
            };
            foreach (var element in PocentajeEncuestaEstadistica.Elements)
            {
                data.Serie.Add(element.Analista, element.Month.Sum(h => h.MonthlyValue));
            }
            return PartialView("_Graphic", data);
        }
        public ActionResult ExportPorcentajeEncuestaEstadisticaToExcel()
        {
            if (PocentajeEncuestaEstadistica == null || PocentajeEncuestaEstadistica.Elements.Count == 0)
                return HttpNotFound("No hay datos para mostrar");
            if (PocentajeEncuestaEstadistica.Filter.IsAnnual)
            {
                var result = PocentajeEncuestaEstadistica.Elements.Select(t => new ExportExcelPorcentajeEstadisticaAnualModel()
                {
                    Analist = t.Analista,
                    Enero = t.Month.Where(h => h.Number == 1).Sum(h => h.PercentRound),
                    Febrero = t.Month.Where(h => h.Number == 2).Sum(h => h.PercentRound),
                    Marzo = t.Month.Where(h => h.Number == 3).Sum(h => h.PercentRound),
                    Abril = t.Month.Where(h => h.Number == 4).Sum(h => h.PercentRound),
                    Mayo = t.Month.Where(h => h.Number == 5).Sum(h => h.PercentRound),
                    Junio = t.Month.Where(h => h.Number == 6).Sum(h => h.PercentRound),
                    Julio = t.Month.Where(h => h.Number == 7).Sum(h => h.PercentRound),
                    Agosto = t.Month.Where(h => h.Number == 8).Sum(h => h.PercentRound),
                    Septiembre = t.Month.Where(h => h.Number == 9).Sum(h => h.PercentRound),
                    Octubre = t.Month.Where(h => h.Number == 10).Sum(h => h.PercentRound),
                    Noviembre = t.Month.Where(h => h.Number == 11).Sum(h => h.PercentRound),
                    Diciembre = t.Month.Where(h => h.Number == 12).Sum(h => h.PercentRound),
                }).ToList();
                return ExportExcel(result, "Detalles", "Porcentaje Encuestas Estadisticas");
            }
            var result2 = PocentajeEncuestaEstadistica.Elements.Select(t => new ExportExcelPorcentajeEstadisticaMensualModel()
            {
                Analist = t.Analista,
                _1 = t.Month.Where(h => h.Number == 1).Sum(h => h.PercentRound),
                _2 = t.Month.Where(h => h.Number == 2).Sum(h => h.PercentRound),
                _3 = t.Month.Where(h => h.Number == 3).Sum(h => h.PercentRound),
                _4 = t.Month.Where(h => h.Number == 4).Sum(h => h.PercentRound),
                _5 = t.Month.Where(h => h.Number == 5).Sum(h => h.PercentRound),
                _6 = t.Month.Where(h => h.Number == 6).Sum(h => h.PercentRound),
                _7 = t.Month.Where(h => h.Number == 7).Sum(h => h.PercentRound),
                _8 = t.Month.Where(h => h.Number == 8).Sum(h => h.PercentRound),
                _9 = t.Month.Where(h => h.Number == 9).Sum(h => h.PercentRound),
                _10 = t.Month.Where(h => h.Number == 10).Sum(h => h.PercentRound),
                _11 = t.Month.Where(h => h.Number == 11).Sum(h => h.PercentRound),
                _12 = t.Month.Where(h => h.Number == 12).Sum(h => h.PercentRound),
                _13 = t.Month.Where(h => h.Number == 13).Sum(h => h.PercentRound),
                _14 = t.Month.Where(h => h.Number == 14).Sum(h => h.PercentRound),
                _15 = t.Month.Where(h => h.Number == 15).Sum(h => h.PercentRound),
                _16 = t.Month.Where(h => h.Number == 16).Sum(h => h.PercentRound),
                _17 = t.Month.Where(h => h.Number == 17).Sum(h => h.PercentRound),
                _18 = t.Month.Where(h => h.Number == 18).Sum(h => h.PercentRound),
                _19 = t.Month.Where(h => h.Number == 19).Sum(h => h.PercentRound),
                _20 = t.Month.Where(h => h.Number == 20).Sum(h => h.PercentRound),
                _21 = t.Month.Where(h => h.Number == 21).Sum(h => h.PercentRound),
                _22 = t.Month.Where(h => h.Number == 22).Sum(h => h.PercentRound),
                _23 = t.Month.Where(h => h.Number == 23).Sum(h => h.PercentRound),
                _24 = t.Month.Where(h => h.Number == 24).Sum(h => h.PercentRound),
                _25 = t.Month.Where(h => h.Number == 25).Sum(h => h.PercentRound),
                _26 = t.Month.Where(h => h.Number == 26).Sum(h => h.PercentRound),
                _27 = t.Month.Where(h => h.Number == 27).Sum(h => h.PercentRound),
                _28 = t.Month.Where(h => h.Number == 28).Sum(h => h.PercentRound),
                _29 = t.Month.Where(h => h.Number == 29).Sum(h => h.PercentRound),
                _30 = t.Month.Where(h => h.Number == 30).Sum(h => h.PercentRound),
                _31 = t.Month.Where(h => h.Number == 31).Sum(h => h.PercentRound),
            }).ToList();
            return ExportExcel(result2, "Detalles", "Porcentaje Encuestas Estadisticas");


        }

        public ActionResult GeneralEncuestaEstadistica() 
        {
            return View("GeneralEncuestasEstadisticas/Index");
        }
        
	}
}