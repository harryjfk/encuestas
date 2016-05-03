using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity.Reportes
{
   public  class IndiceValorFisicoProducidoItem
    {
       public long IdCiiu { get; set; }
       public string CodigoCiiu { get; set; }
       public string Ciiu { get; set; }
       public double Ponderacion { get; set; }
       public long IdEstablecimiento { get; set; }
       public string Establecimiento { get; set; }
       public List<IndiceValorFisicoProducidoValue> Values { get; set; }

       public IndiceValorFisicoProducidoItem()
       {
           Values = new List<IndiceValorFisicoProducidoValue>();
       }
    }

    public class IndiceValorFisicoProducidoValue
    {
        public string Header { get; set; }
        public double Value { get; set; }
        public int Index { get; set; }
    }
}
