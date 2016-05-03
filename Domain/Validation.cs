using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain
{
    public static class Validation
    {
        public static void Required<T, TK>(this List<string> list, T element,Func<T, TK> property,string name)
        {
            var temp = new List<T>() {element};
            var value = temp.Select(property).FirstOrDefault();
            if (value == null || value.ToString().Trim()=="") list.Add(string.Format("El campo \"{0}\" es obligatorio", name));
        }
        public static void RequiredAndNotZero<T, TK>(this List<string> list, T element, Func<T, TK> property, string name)
        {
            var temp = new List<T>() { element };
            var value = temp.Select(property).FirstOrDefault();
            if (value == null)
            {
                list.Add(string.Format("El campo \"{0}\" es obligatorio", name));
                return;
            }
            double vTemp;
            if (double.TryParse(value.ToString(), out vTemp) && Math.Abs(vTemp) < 0.00001)
            {
                list.Add(string.Format("El campo \"{0}\" no puede ser cero", name));
                return;
            }


        }
        public static void PhoneNumber<T, TK>(this List<string> list, T element, Func<T, TK> property, string name)
        {
            var temp = new List<T>() { element };
            var value = temp.Select(property).FirstOrDefault();
            if (value == null) return;
            var text = value.ToString().Trim().Trim('+').Replace(" ","").Replace("(","").Replace(")","");
            if (text.All(char.IsDigit)) return;
            list.Add(string.Format("El campo \"{0}\" no es un número de telefono válido", name));
        }
        public static void Range<T, TK>(this List<string> list, T element, Func<T, TK> property,double min,double max, string name)
        {
            var temp = new List<T>() { element };
            var value = temp.Select(property).FirstOrDefault();
            if (value == null)
            {
                list.Add(string.Format("El campo \"{0}\" debe estar entre {1} y {2}", name,min,max));
                return;
            };
            double vTemp;
            if (!double.TryParse(value.ToString(), out vTemp) || vTemp>max || vTemp<min)
            {
                list.Add(string.Format("El campo \"{0}\" debe estar entre {1} y {2}", name, min, max));
                return;
            }
        }
        public static void Number<T, TK>(this List<string> list, T element, Func<T, TK> property, string name)
        {
            var temp = new List<T>() { element };
            var value = temp.Select(property).FirstOrDefault();
            if (value == null) return;
            var text = value.ToString();
            if (text.All(char.IsDigit)) return;
            list.Add(string.Format("El campo \"{0}\" no es un número válido", name));
        }
        public static void Email<T, TK>(this List<string> list, T element, Func<T, TK> property, string name)
        {
            var temp = new List<T>() { element };
            var value = temp.Select(property).FirstOrDefault();
            if (value == null) return;
            var text = value.ToString().Trim().Replace(" ", "");
            if (text.Contains("@")) return;
            list.Add(string.Format("El campo \"{0}\" no es un correo válido", name));
        }
        public static void MaxLength<T, TK>(this List<string> list, T element, Func<T, TK> property, int length,string name)
        {
            
            var temp = new List<T>() { element };
            var value = temp.Select(property).FirstOrDefault();
            if (value == null) return;
            var plength = value.ToString().Length;
            if(plength>length)
                list.Add(string.Format("El campo \"{0}\" tiene un máximo de {1} caracteres", name, length));
        }
        
        public static void ValidRUC<T, TK>(this List<string> list, T element, Func<T, TK> property, string name)
        {
            var temp = new List<T>() { element };
            var value = temp.Select(property).FirstOrDefault();
            if (value == null) return;
            var text = value.ToString().Trim().Replace(" ", "");
            if (text.Length != 11 || text.Any(t=>!char.IsDigit(t)))
            {
                list.Add(string.Format("El campo \"{0}\" no es un RUC válido", name));
                return;
            }
            var dic = new Dictionary<int, int>
            {
                { 0, 5 }, 
                { 1, 4 }, 
                { 2, 3 }, 
                { 3, 2 },
                { 4, 7 },
                { 5, 6 },
                { 6, 5 },
                { 7, 4 },
                { 8, 3 },
                { 9, 2 },
            };
            var result = 0;
            var last = int.Parse(text[text.Length - 1].ToString());
            for (int i = 0; i < 10; i++)
            {
                var ch = int.Parse(text[i].ToString());
                var val = dic[i];
                result+= val*ch;
            }
            var res = 11-result%11;
            res = res == 10 ? 0 : res;
            res = res == 11 ? 1 : res;
            if(last!=res)
                list.Add(string.Format("El campo \"{0}\" no es un RUC válido", name));

        }
    }
}
