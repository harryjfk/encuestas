using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Reportes
{
    public class TestEntity
    {
        public string IdentificadorEstablecimiento { get; set; }
        public string Establecimiento { get; set; }
    }

    public class EmpresaEnvioInformacion
    {
        public string IdentificadorEstablecimiento { get; set; }
        public string Establecimiento { get; set; }
        public string CIIU { get; set; }
        public string Analista { get; set; }
        public int EsCalculo { get; set; }
    }

    public class EmpresaEnvioInformacionFilter
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int IdAnalista { get; set; }
        public int IdCiiu { get; set; }
    }
}
