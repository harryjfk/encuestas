using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
   public partial class Factor
    {
       public TipoFactor TipoFactor
       {
           get
           {
               return (TipoFactor)Enum.Parse(typeof (TipoFactor), Tipo.ToString());
           }
           set
           {
               Tipo = (int) value;
           }
       }
       public bool Activado
       {
           get { return Estado == 1; }
           set { Estado = value ? 1 : 0; }
       }
    }
}
