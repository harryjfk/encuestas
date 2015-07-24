using System.Data.Entity;
using System.Linq.Expressions;
using Data;
using Data.Repositorios;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Repositorios;
using PagedList;

namespace Domain.Managers
{
    public abstract class GenericManager<T> where T : class
    {
        protected GenericRepository<T> Repository { get; set; }

        public string Usuario
        {
            get
            {
                return Manager != null ? Manager.UsuarioAutenticado : null;
            }
        }

        protected Manager Manager { get; set; }

        protected GenericManager(GenericRepository<T> repository, Manager manager)
        {
            Repository = repository;
            Manager = manager;
        }

        protected GenericManager(Entities context, Manager manager)
        {
            Repository = new GenericRepository<T>(context);
            Manager = manager;
        }
        //public virtual IPagedList<T> Get(Paginacion paginacion )
        //{
        //    return Repository.Get(paginacion);
        //}
        //public virtual IPagedList<T> Get(Func<T, bool> filter, Paginacion paginacion = null)
        //{
        //    return Repository.Get(filter, paginacion);
        //}
        public virtual IPagedList<T> Get(Func<T, bool> filter = null, Paginacion paginacion = null, Order<T> order = null)
        {
            return Repository.Get(filter, paginacion, order);
        }
        public virtual IPagedList<T> Get(Query<T>query)
        {
            var list= Repository.Get(query.Filter, query.Paginacion, query.Order);
            query.Elements = list;
            return list;
        }
        public virtual T Find(params object[] keys)
        {
            return Repository.Find(keys);
        }
        public virtual OperationResult<T> Add(T element)
        {
            var errors = Validate(element);
            if (errors.Count == 0)
            {
                try
                {
                    UpdateKey(element);
                    SetValue("creado",element,DateTime.Now);
                    SetValue("usuario_creacion",element,Usuario);
                    var entity = Repository.Add(element);
                    return new OperationResult<T>(entity) { Success = true };
                }
                catch (Exception)
                {
                    return new OperationResult<T>(null) { Errors = new List<string>() { "Imposible efectuar la operación." } };
                }
            }
            return new OperationResult<T>(null) { Errors = errors };
        }

        public virtual void UpdateKey(T element)
        {
            var key = element.GeKey();
            if (key != null)
            {
                var last = Get().OrderBy(key).LastOrDefault();
                var value = last == null ? (long)1 : Tools.GetKeyValue(last) + 1;
                element.UpdateKey(value);
            }
        }

        private void SetValue(string property,T element, object value)
        {
            var type = typeof (T);
            var prop = type.GetProperties().FirstOrDefault(t => t.Name.ToLower().Equals(property.ToLower()));
            if (prop != null)
            {
                prop.SetMethod.Invoke(element, new[] {value});
            }
        }
        public virtual OperationResult<T> Modify(T element,params string [] properties)
        {
            var errors = Validate(element);
            if (errors.Count == 0)
            {
                try
                {
                    SetValue("modificado", element, DateTime.Now);
                    SetValue("usuario_modificacion", element, Usuario);
                    var lis = new List<string>();//properties.ToList();
                    lis.Add("creado");
                    lis.Add("usuario_creacion");
                    var entity = Repository.Modify(element,false,lis.ToArray());
                    return new OperationResult<T>(entity) { Success = true };
                }
                catch (Exception)
                {
                    return new OperationResult<T>(null) { Errors = new List<string>() { "Imposible efectuar la operación." } };
                }
            }
            return new OperationResult<T>(null) { Errors = errors };
        }
        
        public virtual OperationResult<T> Delete(T element)
        {
            try
            {
                Repository.Delete(element);
                return new OperationResult<T>(element) { Success = true };
            }
            catch (Exception)
            {
                return new OperationResult<T>(null) { Errors = new List<string>() { "Imposible efectuar la operación." } };
            }

        }
        public virtual OperationResult<T> Delete(params object[] keys)
        {
            try
            {
                var item = Find(keys);
                if (item != null)
                {
                    return Delete(item);
                    
                }
                return new OperationResult<T>(null) { Errors = new List<string>() { "No se pudo encontrar el elemento." } };
            }
            catch (Exception)
            {
                return new OperationResult<T>(null) { Errors = new List<string>() { "Imposible efectuar la operación." } };
            }

        }
        public virtual int SaveChanges()
        {
            return Repository.SaveChanges();
        }
        public virtual List<string> Validate(T element)
        {
            return new List<string>();
        }

        public virtual void AddOrUpdate(Expression<Func<T,object>> expression,params T[] entites)
        {
            Repository.AddOrUpdate(expression,entites);
            Repository.SaveChanges();
        }
        public virtual void Seed()
        {
            
        }

        

    }
}
