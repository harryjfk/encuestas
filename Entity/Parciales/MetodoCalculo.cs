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
       public bool RegistroObligatorio
       {
           get { return registro_obligatorio == 1; }
           set { registro_obligatorio = value ? 1 : 0; }
       }

       public bool PuedeElimiarse
       {
           get
           {
               return !RegistroObligatorio && !CAT_CIIU.Any();
           }
       }

       public override string ToString()
       {
           return nombre;
       }
    }
}
