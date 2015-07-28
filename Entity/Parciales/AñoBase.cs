using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
   public partial class AñoBase
    {
       public bool Activado
       {
           get { return estado == 1; }
           set { estado = value ? 1 : 0; }
       }

      
    }
}
