using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
   public partial class UsuarioExtranet:Usuario
    {
       public string Ruc { get; set; }
       public string Email { get; set; }

       public bool Seleccionado { get; set; }

       
    }
}
