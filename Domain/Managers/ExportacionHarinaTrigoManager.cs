﻿using System;
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
    public class ExportacionHarinaTrigoManager:GenericManager<ExportacionHarinaTrigo>
    {
        public ExportacionHarinaTrigoManager(GenericRepository<ExportacionHarinaTrigo> repository, Manager manager)
            : base(repository, manager)
        {
        }

        public ExportacionHarinaTrigoManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }
        public override List<string> Validate(ExportacionHarinaTrigo element)
        {
            var list= base.Validate(element);
            list.Required(element,t=>t.fob_s,"FOB S/.");
            list.MaxLength(element,t=>t.fob_usd,50,"FOB USD");
            return list;
        }
    }
}