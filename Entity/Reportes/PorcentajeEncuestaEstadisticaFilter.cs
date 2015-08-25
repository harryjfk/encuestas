using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Reportes
{
    public class PorcentajeEncuestaEstadisticaFilter
    {
        public bool IsAnnual { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public long? IdAnalista { get; set; }
        public EstadoEncuesta Estado { get; set; }
    }
}
