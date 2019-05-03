using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Subzz.Api.Controllers.Base;
using Subzz.Business.Services.Users;
using Subzz.Business.Services.Users.Interface;
using SubzzV2.Core.Models;

namespace Subzz.Api.Controllers.User
{
    /// <summary>
    /// Role Permissions
    /// </summary>
    [Route("api/permissions")]
    public class RolePermissionsController : BaseApiController
    {
        private readonly IPermissionService _service;
        /// <summary>
        /// 
        /// </summary>
        public RolePermissionsController(IPermissionService service)
        {
            _service = service;
        }


        [HttpGet]
        [Route("role/{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var result = _service.GetRolePermissions(id, CurrentUser.DistrictId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("bulk/update")]
        public IActionResult Post([FromBody]Role model)
        {
            try
            {
                model.DistrictId = 0;
                var result = _service.UpdatePermissions(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("")]
        public IActionResult Post(RolePermission model)
        {
            try
            {
                var result = _service.Post(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPut]
        [Route("")]
        public IActionResult Put(int id, RolePermission model)
        {
            try
            {
                var result = _service.Put(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpDelete]
        [Route("")]
        public IActionResult Delete(int id)
        {
            try
            {
                var result = _service.Delete(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}