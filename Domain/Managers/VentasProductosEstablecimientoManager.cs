using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositorios;
using Entity;

namespace Domain.Managers
{
    public class VentasProductosEstablecimientoManager:GenericManager<VentasProductosEstablecimientos>
    {
        public VentasProductosEstablecimientoManager(GenericRepository<VentasProductosEstablecimientos> repository, Manager manager) : base(repository, manager)
        {
        }

        public VentasProductosEstablecimientoManager(Entities context, Manager manager) : base(context, manager)
        {
        }
    }
}
