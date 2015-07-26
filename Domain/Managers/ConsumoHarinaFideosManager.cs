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
    public class ConsumoHarinaFideoManager : GenericManager<ConsumoHarinaFideo>
    {
        public ConsumoHarinaFideoManager(GenericRepository<ConsumoHarinaFideo> repository, Manager manager)
            : base(repository, manager)
        {
        }

        public ConsumoHarinaFideoManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }

        public override List<string> Validate(ConsumoHarinaFideo element)
        {
            var list= base.Validate(element);
            list.Required(element,t=>t.tonelada_tmb,"Toneladas TMB");
            return list;
        }
    }
}
