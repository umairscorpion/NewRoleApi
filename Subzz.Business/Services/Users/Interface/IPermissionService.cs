using SubzzV2.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subzz.Business.Services.Users.Interface
{
    public interface IPermissionService
    {
        IEnumerable<User> GetResourcesByParentResourceId(int RoleId, int ParentResourceId);
        User InsertUserRole(string RoleName);
        IEnumerable<User> GetUserRoles();
    }
}
