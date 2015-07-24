using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public partial class Usuario
    {
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string NombreApellidos { get; set; }

        public string Trabajador
        {
            get
            {
                return string.Format("{0} {1}", Nombres, Apellidos);
            }
        }

        public Func<Usuario, bool> BuildFilter()
        {
            if (string.IsNullOrEmpty(NombreApellidos) || string.IsNullOrWhiteSpace(NombreApellidos))
                return null;
            return t => (t.Nombres.ToLower() + " " + t.Apellidos.ToLower()).Contains(NombreApellidos.ToLower());
        }

        public override string ToString()
        {
            return string.Format("{0}",Nombres);
        }
    }
}
