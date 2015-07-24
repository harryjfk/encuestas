using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
   public partial class Valor
   {
       public bool Seleccionado
       {
           get { return Estado == 1; }
           set { Estado = value ? 1 : 0; }
       }

       public override string ToString()
       {
           
           var subName = Texto;
           if (subName.Length > 30)
           {
               subName = Texto.Substring(0, 27) + "...";
           }
           return String.Format("{0}", subName);
       }
   }
}
