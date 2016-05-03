using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
   public partial class UsuarioIntranet:Usuario
    {
       public string CodigoDepartamento { get; set; }
       public string CodigoDistrito { get; set; }
       public string CodigoProvincia { get; set; }
       public string DNI { get; set; }
       public string Telefono { get; set; }
       public int IdRol { get; set; }

       public string Ubigeo
       {
           get
           {
               return string.Format("{0}{1}{2}", CodigoDepartamento, CodigoProvincia, CodigoDistrito);
           }
       }

       public bool IsAdministrador
       {
           get
           {
               return Roles.Any(t => t.Nombre == "Administrador");
           }
       }
    }
}
