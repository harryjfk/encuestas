using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;

namespace WebApplication.Models
{
    public class ExportGeneralEncuestaEstadistica
    {
        public int Year { get; set; }
        public IEnumerable<int> Month { get; set; }
        public IEnumerable<long> Values { get; set; }
        public IEnumerable<long> Establecimiento { get; set; }
        public string Output { get; set; }
        public string InputTypes { get; set; }
        public List<string> ScopeElements { get; set; }
        public IEnumerable<MateriaPropia> VolumenProduccionMateriaPropia { get; set; }
        public IEnumerable<MateriaTerceros> VolumenProduccionMateriaTerceros { get; set; }
        public IEnumerable<ValorProduccion> ValorProduccion { get; set; }
        public IEnumerable<VentasPaisExtranjero> VentasPaisExtranjeros { get; set; }
        public IEnumerable<TrabajadoresDiasTrabajados> TrabajadoresDiasTrabajadoses { get; set; }
    }
}