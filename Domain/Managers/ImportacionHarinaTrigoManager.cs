﻿using System;
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
    }
}
