using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Parciales;

namespace Entity
{
   public partial class Ciiu
   {
       public bool Activado
       {
           get { return Estado == 1; }
           set { Estado = value ? 1 : 0; }
       }

       public bool Seleccionado { get; set; }


       public EnumSubsector EnumSubSector
       {
           get
           {
               return (EnumSubsector)Enum.Parse(typeof(EnumSubsector), sub_sector.ToString());
           }
           set
           {
               sub_sector = (long)value;
           }
       }

        public CiiuRevision EnumRevision
        {
            get
            {
                return (CiiuRevision)Enum.Parse(typeof(CiiuRevision), Revision.ToString());
            }
            set
            {
                Revision = (decimal)value;
            }
        }

       public EnumRubro EnumRubro
       {
           get
           {
               return (EnumRubro)Enum.Parse(typeof(EnumSubsector), rubro.ToString());
           }
           set
           {
               rubro = (long)value;
           }
       }

       public override string ToString()
       {
           var subName = Nombre;
           if (subName.Length > 100)
           {
               subName = Nombre.Substring(0, 97)+"...";
           }
           return String.Format("{0}-{1}",Codigo,subName);
       }

        // public double Peso { get; set; }
        public double ValorAgregado { get; set; }
    }
}
