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
    public class FactorProducccionManager:GenericManager<FactorProducccion>
    {
        public FactorProducccionManager(GenericRepository<FactorProducccion> repository, Manager manager) : base(repository, manager)
        {
        }

        public FactorProducccionManager(Entities context, Manager manager) : base(context, manager)
        {
        }
    }
}
