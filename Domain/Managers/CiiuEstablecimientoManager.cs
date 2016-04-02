using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositorios;
using Entity;

namespace Domain.Managers
{
    public class CiiuEstablecimientoManager : GenericManager<CiiuEstablecimiento>
    {
        public CiiuEstablecimientoManager(GenericRepository<CiiuEstablecimiento> repository, Manager manager) : base(repository, manager)
        {
        }

        public CiiuEstablecimientoManager(Entities context, Manager manager) : base(context, manager)
        {
        }
    }
}
