using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Contratos;
using Data.Repositorios;
using Entity;
using PagedList;

namespace Domain.Managers
{
    public class ProvinciaManager 
    {
        private IRepositorioProvincia Repository { get; set; }
        private Manager Manager { get; set; }

        public ProvinciaManager(IRepositorioProvincia repositorio, Manager manager)
        {
            Repository = repositorio;
            Manager = manager;
        }
        public Provincia Find(string codigo)
        {
            return Repository.Find(codigo);
        }

        public IPagedList<Provincia> Get(Paginacion paginacion = null)
        {
            return Repository.Get(paginacion);
           
        }

        public IPagedList<Distrito> GetDistritos(string codigoProvincia,Paginacion paginacion=null)
        {
            return Repository.GetDistritos(codigoProvincia,paginacion);
        }
    }
}
