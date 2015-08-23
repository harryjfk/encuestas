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
        public List<MonthData> Month { get; set; }
        public List<CiiuData> Ciius { get; set; }
        public int Total { get; set; }

        public PorcentajeEncuestaEstadisticaItem()
        {
            Month = new List<MonthData>();
            Ciius = new List<CiiuData>();
        }

    }

    public class MonthData
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public int MonthlyValue { get; set; }
        public int Total { get; set; }

        public double Percent
        {
            get { return MonthlyValue * 100.0 / Total; }
        }

        public double PercentRound
        {
            get
            {
                return Math.Round(Percent, 2);
            }
        }
    }

    public class CiiuData
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public IList<MonthData> Month { get; set; }

        public CiiuData()
        {
            Month = new List<MonthData>();
        }
    }


}
