using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Entity;

namespace Domain
{
    public static class Filter
    {
        public static Func<T, bool> GetFilter<T>(this string text)
        {
            var name = string.Format("Filter{0}", typeof (T).Name);
            var type = typeof (Filter);
            var method = type.GetMethod(name, BindingFlags.Static);
            if (method == null) return t => true;
            var result = method.Invoke(null, new object[] {text});
            return (Func<T, bool>)result;
        }

        public static Func<Ciiu, bool> FilterCiiu(string text)
        {
            text = text.ToLower();
            return t => t.Nombre.ToLower().Contains(text) || t.Revision.ToString().Contains(text);
        }
        public static Func<LineaProducto, bool> FilterLineaProducto(string text)
        {
            text = text.ToLower();
            return t => t.Nombre.ToLower().Contains(text) || t.Codigo.ToLower().Contains(text);
        }
        public static Func<Establecimiento, bool> FilterEstablecimiento(string text)
        {
            text = text.ToLower();
            return t => t.Nombre.ToLower().Contains(text) 
                || t.Ruc.ToLower().Contains(text)
                || t.RazonSocial.ToLower().Contains(text);
        }
    }
}
