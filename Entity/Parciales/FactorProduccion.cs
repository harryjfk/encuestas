using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public partial class FactorProducccion
    {
        public bool IncrementoB
        {
            get
            {
                return Incremento == 1;
            }
            set
            {
                Incremento = (short)(value ? 1 : 0);
                ProduccionNormal = 0;
                Decrecimiento = 0;
            }
        }
        public bool DecrecimientoB
        {
            get
            {
                return Decrecimiento == 1;
            }
            set
            {
                Decrecimiento = (short)(value ? 1 : 0);
                Incremento = 0;
                ProduccionNormal = 0;
            }
        }
        public bool ProduccionNormalB
        {
            get
            {
                return ProduccionNormal == 1;
            }
            set
            {
                ProduccionNormal = (short)(value ? 1 : 0);
                Incremento = 0;
                Decrecimiento = 0;
            }
        }
    }
}
