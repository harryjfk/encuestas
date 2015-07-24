using System.Collections.Generic;
using Entity;
using PagedList;

namespace Data.Contratos
{
    public interface IRepositorioDepartamento
    {
        IPagedList<Departamento> Get(Paginacion paginacion = null);
        Departamento Find(string codigo);
        IPagedList<Provincia> GetProvincias(string codigoDepartamento, Paginacion paginacion);
    }
}
