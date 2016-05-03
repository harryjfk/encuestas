using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Reportes
{
    public class GraphicModel
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string YTitle { get; set; }
        public Dictionary<string, double> Serie { get; set; }
    }
}
