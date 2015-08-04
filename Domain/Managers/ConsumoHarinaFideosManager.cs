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
        public void Generate(int año)
        {
            for (int i = 1; i < 13; i++)
            {
                var element = Get(t => t.fecha.Month == i && t.fecha.Year == año).FirstOrDefault();
                if (element == null)
                {
                    element = new ConsumoHarinaFideo() { 
                    fecha=new DateTime(año,i,1),
                    Activado=true
                    };
                    Add(element);

                }
                SaveChanges();
            }
        }
    }
}
