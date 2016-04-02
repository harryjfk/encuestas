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

       public Func<AñoBase, bool> BuildFilter()
       {
           if (id_establecimiento>0)
           {
               return t => t.id_establecimiento == id_establecimiento;
           }
           if (id_ciiu > 0)
           {
               return t => t.id_ciiu == id_ciiu;
           }
           return null;
       }
    }
}
