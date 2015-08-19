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
    public class ImportacionHarinaTrigoManager:GenericManager<ImportacionHarinaTrigo>
    {
        public ImportacionHarinaTrigoManager(GenericRepository<ImportacionHarinaTrigo> repository, Manager manager)
            : base(repository, manager)
        {
        }

        public ImportacionHarinaTrigoManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }

        public override OperationResult<ImportacionHarinaTrigo> Modify(ImportacionHarinaTrigo element, params string[] properties)
        {
            var res = base.Modify(element, properties);
            CalcularFobSoles(element.Id);
            return res;
        }
        public override List<string> Validate(ImportacionHarinaTrigo element)
        {
            var list= base.Validate(element);
            list.RequiredAndNotZero(element, t => t.cif_usd, "CIF USD");
            return list;
        }
        public void Generate(int año)
        {
            for (int i = 1; i < 13; i++)
            {
                var element = Get(t => t.fecha.Month == i && t.fecha.Year == año).FirstOrDefault();
                if (element == null)
                {
                    element = new ImportacionHarinaTrigo()
                    {
                        fecha = new DateTime(año, i, 1),
                        Activado = true
                    };
                    Add(element);

                }
                SaveChanges();
            }
        }

        public decimal CalcularFobSoles(long id)
        {
            var element = Find(id);
            if (element == null) return 0;
            var tipocambio = Manager.TipoCambioManager.Get(t => t.fecha.Year == element.fecha.Year && t.fecha.Month == element.fecha.Month).FirstOrDefault();
            if (tipocambio == null || tipocambio.tipo_cambio_compra == 0) return 0;
            var result = tipocambio.tipo_cambio_compra * element.cif_usd;
            element.cif_s = result;
            base.Modify(element);
            SaveChanges();
            return result;
        }
        public decimal CalcularFobSoles(long id, decimal value)
        {
            var element = Find(id);
            if (element == null) return 0;
            var tipocambio = Manager.TipoCambioManager.Get(t => t.fecha.Year == element.fecha.Year && t.fecha.Month == element.fecha.Month).FirstOrDefault();
            if (tipocambio == null || tipocambio.tipo_cambio_compra == 0) return 0;
            var result = tipocambio.tipo_cambio_compra * value;
            return result;
        }
        public decimal GetTipoCambioVenta(long id)
        {
            var element = Find(id);
            if (element == null) return 0;
            var tipocambio = Manager.TipoCambioManager.Get(t => t.fecha.Year == element.fecha.Year && t.fecha.Month == element.fecha.Month).FirstOrDefault();
            if (tipocambio == null || tipocambio.tipo_cambio_compra == 0) return 0;
            var result = tipocambio.tipo_cambio_compra;
            return result;
        }
    }
}
