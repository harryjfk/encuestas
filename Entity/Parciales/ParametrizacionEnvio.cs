using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public partial class ParametrizacionEnvio
    {
        public bool Activado
        {
            get { return estado == 1; }
            set { estado = value ? 1 : 0; }
        }

        public List<string> Frecuencia()
        {
           
                return frecuencia != null ? frecuencia.Split(' ').ToList() : new List<string>();
           
            
        }

        public Dictionary<string, bool> FrecuenciaDic()
        {
          
                var res = new Dictionary<string, bool>
               {
                   {"LUN", false},
                   {"MAR", false},
                   {"MIE", false},
                   {"JUE", false},
                   {"VIE", false},
               };
                var list = Frecuencia();
                foreach (var day in list.Where(res.ContainsKey))
                    res[day] = true;
                return res;
            }
           
        

    }
}
