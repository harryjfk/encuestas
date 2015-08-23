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
    public class IpmIppManager : GenericManager<IpmIpp>
    {
        public IpmIppManager(GenericRepository<IpmIpp> repository, Manager manager)
            : base(repository, manager)
        {
        }

        public IpmIppManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }
        public override List<string> Validate(IpmIpp element)
        {
            var list = base.Validate(element);
            if (element.Id == 0) return list;
            list.RequiredAndNotZero(element, t => t.ipm, "IPM");
            //list.Required(element, t => t.ipp, "IPP");
            return list;
        }

        public void Generate(long idCiuu, int año)
        {
            for (int i = 1; i < 13; i++)
            {
                var element = Get(t => t.fecha.Month == i && t.fecha.Year == año && t.id_ciiu == idCiuu).FirstOrDefault();
                if (element == null)
                {
                    element = new IpmIpp()
                    {
                        fecha = new DateTime(año, i, 1),
                        Activado = true,
                        id_ciiu = idCiuu
                    };
                    Add(element);
                }
                SaveChanges();
            }


        }
    }
}
