using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Text;
using Data;
using Entity;
using PagedList;

namespace Domain
{
    public class Query<T>
    {
        public Paginacion Paginacion { get; set; }
        public Func<T, bool> Filter { get; set; }
        public Order<T> Order { get; set; }
        public IPagedList<T> Elements { get; set; }
        public T Criteria { get; set; }

        public Query<T> Validate()
        {
            if (Paginacion == null)
                Paginacion = new Paginacion().Validate();
            if (Elements == null)
                Elements = new PagedList<T>(new List<T>(), 1, 1);
            return this;
        }

        public void BuildFilter()
        {
            var type = typeof(T);
            var method = type.GetMethod("BuildFilter");
            Func<T, bool> temp = null;
            if (method != null && Criteria != null)
            {
                temp = (Func<T, bool>)method.Invoke(Criteria, null);
            }
            if (Criteria == null)
            {
                Filter = temp;
                return;
            }
            
            var properties = type.GetValidProperties();
            Expression<Func<T, bool>> filter = null;
            var parameter = Expression.Parameter(typeof(T), "t");
            var list = new List<Expression>();
            foreach (var prop in properties)
            {
                var value = prop.GetMethod.Invoke(Criteria, null);
                if (value == null || value.ToString().Trim() == "" || value.ToString().Trim() == "-99999999") continue;
                Expression expression = parameter;
                var left = Expression.Property(expression, prop);
                if (prop.PropertyType == typeof(string))
                {

                    var hh = typeof(string).GetMethods().FirstOrDefault(t => t.Name.Equals("ToLower"));
                    var tr = Expression.Call(left, hh);
                    var right = Expression.Constant(value.ToString().ToLower());
                    var h = Expression.Call(tr, typeof(string).GetMethod("Contains"), right);
                    var ex = Expression.IsTrue(h);
                    var jj = Expression.AndAlso(Expression.NotEqual(left, Expression.Constant(null)), ex);
                    list.Add(jj);
                }
                else
                {
                    if (prop.PropertyType.ToString().ToLower().Contains("nullable"))
                    {
                        var fg = prop.PropertyType.GetMethods().FirstOrDefault(t => t.Name.Equals("GetValueOrDefault"));
                        var ed = Expression.Call(left, fg);
                        var right = Expression.Constant(value);
                        var ex = Expression.Equal(ed, right);
                        list.Add(ex);

                    }
                    else
                    {
                        var right = Expression.Constant(value);
                        var ex = Expression.Equal(left, right);
                        list.Add(ex);
                    }
                }
            }
            while (list.Count > 1)
            {
                Expression a = Expression.AndAlso(list[0], list[1]);
                list.RemoveAt(0);
                list.RemoveAt(0);
                list.Add(a);
            }
            Expression result = Expression.IsTrue(Expression.Constant(true));
            if (list.Any())
                result = list.First();
            var temp2 = Expression.Lambda<Func<T, bool>>(result, new ParameterExpression[] { parameter }).Compile();
            if (temp == null)
                Filter = temp2;
            else
            {
                Filter = t => temp2(t) && temp(t);
            }
        }
    }
}
