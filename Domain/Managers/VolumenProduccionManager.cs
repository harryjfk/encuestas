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
    public class VolumenProduccionManager:GenericManager<VolumenProduccion>
    {
        public VolumenProduccionManager(GenericRepository<VolumenProduccion> repository, Manager manager) : base(repository, manager)
        {
        }
        public VolumenProduccionManager(Entities context, Manager manager) : base(context, manager)
        {
        }

        public override List<string> Validate(VolumenProduccion element)
        {
            var list= base.Validate(element);
            return list;
        }
    }
}
