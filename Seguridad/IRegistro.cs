using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Seguridad
{
    public interface IRegistro
    {
        void Escribir(ITraza mensaje);
        IEnumerable<ITraza> Leer(Func<ITraza,bool>filter );
    }
}
