using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Paginacion
    {
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }
        public int Count { get; set; }
        public int PageCount
        {
            get
            {
                var calc = Count/(ItemsPerPage*1.0);
                calc = Math.Ceiling(calc);
                return (int)Math.Max(1, calc);
            }
        }
        public Paginacion Validate()
        {
            if (Page < 1) Page = 1;
            if (ItemsPerPage < 1) ItemsPerPage = 15;
            //if (Page > PageCount && PageCount>0) Page = PageCount;
            return this;
        }
    }
}
