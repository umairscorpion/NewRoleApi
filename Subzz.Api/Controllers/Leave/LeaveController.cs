using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Subzz.Api.Controllers.Base;
using SubzzAbsence.Business.Leaves.Interface;
using SubzzV2.Core.Models;

namespace Subzz.Api.Controllers.Leave
{
    [Route("api/Leave")]
    public class LeaveController : BaseApiController
    {
        private readonly ILeaveService _service;
        public LeaveController(ILeaveService service)
        {
            _service = service;
        }
        [Route("insertLeaveRequest")]
        [HttpPost]
        public LeaveRequestModel InsertLeaveRequest([FromBody]LeaveRequestModel model)
        {
            model.CreatedById = base.CurrentUser.Id;
            var leaveModel = _service.InsertLeaveRequest(model);
            return leaveModel;
        }

        [Route("updateLeaveRequestStatus")]
        [HttpPost]
        public LeaveRequestModel UpdateLeaveRequestStatus([FromBody]LeaveRequestModel model)
        {
            model.EmployeeId = base.CurrentUser.Id;
            var leaveRequests = _service.UpdateLeaveRequestStatus(model);
            return leaveRequests;
        }
        [Route("insertLeaveType")]
        [HttpPost]
        public LeaveTypeModel InsertLeaveType([FromBody]LeaveTypeModel model)
        {
            var leaveTypeModel = _service.InsertLeaveType(model);
            return leaveTypeModel;
        }
        [Route("getLeaveRequests/{districtId}/{organizationId}")]
        [HttpGet]
        public IEnumerable<LeaveRequestModel> GetLeaveRequests(int districtId, string organizationId)
        {
            var leaveRequests = _service.GetLeaveRequests(districtId, organizationId);
            return leaveRequests;
        }

        [Route("getLeaveTypes")]
        [HttpGet]
        public IEnumerable<LeaveTypeModel> GetLeaveTypes()
        {
            var leaveTypes = _service.GetLeaveTypes();
            return leaveTypes;
        }

        [Route("getLeaveTypes/{districtId}/{organizationId}")]
        [HttpGet]
        public IEnumerable<LeaveTypeModel> GetLeaveTypes(int districtId, string organizationId)
        {
            var leaveTypes = _service.GetLeaveTypes(districtId, organizationId);
            return leaveTypes;
        }

        [Route("deleteLeaveType/{leaveTypeId}")]
        [HttpDelete]
        public IActionResult DeleteLeaveType(int leaveTypeId)
        {
            var response = _service.DeleteLeaveType(leaveTypeId);
            return Ok(response);
        }

        [Route("getleaveTypeById/{leaveTypeId}")]
        [HttpGet]
        public IActionResult GetleaveTypeById(int leaveTypeId)
        {
            var response = _service.GetleaveTypeById(leaveTypeId);
            return Ok(response);
        }
    }
}