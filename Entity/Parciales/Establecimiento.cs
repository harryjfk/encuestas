using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
   public partial class Establecimiento
    {
       public bool Activado
       {
           get { return Estado == 1; }
           set { Estado = value ? 1 : 0; }
       }

       public bool IsNew
       {
           get
           {
               try
               {
                   return Encuestas.OfType<EncuestaEstadistica>().Count() < 2;
               }
               catch (Exception)
               {
                   return false;
               }
               //if (creado == null) return false;
               //var date = creado.GetValueOrDefault();
               //var now = DateTime.Now;
               //return now.Year == date.Year && now.Month == date.Month;
           }
       }

       public Contacto ContactoTemporal { get; set; }
       public Contacto ContactoPredeterminado
       {
           get
           {
               return Contactos.FirstOrDefault(t => t.Activado);
           }
           set { }
           
       }

       public override string ToString()
       {
           return string.Format("{0}", Nombre);
       }

       public string IdDepartamento
       {
           get
           {
               return Ubigeo != null ? Ubigeo.Take(2).Aggregate("", (t, h) => t + h) : null;
           }
           set
           {
               if (Ubigeo != null)
                   Ubigeo = value +IdProvincia+IdDistrito;
               else
               {
                   Ubigeo = value + "0000";
               }
           }
       }
       public string IdProvincia
       {
           get
           {
               return Ubigeo != null ? Ubigeo.Take(4).Aggregate("", (t, h) => t + h) : null;
           }
           set
           {
               if (Ubigeo != null)
                   Ubigeo = value +IdDistrito.Skip(4).Take(4).Aggregate("", (t, h) => t + h);
               else
               {
                   Ubigeo = value + "00";
               }
           }
       }
       public string IdDistrito
       {
           get
           {
               return Ubigeo;
           }
           set
           {
               if (Ubigeo != null)
                   Ubigeo =  value;
               else
               {
                   Ubigeo = value;
               }
           }
       }

       public TipoEstablecimiento TipoEnum
       {
           get
           {
               return (TipoEstablecimiento) Enum.Parse(typeof (TipoEstablecimiento), tipo_establecimiento.GetValueOrDefault().ToString());
           }
           set
           {
               tipo_establecimiento = (int) value;
           }
       }
       public string CiiuText { get; set; }

       public Func<Establecimiento, bool> BuildFilter()
       {
           if (!string.IsNullOrEmpty(CiiuText) && !string.IsNullOrWhiteSpace(CiiuText))
           {
               var temp = CiiuText.ToLower();
               return t => t.Ciius.Any(h => h.Codigo.ToLower().Contains(temp) 
                   || h.Nombre.ToLower().Contains(temp));
           }

           return null;
       }
    }

    public enum TipoEstablecimiento
    {
        Servicio=0,
        ServicioProducto=1
    }
}
