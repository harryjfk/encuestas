using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class ExportExcelPorcentajeEstadisticaAnualModel
    {
        public string Analista { get; set; }
        public double Enero { get; set; }
        public double Febrero { get; set; }
        public double Marzo { get; set; }
        public double Abril { get; set; }
        public double Mayo { get; set; }
        public double Junio { get; set; }
        public double Julio { get; set; }
        public double Agosto { get; set; }
        public double Septiembre { get; set; }
        public double Octubre { get; set; }
        public double Noviembre { get; set; }
        public double Diciembre { get; set; }
    }

    public class ExportExcelIndiceValorFijoModel
    {
        public string Ciiu { get; set; }
        public string Establecimiento { get; set; }
        public double Ponderacion { get; set; }
        public double Enero { get; set; }
        public double Febrero { get; set; }
        public double Marzo { get; set; }
        public double Abril { get; set; }
        public double Mayo { get; set; }
        public double Junio { get; set; }
        public double Julio { get; set; }
        public double Agosto { get; set; }
        public double Septiembre { get; set; }
        public double Octubre { get; set; }
        public double Noviembre { get; set; }
        public double Diciembre { get; set; }
    }
}