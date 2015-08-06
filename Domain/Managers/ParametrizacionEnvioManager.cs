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
    public class ParametrizacionEnvioManager:GenericManager<ParametrizacionEnvio>
    {
        public ParametrizacionEnvioManager(GenericRepository<ParametrizacionEnvio> repository, Manager manager) : base(repository, manager)
        {
        }

        public ParametrizacionEnvioManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }

        public void Generate()
        {
            var estadistica = Get(t => t.tipo_encuesta == "Estadistica").FirstOrDefault();
            var empresarial = Get(t => t.tipo_encuesta == "Empresarial").FirstOrDefault();
            if (estadistica == null)
            {
                estadistica=new ParametrizacionEnvio()
                {
                    tipo_encuesta = "Estadistica",
                    Activado = false,
                    comienzo = DateTime.Now,
                    envio_1 = 5,
                    envio_2 = 25,
                    mensaje = "Tiene encuestas estadísticas sin llenar"
                };
                Add(estadistica);
                SaveChanges();
            }
            if (empresarial == null)
            {
                empresarial = new ParametrizacionEnvio()
                {
                    tipo_encuesta = "Empresarial",
                    Activado = false,
                    comienzo = DateTime.Now,
                    envio_1 = 5,
                    envio_2 = 25,
                    mensaje = "Tiene encuestas de opinión empresarial sin llenar"
                };
                Add(empresarial);
                SaveChanges();
            }

        }

        public override List<string> Validate(ParametrizacionEnvio element)
        {
            var list= base.Validate(element);           
            list.Required(element, t => t.mensaje, "Mensaje");
            list.Required(element, t => t.comienzo, "Comienzo");
            list.Required(element, t => t.envio_1, "Envío 1");
            list.Required(element, t => t.envio_1, "Envío 2");
            list.MaxLength(element,t=>t.mensaje,500,"Mensaje");
            return list;
        }
        public override OperationResult<ParametrizacionEnvio> Modify(ParametrizacionEnvio element, params string[] properties)
        {
            return base.Modify(element, "tipo_encuesta");
        }
    }
}
