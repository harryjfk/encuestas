using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public partial class VentasProductosEstablecimientos
    {
       public bool ServiciosActivados
       {
           get { return brindo_servicios == 1; }
           set { brindo_servicios = value ? (short)1 : (short)0; }
       }
    }
}
