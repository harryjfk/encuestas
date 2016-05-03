using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity
{
    public class Order<T>
    {
        public Func<T,Object> Func { get; set; }
        public bool Descending { get; set; }

    }
}
