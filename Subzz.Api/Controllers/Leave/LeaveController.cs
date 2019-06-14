using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Subzz.Api.Controllers.Base;
using Subzz.Business.Services.Users.Interface;
using SubzzAbsence.Business.Leaves.Interface;
using SubzzManage.Business.District.Interface;
using SubzzV2.Core.Enum;
using SubzzV2.Core.Models;

namespace Subzz.Api.Controllers.Leave
{
    [Route("api/Leave")]
    public class LeaveController : BaseApiController
    {
        private readonly ILeaveService _service;
        private readonly IAuditingService _audit;
        private readonly IDistrictService _disService;
        public LeaveController(ILeaveService service, IAuditingService audit, IDistrictService disService)
        {
            _service = service;
            _audit = audit;
            _disService = disService;
        }

        [Route("insertLeaveRequest")]
        [HttpPost]
        public LeaveRequestModel InsertLeaveRequest([FromBody]LeaveRequestModel model)
        {
            try
            {
                model.CreatedById = base.CurrentUser.Id;
                var leaveModel = _service.InsertLeaveRequest(model);
                return leaveModel;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("updateLeaveRequestStatus")]
        [HttpPost]
        public LeaveRequestModel UpdateLeaveRequestStatus([FromBody]LeaveRequestModel model)
        {
            try
            {
                model.EmployeeId = base.CurrentUser.Id;
                var leaveRequests = _service.UpdateLeaveRequestStatus(model);
                // Audit Log
                if (model.IsApproved == true)
                {
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = model.AbsenceId.ToString(),
                        EntityType = AuditLogs.EntityType.Absence,
                        ActionType = AuditLogs.ActionType.Approved,
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
                if (model.IsDeniend == true)
                {
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = model.AbsenceId.ToString(),
                        EntityType = AuditLogs.EntityType.Absence,
                        ActionType = AuditLogs.ActionType.Declined,
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }

                return leaveRequests;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("insertLeaveType")]
        [HttpPost]
        public LeaveTypeModel InsertLeaveType([FromBody]LeaveTypeModel model)
        {
            try
            {
                var leaveTypeModel = _service.InsertLeaveType(model);
                return leaveTypeModel;
            }
            catch(Exception ex)
            {
            }
            finally
            { 
            }
            return null;
        }

        [Route("getLeaveRequests/{districtId}/{organizationId}")]
        [HttpGet]
        public IEnumerable<LeaveRequestModel> GetLeaveRequests(int districtId, string organizationId)
        {
            try
            {
                var leaveRequests = _service.GetLeaveRequests(districtId, organizationId);
                return leaveRequests;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getLeaveTypes")]
        [HttpGet]
        public IEnumerable<LeaveTypeModel> GetLeaveTypes()
        {
            try
            {
                var leaveTypes = _service.GetLeaveTypes();
                return leaveTypes;
            }
            catch (Exception)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getLeaveTypes/{districtId}/{organizationId}")]
        [HttpGet]
        public IEnumerable<LeaveTypeModel> GetLeaveTypes(int districtId, string organizationId)
        {
            try
            {
                var leaveTypes = _service.GetLeaveTypes(districtId, organizationId);
                return leaveTypes;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("deleteLeaveType/{leaveTypeId}")]
        [HttpDelete]
        public IActionResult DeleteLeaveType(int leaveTypeId)
        {
            try
            {
                var response = _service.DeleteLeaveType(leaveTypeId);
                return Ok(response);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getleaveTypeById/{leaveTypeId}")]
        [HttpGet]
        public IActionResult GetleaveTypeById(int leaveTypeId)
        {
            try
            {
                var response = _service.GetleaveTypeById(leaveTypeId);
                return Ok(response);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        //For Leave Balance Report
        [Route("getEmployeeLeaveBalance")]
        [HttpPost]
        public IActionResult GetEmployeeLeaveBalance([FromBody]LeaveBalance leaveBalance)
        {
            try
            {
                var Value = 10;
                var districtId = base.CurrentUser.DistrictId;
                var allowances = _disService.GetAllowances(Convert.ToString(districtId));
                var response = _service.GetEmployeeLeaveBalance(leaveBalance);
                response.Where(x => x.Personal == null).ToList().ForEach(x => { x.Personal = Value.ToString(); });
                return Ok(response);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        //For Absence Page to check remaining Balance
        [Route("getLeaveBalance")]
        [HttpPost]
        public IActionResult GetLeaveBalance([FromBody]LeaveBalance leaveBalance)
        {
            try
            {
                var districtId = base.CurrentUser.DistrictId;
                var response = _service.GetLeaveBalance(leaveBalance);
                return Ok(response);
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