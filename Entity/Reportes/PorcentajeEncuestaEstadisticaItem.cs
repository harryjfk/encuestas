using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity.Reportes
{
    public class PorcentajeEncuestaEstadisticaItem
    {
        public string Analista { get; set; }
        public long IdAnalista { get; set; }
        public IList<MonthData> Month { get; set; }
        public IList<CiiuData> Ciius { get; set; }

       
    }

    public class MonthData
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public int Value { get; set; }
    }

    public class CiiuData
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public IList<MonthData> Month { get; set; }
    }


}
