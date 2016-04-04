using System;
using System.Collections.Generic;
using Entity;
using PagedList;

namespace Data.Contratos
{
    public interface IRepositorioUsuario
    {
        IPagedList<UsuarioIntranet> GetUsuariosIntranet(Paginacion paginacion = null,Func<UsuarioIntranet,bool>filter=null );

        IPagedList<UsuarioIntranet> GetUsuariosIntranetAnalista(Paginacion paginacion = null, Func<UsuarioIntranet, bool> filter = null);
        IPagedList<UsuarioIntranet> GetUsuariosIntranetAdministrador(Paginacion paginacion = null, Func<UsuarioIntranet, bool> filter = null);

        UsuarioIntranet FindUsuarioIntranet(int codigo);

        /*UsuarioIntranet FindUsuarioIntranet(int codigo, int idRol);
        
        UsuarioIntranet GetUsuarioIntranetById(int id);*/

        IPagedList<UsuarioExtranet> GetUsuariosExtranet(Paginacion paginacion = null, Func<UsuarioExtranet, bool> filter = null);
        UsuarioExtranet FindUsuarioExtranet(int codigo);

        UsuarioIntranet AutenticateIntranet(string login, string password);
        UsuarioExtranet AutenticateExtranet(string login, string password);        
    }
}
