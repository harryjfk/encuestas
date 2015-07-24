using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositorios;
using Entity;

namespace Domain.Managers
{
    public class FactorManager:GenericManager<Factor>
    {
        public FactorManager(GenericRepository<Factor> repository, Manager manager) : base(repository, manager)
        {
        }

        public FactorManager(Entities context, Manager manager) : base(context, manager)
        {
        }

        public override OperationResult<Factor> Delete(Factor element)
        {
            //if (element.FactoresProduccion.Count > 0)
               // return new OperationResult<Factor>(element) { Errors = new List<string>() { "Hay factores de producción relacionados" }, Success = false };
            return base.Delete(element);
        }

        public override List<string> Validate(Factor element)
        {
            var list= base.Validate(element);
            list.Required(element,t=>t.Nombre,"Nombre");
            list.Required(element,t=>t.Tipo,"Tipo");
            list.MaxLength(element,t=>t.Nombre,150,"Nombre");
            return list;
        }
    }
}
