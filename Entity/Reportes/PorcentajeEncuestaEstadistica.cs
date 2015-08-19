using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Reportes
{
    public class PorcentajeEncuestaEstadistica
    {
        public List<PorcentajeEncuestaEstadisticaItem> Elements { get; set; }

        public PorcentajeEncuestaEstadistica()
        {
            Elements = new List<PorcentajeEncuestaEstadisticaItem>();
        }
    }
}
