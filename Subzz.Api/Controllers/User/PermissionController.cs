using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Subzz.Api.Controllers.Base;
using Subzz.Business.Services.Users.Interface;
using SubzzV2.Core;

namespace Subzz.Api.Controllers.User
{
    [Route("api/Default")]
    public class PermissionController : BaseApiController
    {
        private readonly IPermissionService _service;
        public PermissionController()
        {
        }
        public PermissionController(IPermissionService service)
        {
            _service = service;
        }
        [Route("getResourcesByParentResourceId/{RoleId}/{ParentResourceId}")]
        [HttpGet]
        public IEnumerable<SubzzV2.Core.Entities.User> GetResourcesByParentResourceId(int RoleId, int ParentResourceId)
        {
            try
            {
                var ParentResources = _service.GetResourcesByParentResourceId(RoleId, ParentResourceId);
                return ParentResources;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("insertUserRole/{RoleName}")]
        [HttpGet]
        public SubzzV2.Core.Entities.User InsertUserRole(string RoleName)
        {
            try
            {
                var ParentResources = _service.InsertUserRole(RoleName);
                return ParentResources;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("GetUserRoles")]
        [HttpGet]
        public IEnumerable<SubzzV2.Core.Entities.User> GetUserRoles()
        {
            try
            {
                var UserRoles = _service.GetUserRoles();
                return UserRoles;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

    }
}