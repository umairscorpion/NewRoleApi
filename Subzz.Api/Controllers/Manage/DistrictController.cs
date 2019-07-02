using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SubzzManage.Business.District.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Subzz.Api.Controllers.Base;
using SubzzV2.Core.Models;
using FluentValidation.Results;
using Subzz.Api.Validators;
using Subzz.Business.Services.Users.Interface;
using SubzzV2.Core.Enum;
using Subzz.Integration.Core.Helper;

namespace Subzz.Api.Controllers.Manage
{
    [Produces("application/json")]
    [Route("api/District")]
    public class DistrictController : BaseApiController
    {
        private readonly IDistrictService _service;
        private readonly IAuditingService _audit;
        public DistrictController(IDistrictService service, IAuditingService audit)
        {
            _service = service;
            _audit = audit;
        }

        [Route("insertDistrict")]
        [HttpPost]
        public IActionResult InsertDistrict([FromBody]DistrictModel model)
        {
            try
            {
                DistrictValidator validator = new DistrictValidator();
                ValidationResult result = validator.Validate(model);
                if (result.IsValid)
                {
                    var userModel = _service.InsertDistrict(model);
                    // Audit Log
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = model.DistrictId.ToString(),
                        EntityType = AuditLogs.EntityType.District,
                        ActionType = AuditLogs.ActionType.CreatedDistrict,
                        PostValue = Serializer.Serialize(model),
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
                else
                {
                    return BadRequest("Fill All fields");
                }          
                return Json("successfull");
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("updateDistrict")]
        [HttpPatch]
        public IActionResult UpdateDistrict([FromBody]DistrictModel model)
        {
            try
            {
                DistrictValidator validator = new DistrictValidator();
                ValidationResult result = validator.Validate(model);
                if (result.IsValid)
                {
                    var userModel = _service.UpdateDistrict(model);
                    // Audit Log
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = model.DistrictId.ToString(),
                        EntityType = AuditLogs.EntityType.District,
                        ActionType = AuditLogs.ActionType.UpdatedDistrict,
                        PostValue = Serializer.Serialize(model),
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
                else
                {
                    return BadRequest("Fill All fields");
                }
                return Json("successfull");
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getDistricts")]
        [HttpGet]
        public IEnumerable<DistrictModel> GetDistricts()
        {
            try
            {
                var districts = _service.GetDistricts();
                return districts;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;

        }

        [Route("{id}")]
        [HttpDelete]
        public bool Delete(int id)
        {
            var DeleteDistrict = _service.DeleteDistrict(id);
            // Audit Log
            var audit = new AuditLog
            {
                UserId = CurrentUser.Id,
                EntityId = id.ToString(),
                EntityType = AuditLogs.EntityType.District,
                ActionType = AuditLogs.ActionType.DeletedDistrict,
                DistrictId = CurrentUser.DistrictId,
                OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
            };
            _audit.InsertAuditLog(audit);
            return DeleteDistrict;
        }

        [Route("getDistrictById/{Id}")]
        [HttpGet]
        public IEnumerable<DistrictModel> GetDistrict(int id)
        {
            try
            {
                var district = _service.GetDistrict(id);
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = id.ToString(),
                    EntityType = AuditLogs.EntityType.District,
                    ActionType = AuditLogs.ActionType.ViewedDistrict,
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);
                return district;
            }
            catch (Exception ex)
            {
            }
            finally {
            }
            return null;
        }

        [Route("updateSettings")]
        [HttpPost]
        public IActionResult UpdateSettings([FromBody] DistrictModel districtModel)
        {
            try
            {
                var district = _service.UpdateSettings(districtModel);
            return Ok(district);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("allowances")]
        [HttpPost]
        public IActionResult AddAllowances([FromBody]Allowance model)
        {
            try
            {
                var allowance = _service.AddAllowance(model);

                return Ok(allowance);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("allowances")]
        [HttpPatch]
        public IActionResult UpdateAllowances([FromBody]Allowance model)
        {
            try
            {
                var allowance = _service.AddAllowance(model);
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = model.Id.ToString(),
                    EntityType = AuditLogs.EntityType.Allowances,
                    ActionType = AuditLogs.ActionType.UpdatedAllowance,
                    PostValue = Serializer.Serialize(model),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

                return Ok(allowance);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getAllowances/{districtId}")]
        [HttpGet]
        public IActionResult GetAllowances(string districtId)
        {
            try
            {
                var allowance = _service.GetAllowances(districtId);
            return Ok(allowance);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("deleteAllowance/{id}")]
        [HttpDelete]
        public IActionResult DeleteAllowance(int id)
        {
            try
            {
                var allowance = _service.DeleteAllowance(id);
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = id.ToString(),
                    EntityType = AuditLogs.EntityType.Allowances,
                    ActionType = AuditLogs.ActionType.DeletedAllowance,
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

                return Ok(allowance);
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