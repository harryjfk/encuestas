using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class ServiceModel
    {
        public long Id { get; set; }
        public decimal VentaPais { get; set; }
        public decimal VentaExtranjero { get; set; }
    }
}
