using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Reportes
{
    public class IndiceVariacion
    {
        public string IsVariacionStr { get; set; }
        public int Year { get; set; }
        public long IdInformante { get; set; }
        public bool IsVariacion { get; set; }
        public List<IniceElement> Indice { get; set; }
        public List<HeaderValue> Header { get; set; }

        public IndiceVariacion()
        {
            Indice = new List<IniceElement>();
            Header=new  List<HeaderValue>();
        }
    }

    public class HeaderValue
    {
        public string Text { get; set; }
        public int Number { get; set; }
    }

    public class IniceElement
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }
        public List<MonthValue> Values { get; set; }

        public bool IsCiiuInformante { get; set; }

        public IniceElement()
        {
            Values=new List<MonthValue>();
        }
    }

    public class MonthValue
    {
        public string Text { get; set; }
        public int Number { get; set; }
        public double Value { get; set; }
    }
}
