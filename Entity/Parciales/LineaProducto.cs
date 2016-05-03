using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
   public partial class LineaProducto
    {
       public bool Activado
       {
           get { return Estado == 1; }
           set { Estado = value ? 1 : 0; }
       }

       public override string ToString()
       {
          return String.Format("{0} - {1}", Codigo, Nombre);                     
       }

        public Func<LineaProducto, bool> BuildFilter()
        {
            return t => t.Codigo.Length == 7;
        }
    }
}
