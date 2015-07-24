using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
   public partial class Ciiu
   {
       public bool Activado
       {
           get { return Estado == 1; }
           set { Estado = value ? 1 : 0; }
       }

       public bool Seleccionado { get; set; }

       public override string ToString()
       {
           var subName = Nombre;
           if (subName.Length > 1000)
           {
               subName = Nombre.Substring(0, 97)+"...";
           }
           return String.Format("{0}-{1}",Codigo,subName);
       }
   }
}
