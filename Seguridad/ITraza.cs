using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seguridad
{
    public interface ITraza
    {
         string Texto { get; set; }
         TipoTraza Tipo { get; set; }
         DateTime Fecha { get; set; }
    }
}
