using System;
using System.Collections.Generic;
using Entity;
using PagedList;

namespace Data.Contratos
{
    public interface IRepositorioUsuario
    {
        IPagedList<UsuarioIntranet> GetUsuariosIntranet(Paginacion paginacion = null,Func<UsuarioIntranet,bool>filter=null );
        UsuarioIntranet FindUsuarioIntranet(int codigo);

        IPagedList<UsuarioExtranet> GetUsuariosExtranet(Paginacion paginacion = null, Func<UsuarioExtranet, bool> filter = null);
        UsuarioExtranet FindUsuarioExtranet(int codigo);

        UsuarioIntranet AutenticateIntranet(string login, string password);
        UsuarioExtranet AutenticateExtranet(string login, string password);
        
    }
}
