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
        [Route("getLeaveRequests/{IsApproved}/{IsDenied}")]
        [HttpGet]
        public IEnumerable<LeaveRequestModel> GetLeaveRequests(int IsApproved, int IsDenied)
        {
            var leaveRequests = _service.GetLeaveRequests(IsApproved, IsDenied);
            return leaveRequests;
        }
        [Route("getLeaveTypes")]
        [HttpGet]
        public IEnumerable<LeaveTypeModel> GetLeaveTypes()
        {
            var leaveTypes = _service.GetLeaveTypes();
            return leaveTypes;
        }
    }
}