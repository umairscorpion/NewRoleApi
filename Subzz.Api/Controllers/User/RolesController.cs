using Microsoft.AspNetCore.Mvc;
using Subzz.Api.Controllers.Base;
using Subzz.Business.Services.Users.Interface;
using SubzzV2.Core.Models;
using System;

namespace Subzz.Api.Controllers.User
{
    [Route("api/roles")]
    public class RolesController : BaseApiController
    {
        private readonly IPermissionService _service;
       
        public RolesController(IPermissionService service)
        {
            _service = service;
        }

        [Route("list/summary")]
        [HttpGet]
        public IActionResult GetRoleSummary()
        {
            try
            { 
                var result = _service.GetRoleSummaryList(CurrentUser.DistrictId);
                return Ok(result);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("")]
        [HttpGet]
        public IActionResult GetUserRoles()
        {
            try
            { 
                var result = _service.GetUserRoles();
                return Ok(result);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("bulk/delete")]
        [HttpPut]
        public IActionResult RemoveUsers([FromBody]int[] ids)
        {
            try
            { 
                if (ids.Length <= 0) return NotFound();
                foreach (var id in ids)
                {
                    _service.Delete(id);
                }
                return Ok();
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [HttpPost]
        [Route("")]
        public SubzzV2.Core.Entities.User InsertUserRole([FromBody]Role model)
        {
            try
            { 
                var ParentResources = _service.InsertUserRole(model.Name);
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
    }
}