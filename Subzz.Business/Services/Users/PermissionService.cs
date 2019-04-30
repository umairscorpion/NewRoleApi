using Subzz.Business.Services.Users.Interface;
using SubzzV2.Core.Entities;
using Subzz.DataAccess.Repositories.Users;
using Subzz.DataAccess.Repositories.Users.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubzzV2.Core.Models;

namespace Subzz.Business.Services.Users
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _repo;
        public PermissionService(IPermissionRepository repo)
        {
            _repo = repo;
        }
        public IEnumerable<User> GetResourcesByParentResourceId(int RoleId, int ParentResourceId)
        {
            return _repo.GetResourcesByParentResourceId(RoleId, ParentResourceId);
        }
        public User InsertUserRole(string RoleName)
        {
            return _repo.InsertUserRole(RoleName);
        }
        public IEnumerable<User> GetUserRoles()
        {
            return _repo.GetUserRoles();
        }

        public List<PermissionsCategory> GetAllByRole(int roleId, int districtId)
        {
            var permissionCategories = _repo.GetAll();
            if (roleId <= 0) return permissionCategories;
            var rolePermissions = _repo.RolePermissions(roleId);
            if (rolePermissions == null || rolePermissions.Count <= 0) return permissionCategories;
            foreach (var item in rolePermissions)
            {
                var permissionCategory = permissionCategories.FirstOrDefault(pr => pr.Permissions.Any(p => p.PermissionId == item.PermissionId));
                if (permissionCategory == null) continue;
                permissionCategory.IsChecked = true;
                if (permissionCategory.Permissions == null) continue;
                RolePermission first = null;
                foreach (var t in permissionCategory.Permissions)
                {
                    if (t.PermissionId != item.PermissionId) continue;
                    first = t;
                    break;
                }
                if (first != null)
                    first.IsChecked = true;
            }
            return permissionCategories;
        }

        public PermissionMaster UpdatePermissions(PermissionMaster model)
        {
            //TODO
            throw new NotImplementedException();
        }

        public RolePermission Post(RolePermission model)
        {
            return _repo.Post(model);
        }

        public RolePermission Put(RolePermission model)
        {
            return _repo.Put(model);
        }

        public bool Delete(int id)
        {
            return _repo.Delete(id);
        }
    }
}
