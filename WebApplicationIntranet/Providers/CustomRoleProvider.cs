using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Domain.Managers;
using Entity;

namespace WebApplication.Providers
{
    public class CustomRoleProvider : RoleProvider
    {
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            var manager = Tools.GetManager();
            var user = manager.Usuario.Get(t => t.Login == username).FirstOrDefault();
            return user == null 
                ? new string[0] 
                : user.Roles.Select(t => t.Nombre).ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            var manager = Tools.GetManager();
            var rol =
                manager.Rol.Get(t => t.Nombre==roleName).FirstOrDefault();
            return rol==null 
                ? new string[0] 
                : rol.Usuarios.Select(t => t.Login).ToArray();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var manager = Tools.GetManager();
            var user = manager.Usuario.Get(t => t.Login == username).FirstOrDefault();
            return user != null && user.Roles.Any(t => t.Nombre == roleName);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}