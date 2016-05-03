using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Entity.Parciales
{
    public enum CiiuRevision
    {
        [Display(Name = "REV. 3")]
        Revision_3 = 3,

        [Display(Name = "REV. 4")]
        Revision_4 = 4
    }
}
