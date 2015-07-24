using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
   public partial class Encuesta
   {
       public bool Activado
       {
           get { return Estado == 1; }
           set { Estado = value ? 1 : 0; }
       }

       public int? Year { get; set; }
       public int? Mes { get; set; }
       public EstadoEncuesta EstadoEncuesta
       {
           get
           {
               return (EstadoEncuesta) Enum.Parse(typeof (EstadoEncuesta), Estado.ToString());
           }
           set
           {
               Estado =(int) value;
           }
       }
       public Func<Encuesta, bool> BuildFilter()
       {
           if (Year != null)
           {
               if (Mes != null)
               {
                   return t => t.Fecha.Year == Year.GetValueOrDefault() && t.Fecha.Month == Mes.GetValueOrDefault();
               }
               else
               {
                   return t => t.Fecha.Year == Year.GetValueOrDefault();
               }
           }
           else
           {
               if (Mes != null)
               {
                   return t => t.Fecha.Month == Mes.GetValueOrDefault();
               }
               else
               {
                   return null;
               }
           }

           return null;
       }
   }
}
