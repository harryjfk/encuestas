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
                IsAnnual = false,
                Year = DateTime.Now.Year,
                Estado = EstadoEncuesta.Todos,
                Month = 7,
                From = 1,
                To = 31,
            };
            //var report = new PorcentajeEncuestaEstadistica()
            //{
            //    Filter = filter
            //};
            var report = Manager.EncuestaEstadistica.PorcentajeEncuestaEstadistica(filter);
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
            };
            var report = Manager.EncuestaEstadistica.PorcentajeEncuestaEstadistica(filter);
            return View("PorcentajeEncuestaEstadistica/Index",report);
        }
	}
}