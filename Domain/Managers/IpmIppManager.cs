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
    public class IpmIppManager:GenericManager<IpmIpp>
    {
        public IpmIppManager(GenericRepository<IpmIpp> repository, Manager manager) : base(repository, manager)
        {
        }

        public IpmIppManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }
        public override List<string> Validate(IpmIpp element)
        {
            var list= base.Validate(element);
            list.Required(element, t => t.ipm, "IPM");
            list.Required(element, t => t.ipp, "IPP");
            return list;
        }
    }
}
