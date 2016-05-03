﻿using System;
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
    public class UbigeoManager
    {
        private IRepositorioUbigeo Repository { get; set; }
        private Manager Manager { get; set; }

        public UbigeoManager(IRepositorioUbigeo repositorio, Manager manager)
        {
            Repository = repositorio;
            Manager = manager;
        }

        public Ubigeo Find(string codigo)
        {
            return Repository.Find(codigo);
        }

        public IPagedList<Ubigeo> Get(Paginacion paginacion = null)
        {
            return  Repository.Get(paginacion);
        }
    }
}
