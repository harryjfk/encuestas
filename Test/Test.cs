using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Domain.Managers;

namespace Test
{
    public static class Test
    {
        public delegate void Tt(int x);

        public static event Tt Update;
        private static int cont;
        public static string Calculate(this string text)
        {
            Update(++cont);
            return text.Replace(" ", "");
        }



        public static int Otro(this IEnumerable<Persona> number)
        {
            
            var xx = number.Where(t => t.Edad > 50);
            var reg = new Regex(".*");

            var reee = number.Select(t => new {t.Nombre,Edad = 58}).ToList();
            //var query = from f in number where f > 5  select f;
            //var lista = query.ToList();
            return 2;
        }


    }

    public class Persona:IComparable<Persona>
    {
        public int Edad { get; set; }
        public string Nombre { get; set; }

        public int CompareTo(Persona other)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }


        
    }
}
