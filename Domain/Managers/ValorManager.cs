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
    public class ValorManager:GenericManager<Valor>
    {
        public ValorManager(GenericRepository<Valor> repository, Manager manager) : base(repository, manager)
        {
        }

        public ValorManager(Entities context, Manager manager) : base(context, manager)
        {
        }

        public override List<string> Validate(Valor element)
        {
            var list= base.Validate(element);
            list.Required(element,t=>t.Texto,"Texto");
            list.Required(element,t=>t.IdPosibleRespuesta,"PosibleRespuesta");
            list.MaxLength(element,t=>t.Texto,1000,"Texto");
            return list;
        }
    }
}
