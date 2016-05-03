using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Reportes
{
    public class IndiceValorFisicoProducido
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public long IdCiiu { get; set; }
        public string Ciiu { get; set; }
        public List<IndiceValorFisicoProducidoItem> Elements { get; set; }
        public List<string> Header { get; set; }

        public IndiceValorFisicoProducido()
        {
            Elements = new List<IndiceValorFisicoProducidoItem>();
            Header = new List<string>();
        }
    }
}
