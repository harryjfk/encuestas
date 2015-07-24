using System;
using System.Linq;
using Entity;
using PagedList;

namespace Data.Contratos
{
    public interface IRepository<T> where T : class
    {
        T Find(params object[] keys);
        bool Delete(T element,bool saveChanges=false);
        T Add(T element, bool saveChanges = false);
        T Modify(T element, bool saveChanges = false,params string []properties);
        IPagedList<T> Get(Paginacion paginacion=null);
        //IPagedList<T> Get(Func<T, bool> filter,Paginacion paginacion=null);
        IPagedList<T> Get(Func<T, bool> filter=null,Paginacion paginacion=null,Order<T>order=null );
        int SaveChanges();
    }
}
