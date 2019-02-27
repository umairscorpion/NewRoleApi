using Subzz.Business.Services.Users.Interface;
using SubzzV2.Core.Entities;
using Subzz.DataAccess.Repositories.Users;
using Subzz.DataAccess.Repositories.Users.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
