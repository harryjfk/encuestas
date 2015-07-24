using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Seguridad;
using Entity;

namespace Domain.Trazas
{
    public class RegistroTrazaDb : IRegistro
    {
        public void Escribir(ITraza mensaje)
        {
            //var nuevo = new Traza()
            //{
            //    Fecha = mensaje.Fecha,
            //    Texto = mensaje.Texto,
            //    Tipo = mensaje.Tipo
            //};
            throw new NotImplementedException();
        }

        public IEnumerable<ITraza> Leer(Func<ITraza, bool> filter)
        {
            throw new NotImplementedException();
        }
    }
}
