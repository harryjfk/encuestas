using System.Collections.Generic;
using Entity;
using PagedList;

namespace Data.Contratos
{
    public interface IRepositorioDistrito
    {
        IPagedList<Distrito> Get(Paginacion paginacion = null);
        Distrito Find(string codigo);
    }
}
