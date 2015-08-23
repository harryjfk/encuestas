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
    public class ViewProcentajeEncuestaExtadisticaManager:GenericManager<ViewProcentajeEncuestaExtadistica>
    {
        public ViewProcentajeEncuestaExtadisticaManager(GenericRepository<ViewProcentajeEncuestaExtadistica> repository, Manager manager) : base(repository, manager)
        {
        }

        public ViewProcentajeEncuestaExtadisticaManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }

       
    }
}
