using System.Globalization;
using System.IO;
using System.Reflection;
using Data.Contratos;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Domain
{
    public static class Tools
    {
        public static void UpdateKey<T>(this T element,long value)where T:class
        {
            try
            {
                var property = typeof (T).GetProperty("Id");
                if (property != null)
                {
                    property.SetMethod.Invoke(element, new object[] { value });
                }
            }
            catch
            {
               
            }
        }

        public static Func<T, long> GeKey<T>(this T element)
        {
            var type = typeof (T);
            var property = type.GetProperty("Id");
            if (property == null) return null;
            var parameter = Expression.Parameter(typeof(T), "t");
            Expression expression = parameter;
            expression = Expression.Property(expression, property);
            return Expression.Lambda<Func<T, long>>(expression, new ParameterExpression[] { parameter }).Compile();
        }

        public static long GetKeyValue<T>(T element)
        {
            var type = typeof(T);
            var prop = type.GetProperty("Id");
            if (prop == null) return 0;
            var value = (long)prop.GetMethod.Invoke(element, null);
            return value;
        }

        public static double Varianza(this List<double> data)
        {
            var media = data.Average();
            if (data.Count < 2) return 0;
            return data.Sum(t => Math.Pow(t - media, 2))/data.Count-1;
        }

        public static double DesviacionEstandar(this List<double> data)
        {
            var varianza = data.Varianza();
            if (varianza <= 0) return 0;
            
            return Math.Sqrt(varianza);
        }

        public static string GetMonthText(this int number)
        {
            var now = DateTime.Now;
            var date = new DateTime(now.Year, number, now.Day);
            var cInfo = CultureInfo.GetCultureInfo("es");
            var textInfo = cInfo.TextInfo;
            var month= date.ToString("MMMM", cInfo);
            return textInfo.ToTitleCase(month);
        }

        public static MemoryStream ExportToExcel(this List<object> source)
        {
            var result = new SimpleExcelExport.ExportToExcel();
            var bytes = result.ListToExcel(source);
            var stream = new MemoryStream(bytes);
            return stream;
        }

        //public static Func<T, bool> GetFilter<T>(this string text) where T : class
        //{
        //    var type = typeof (T);
        //    if (type.IsSubclassOf(typeof (Filtered<T>)))
        //    {
        //        var method = type.GetMethod("GetFilter",BindingFlags.Static);
        //        if (method != null)
        //        {
                    
        //        }
        //    }
        //}
    }
}
