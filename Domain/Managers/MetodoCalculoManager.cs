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
    public class MetodoCalculoManager:GenericManager<MetodoCalculo>
    {
        public MetodoCalculoManager(GenericRepository<MetodoCalculo> repository, Manager manager)
            : base(repository, manager)
        {
        }

        public MetodoCalculoManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }

        public override List<string> Validate(MetodoCalculo element)
        {
            var list= base.Validate(element);
            list.Required(element,t=>t.nombre,"Nombre");
            list.MaxLength(element,t=>t.nombre,20,"Nombre");
            return list;
        }

        public void Generate()
        {
            var vp = Get(t => t.nombre.Equals("Volumen Producción")).FirstOrDefault();
            if (vp == null)
            {
                vp = new MetodoCalculo()
                {
                    nombre = "Volumen Producción",
                    Activado = true,
                    RegistroObligatorio = true,

                };
                Add(vp);
                SaveChanges();
            }
            var vd = Get(t => t.nombre.Equals("VD-IIP")).FirstOrDefault();
            if (vd == null)
            {
                vd = new MetodoCalculo()
                {
                    nombre = "VD-IIP",
                    Activado = true,
                    RegistroObligatorio = true
                };
                Add(vd);
                SaveChanges();
            }
            var ca = Get(t => t.nombre.Equals("Consumo Aparente")).FirstOrDefault();
            if (ca == null)
            {
                ca = new MetodoCalculo()
                {
                    nombre = "Consumo Aparente",
                    Activado = true,
                    RegistroObligatorio = true
                };
                Add(ca);
                SaveChanges();
            }
        }
    }
}
