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

        public Role GetRolePermissions(int roleId, int districtId)
        {
            var role = new Role();
            role.Role_Id = roleId;
            var permissionCategories = _repo.GetPermissionCategories();
            var rolePermissions = _repo.RolePermissions(roleId);
            if (rolePermissions == null || rolePermissions.Count <= 0) return role;
            role.Name = rolePermissions.First().RoleName;
            foreach (var category in permissionCategories)
            {
                category.Permissions = rolePermissions.Where(t => t.PermissionCategoryId == category.Id).ToList();
            }
            role.PermissionsCategories = permissionCategories;
            return role;
        }

        public Role UpdatePermissions(Role model)
        {
            return _repo.UpdatePermissions(model);
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

        public List<Role> GetRoleSummaryList(int districtId)
        {
            return _repo.GetRoleSummaryList(districtId);
        }
    }
}
