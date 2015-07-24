using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public partial class LineaProductoEstablecimiento
    {
        public override string ToString()
        {
            return string.Format("{0}-{1}", "", "");
        }

        public bool IsNew(DateTime now)
        {

            if (fecha_creacion_informante == null) return false;
            var fecha = fecha_creacion_informante.GetValueOrDefault();
            return now.Year == fecha.Year && now.Month == fecha.Month;

        }
    }
}
