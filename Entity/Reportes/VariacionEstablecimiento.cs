using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Reportes
{
    public class VariacionEstablecimientoFilter
    {
        public int Year { get; set; }
        public int Month { get; set; }        
    }

    public class VariacionEstablecimiento
    {
        public int Level { get; set; }
        public string Descripcion { get; set; }
        public string Enero { get; set; }
        public string Febrero { get; set; }
        public string Marzo { get; set; }
        public string Abril { get; set; }
        public string Mayo { get; set; }
        public string Junio { get; set; }
        public string Julio { get; set; }
        public string Agosto { get; set; }
        public string Setiembre { get; set; }
        public string Octubre { get; set; }
        public string Noviembre { get; set; }
        public string Diciembre { get; set; }
    }
}
