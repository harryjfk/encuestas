using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using Data.Contratos;
using Entity;
using PagedList;

namespace Data.Repositorios
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        public DbContext Context { get; set; }
        public GenericRepository(DbContext context)
        {
            Context = context;
        }

        public T Find(params object[] keys)
        {
            return Context.Set<T>().Find(keys);
        }

        public bool Delete(T element, bool saveChanges = false)
        {
            Context.Entry(element).State = EntityState.Deleted;
            if (saveChanges)
                SaveChanges();
            return true;
        }

        public T Add(T element, bool saveChanges = false)
        {
            Context.Entry(element).State = EntityState.Added;
            if (saveChanges)
                SaveChanges();
            return element;
        }

        public T Modify(T element, bool saveChanges = false, params string[] properties)
        {
            Context.Entry(element).State = EntityState.Modified;
            var type = typeof (T);
            var all = type.GetProperties();
            var prop = properties.Where(t => all.Any(h => h.Name == t));
            foreach (var property in prop)
            {
                Context.Entry(element).Property(property).IsModified = false;
            }
            if (saveChanges)
                SaveChanges();
            return element;
        }

        private Func<T, object> GetProperty(string name)
        {
            var type = typeof(T);
            var property = type.GetProperty(name);
            if (property == null) return null;
            var parameter = Expression.Parameter(typeof(T), "t");
            Expression expression = parameter;
            expression = Expression.Property(expression, property);
            return Expression.Lambda<Func<T, object>>(expression, new ParameterExpression[] { parameter }).Compile();
        }

        public IPagedList<T> Get(Paginacion paginacion = null)
        {
            return paginacion == null
                ? new PagedList<T>(Context.Set<T>(), 1, !Context.Set<T>().Any() ? 1 : Context.Set<T>().Count())
                : Context.Set<T>().ToPagedList(paginacion.Page, paginacion.ItemsPerPage);
        }

        //public IPagedList<T> Get(Func<T, bool> filter, Paginacion paginacion = null)
        //{
        //    var list = Context.Set<T>().Where(filter);
        //    if (paginacion != null) return list.ToPagedList(paginacion.Page, paginacion.ItemsPerPage);
        //    var arr = list.ToList();
        //    return new PagedList<T>(arr, 1, !arr.Any() ? 1 : arr.Count);
        //}


        public IPagedList<T> Get(Func<T, bool> filter = null, Paginacion paginacion = null, Order<T> order = null)
        {
            if (filter != null)
            {
                var list = Context.Set<T>().Where(filter).Order(order);
                if (paginacion != null) return list.ToPagedList(paginacion.Page, paginacion.ItemsPerPage);
                var arr = list.ToList();
                return new PagedList<T>(arr, 1, !arr.Any() ? 1 : arr.Count);
            }
            else
            {
                var list = Context.Set<T>().Order(order);
                if (paginacion != null) return list.ToPagedList(paginacion.Page, paginacion.ItemsPerPage);
                var arr1 = list.ToList();
                return new PagedList<T>(arr1, 1, !arr1.Any() ? 1 : arr1.Count);
            }
        }

        public int SaveChanges()
        {
            try
            {
                return Context.SaveChanges();
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public void AddOrUpdate(Expression<Func<T, object>> expression, params T[] entites)
        {
            Context.Set<T>().AddOrUpdate(expression, entites);
        }
    }
}
