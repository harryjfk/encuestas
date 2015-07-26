using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
   public partial class MetodoCalculo
    {
       public bool Activado
       {
           get { return estado == 1; }
           set { estado = value ? 1 : 0; }
       }

       public override string ToString()
       {
           return string.Format("{0}", nombre);
       }
    }
}
