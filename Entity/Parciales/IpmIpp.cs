using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public partial class IpmIpp
    {
        public bool Activado
        {
            get { return estado == 1; }
            set { estado = value ? 1 : 0; }
        }

        public string Año
        {
            get;
            set;
        }
        public Func<TipoCambio, bool> BuildFilter()
        {
            if (!string.IsNullOrEmpty(Año) && !string.IsNullOrWhiteSpace(Año))
            {
                return t => t.fecha.Year.ToString() == Año;
            }
            return null;
        }


    }
}
