using System.Collections.Generic;
using Entity;
using PagedList;

namespace Data.Contratos
{
    public interface IRepositorioUbigeo
    {
        IPagedList<Ubigeo> Get(Paginacion paginacion = null);
        Ubigeo Find(string codigo);
    }
}
