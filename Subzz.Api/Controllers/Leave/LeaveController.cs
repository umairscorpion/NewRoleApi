using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Subzz.Api.Controllers.Base;
using Subzz.Api.Custom;
using Subzz.Business.Services.Users.Interface;
using Subzz.Integration.Core.Container;
using Subzz.Integration.Core.Domain;
using Subzz.Integration.Core.Helper;
using SubzzAbsence.Business.Absence.Interface;
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
        private readonly IUserService _userService;
        private readonly IAbsenceService _absenceService;
        public LeaveController(ILeaveService service, IAuditingService audit, IDistrictService disService, IAbsenceService absenceService, IUserService userService)
        {
            _service = service;
            _audit = audit;
            _disService = disService;
            _absenceService = absenceService;
            _userService = userService;
        }

        private CommunicationContainer _communicationContainer;
        public virtual CommunicationContainer CommunicationContainer
        {
            get
            {
                return _communicationContainer ?? (_communicationContainer = new CommunicationContainer());
            }
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
                model.ApprovedBy = base.CurrentUser.Id;
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
                        ActionType = AuditLogs.ActionType.Denied,
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
                //Notification notification = new Notification(_userService, _absenceService);
                Task.Run(() => SendNotificationsOnJobApprovedOrDenied(model));
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
                var districtId = base.CurrentUser.DistrictId;
                var response = _service.GetEmployeeLeaveBalance(leaveBalance);
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

        async Task SendNotificationsOnJobApprovedOrDenied(LeaveRequestModel leave)
        {
            AbsenceModel absenceDetail = _absenceService.GetAbsenceDetailByAbsenceId(Convert.ToInt32(leave.AbsenceId));
            Message message = new Message();
            message.AbsenceId = absenceDetail.AbsenceId;
            message.StartTime = DateTime.ParseExact(Convert.ToString(absenceDetail.StartTime), "HH:mm:ss",
                                CultureInfo.InvariantCulture).ToSubzzTime();
            message.EndTime = DateTime.ParseExact(Convert.ToString(absenceDetail.EndTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture).ToSubzzTime();
            message.StartDate = Convert.ToDateTime(absenceDetail.StartDate).ToString("D");
            message.EndDate = Convert.ToDateTime(absenceDetail.EndDate).ToString("D");
            message.EmployeeName = absenceDetail.EmployeeName;
            message.Position = absenceDetail.PositionDescription;
            message.Subject = absenceDetail.SubjectDescription;
            message.Grade = absenceDetail.Grade;
            message.Location = absenceDetail.AbsenceLocation;
            message.Notes = absenceDetail.SubstituteNotes;
            message.SubstituteName = absenceDetail.SubstituteName;
            message.Reason = absenceDetail.AbsenceReasonDescription;
            message.ApprovedBy = _userService.GetUserDetail(leave.ApprovedBy).FirstName;
            message.Duration = absenceDetail.DurationType == 1 ? "Full Day" : absenceDetail.DurationType == 2 ? "First Half" : absenceDetail.DurationType == 3 ? "Second Half" : "Custom";
            var employeeDetail = _userService.GetUserDetail(absenceDetail.EmployeeId);
            if (leave.IsApproved)
            message.TemplateId = 16;
            else message.TemplateId = 19;
            message.Photo = absenceDetail.EmployeeProfilePicUrl;
            try
            {
                message.Password = employeeDetail.Password;
                message.UserName = employeeDetail.FirstName;
                message.SendTo = employeeDetail.Email;
                if (employeeDetail.IsSubscribedEmail)
                {
                    var events = _userService.GetSubstituteNotificationEvents(employeeDetail.UserId);
                    var jobPostedEvent = events.Where(x => x.EventId == 1).First();
                    if (leave.IsApproved)
                    {
                        if (jobPostedEvent.EmailAlert)
                            await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                    }
                    else
                    {
                        await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                    }
                }

            }
            catch (Exception ex)
            {
            }

            if (leave.IsApproved)
            {
                IEnumerable<SubzzV2.Core.Entities.User> users = _userService.GetAdminListByAbsenceId(Convert.ToInt32(leave.AbsenceId));
                foreach (var user in users)
                {
                    try
                    {
                        message.Password = user.Password;
                        message.UserName = user.FirstName;
                        message.SendTo = user.Email;
                        //For Substitutes on In case of direct Assign
                        if (user.RoleId == 4 && absenceDetail.AbsenceScope == 2)
                        {
                            var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            var jobPostedEvent = events.Where(x => x.EventId == 2).First();
                            message.TemplateId = 7;
                            if (user.IsSubscribedEmail)
                            {
                                if (jobPostedEvent.EmailAlert)
                                    await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                            }

                            if (user.IsSubscribedSMS)
                            {
                                if (jobPostedEvent.TextAlert)
                                    await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                            }
                        }

                        else if (user.RoleId == 4 && absenceDetail.AbsenceScope != 2 && absenceDetail.AbsenceScope != 5)
                        {
                            message.TemplateId = 1;
                            if (user.IsSubscribedEmail)
                            {
                                var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                                var jobPostedEvent = events.Where(x => x.EventId == 2).First();
                                if (jobPostedEvent.EmailAlert)
                                    await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                            }
                        }
                        else if (user.RoleId == 3)
                        {
                            message.TemplateId = 10;
                            if (user.IsSubscribedEmail)
                            {
                                var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                                var jobPostedEvent = events.Where(x => x.EventId == 2).First();
                                if (jobPostedEvent.EmailAlert)
                                    await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                            }
                        }
                        //For Admins
                        else
                        {
                            message.TemplateId = 2;
                            if (user.IsSubscribedEmail)
                            {
                                var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                                var jobPostedEvent = events.Where(x => x.EventId == 2).First();
                                if (jobPostedEvent.EmailAlert)
                                    await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

        }
    }
}