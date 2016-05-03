using System.Collections.Generic;
using Entity;
using PagedList;

namespace Data.Contratos
{
    public interface IRepositorioProvincia
    {
        IPagedList<Provincia> Get(Paginacion paginacion = null);
        Provincia Find(string codigo);
        IPagedList<Distrito> GetDistritos(string codigoProvincia, Paginacion paginacion = null);
    }
}
