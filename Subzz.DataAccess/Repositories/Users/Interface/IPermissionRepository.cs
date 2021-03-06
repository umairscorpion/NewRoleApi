﻿using SubzzV2.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubzzV2.Core.Models;

namespace Subzz.DataAccess.Repositories.Users.Interface
{
    public interface IPermissionRepository
    {
        IEnumerable<User> GetResourcesByParentResourceId(int RoleId, int ParentResourceId);
        User InsertUserRole(string RoleName);
        IEnumerable<User> GetUserRoles();
        List<PermissionsCategory> GetPermissionCategories();
        List<RolePermission> RolePermissions(int roleId, string userId);
        RolePermission Post(RolePermission model);
        RolePermission Put(RolePermission model);
        bool Delete(int id);
        List<Role> GetRoleSummaryList(int districtId);
        Role UpdatePermissions(Role model);
    }
}
