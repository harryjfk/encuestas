using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Contratos;
using Data.Repositorios;
using Entity;
using PagedList;

namespace Domain.Managers
{
    public class DepartamentoManager
    {
        private IRepositorioDepartamento Repository { get; set; }
        private Manager Manager { get; set; }
        public DepartamentoManager(IRepositorioDepartamento repositorio, Manager manager)
        {
            Repository = repositorio;
            Manager = manager;
        }

        public Departamento Find(string codigo)
        {
            return Repository.Find(codigo);
        }

        public IPagedList<Departamento> Get(Paginacion paginacion = null)
        {
            return Repository.Get(paginacion);
        }

        public IPagedList<Provincia> GetProvincias(string codigoDepartamento,Paginacion paginacion=null)
        {
            return Repository.GetProvincias(codigoDepartamento,paginacion);
        }
    }
}
