using SubzzV2.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubzzV2.Core.Models;

namespace Subzz.Business.Services.Users.Interface
{
    public interface IPermissionService
    {
        IEnumerable<User> GetResourcesByParentResourceId(int RoleId, int ParentResourceId);
        User InsertUserRole(string RoleName);
        IEnumerable<User> GetUserRoles();

        #region Role Permissions
        Role GetRolePermissions(int roleId, int districtId);
        Role UpdatePermissions(Role model);
        RolePermission Post(RolePermission model);
        RolePermission Put(RolePermission model);
        bool Delete(int id);
        #endregion

        List<Role> GetRoleSummaryList(int districtId);
    }
}
