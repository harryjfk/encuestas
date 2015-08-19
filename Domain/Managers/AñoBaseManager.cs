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
    public class AñoBaseManager:GenericManager<AñoBase>
    {
        public AñoBaseManager(GenericRepository<AñoBase> repository, Manager manager) : base(repository, manager)
        {
        }

        public AñoBaseManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }

      

        public override List<string> Validate(AñoBase element)
        {
            var list= base.Validate(element);
            list.RequiredAndNotZero(element,t=>t.id_ciiu,"CIIU");
            list.RequiredAndNotZero(element, t => t.id_establecimiento, "Establecimiento");
            list.RequiredAndNotZero(element, t => t.id_linea_producto, "Línea de producto");
            list.RequiredAndNotZero(element, t => t.id_unidad_medida, "Unidad de Medida");
            list.RequiredAndNotZero(element, t => t.produccion_anual, "Produccón Anual");
            list.RequiredAndNotZero(element, t => t.valor_produccion, "Valor de Produccón");
            list.RequiredAndNotZero(element, t => t.precio, "Precio");
            return list;
        }
    }
}
