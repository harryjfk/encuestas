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
    public class ImportacionHarinaTrigoManager:GenericManager<ImportacionHarinaTrigo>
    {
        public ImportacionHarinaTrigoManager(GenericRepository<ImportacionHarinaTrigo> repository, Manager manager)
            : base(repository, manager)
        {
        }

        public ImportacionHarinaTrigoManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }
        public override List<string> Validate(ImportacionHarinaTrigo element)
        {
            var list= base.Validate(element);
            list.Required(element,t=>t.cif_s,"CIF S/.");
            list.MaxLength(element,t=>t.cif_usd,50,"CIF USD");
            return list;
        }
    }
}
