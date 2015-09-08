using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class EncuestaEstadisticaUploadModel
    {
        public String IdInternoEstablecimiento { get; set; }
        public DateTime Fecha { get; set; }
        public String CodigoCIIU { get; set; }
        public String CodigoLineaProducto { get; set; }
        public String AbreviaturaUM { get; set; }
        public Nullable<decimal> ValorUnitario { get; set; }
        public Nullable<decimal> Existencia { get; set; }
        public Nullable<decimal> Produccion { get; set; }
        public Nullable<decimal> VentasPais { get; set; }
        public Nullable<decimal> VentasExtranjero { get; set; }
        public Nullable<decimal> OtrasSalidas { get; set; }
    }
}