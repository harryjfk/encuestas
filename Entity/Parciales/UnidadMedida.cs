using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
   public partial class UnidadMedida
    {
       public bool Activado
       {
           get { return Estado == 1; }
           set { Estado = value ? 1 : 0; }
       }

       public bool Asignado { get; set; }

       public override string ToString()
       {
           return string.Format("{0}", Abreviatura);
       }
    }
}
