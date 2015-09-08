using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class ExportGeneralEncuestaEstadistica
    {
        public int Year { get; set; }
        public IEnumerable<int> Month { get; set; }
        public IEnumerable<int> Values { get; set; }
        public string Output { get; set; }
        public string InputTypes { get; set; }
        public List<string> ScopeElements { get; set; }


    }
}