﻿using System;
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
                        EntityId = model.ConfirmationNumber.ToString(),
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
                        EntityId = model.ConfirmationNumber.ToString(),
                        EntityType = AuditLogs.EntityType.Absence,
                        ActionType = AuditLogs.ActionType.Denied,
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
                if (model.IsArchived == true)
                {
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = model.ConfirmationNumber.ToString(),
                        EntityType = AuditLogs.EntityType.Absence,
                        ActionType = AuditLogs.ActionType.Archived,
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
                var leaveTypeId = model.LeaveTypeId;
                var leaveTypeModel = _service.InsertLeaveType(model);
                // Audit Log
                if (leaveTypeId > 0)
                {
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = model.LeaveTypeId.ToString(),
                        EntityType = AuditLogs.EntityType.LeaveType,
                        ActionType = AuditLogs.ActionType.UpdatedLeaveType,
                        PostValue = Serializer.Serialize(model),
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
                else
                {
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = model.LeaveTypeId.ToString(),
                        EntityType = AuditLogs.EntityType.LeaveType,
                        ActionType = AuditLogs.ActionType.CreatedLeaveType,
                        PostValue = Serializer.Serialize(model),
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
                
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
                if (response == 1)
                {
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = leaveTypeId.ToString(),
                        EntityType = AuditLogs.EntityType.LeaveType,
                        ActionType = AuditLogs.ActionType.DeletedLeaveType,
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
                
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
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = leaveTypeId.ToString(),
                    EntityType = AuditLogs.EntityType.LeaveType,
                    ActionType = AuditLogs.ActionType.ViewedLeaveType,
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);
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
            message.ConfirmationNumber = absenceDetail.ConfirmationNumber;
            message.AbsenceId = absenceDetail.AbsenceId;
            message.StartTime = DateTime.ParseExact(Convert.ToString(absenceDetail.StartTime), "HH:mm:ss",
                                CultureInfo.InvariantCulture).ToSubzzTime();
            message.EndTime = DateTime.ParseExact(Convert.ToString(absenceDetail.EndTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture).ToSubzzTime();
            message.StartDate = Convert.ToDateTime(absenceDetail.StartDate).ToString("D");
            message.EndDate = Convert.ToDateTime(absenceDetail.EndDate).ToString("D");
            message.StartTimeSMS = DateTime.ParseExact(Convert.ToString(absenceDetail.StartTime), "HH:mm:ss",
                                CultureInfo.InvariantCulture).ToSubzzTimeForSms();
            message.EndTimeSMS = DateTime.ParseExact(Convert.ToString(absenceDetail.EndTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture).ToSubzzTimeForSms();
            if (message.StartDate == message.EndDate)
            {
                message.DateToDisplayInSMS = Convert.ToDateTime(absenceDetail.StartDate).ToSubzzDateForSMS();
            }
            else
            {
                message.DateToDisplayInSMS = Convert.ToDateTime(absenceDetail.StartDate).ToSubzzShortDateForSMS() + "-" + Convert.ToDateTime(absenceDetail.EndDate).ToSubzzDateForSMS();
            }
            if (!string.IsNullOrEmpty(absenceDetail.OrganizationPhoneNumber) && absenceDetail.OrganizationPhoneNumber.Length > 5)
            {
                message.FromPhoneNumber = absenceDetail.OrganizationPhoneNumber;
            }
            else
            {
                message.FromPhoneNumber = absenceDetail.DistrictPhoneNumber;
            }
            message.EmployeeName = absenceDetail.EmployeeName;
            message.Position = absenceDetail.PositionDescription;
            message.Subject = absenceDetail.SubjectDescription;
            message.Grade = absenceDetail.Grade;
            message.Location = absenceDetail.AbsenceLocation;
            message.School = absenceDetail.OrganizationName;
            message.Notes = absenceDetail.SubstituteNotes;
            message.SubstituteName = absenceDetail.SubstituteName;
            message.Reason = absenceDetail.AbsenceReasonDescription;
            message.AttachedFileName = absenceDetail.AttachedFileName;
            message.FileContentType = absenceDetail.FileContentType;
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
                        if (user.RoleId == 4 && absenceDetail.AbsenceType == 2 && absenceDetail.SubstituteRequired)
                        {
                            message.TemplateId = 7;
                            var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            var jobPostedEvent = events.Where(x => x.EventId == 5).First();
                            var grade = _userService.GetGradeLevelsForNotification(user.UserId);
                            var isGradeEnabled = grade.Where(x => x.TeachingLevelId == absenceDetail.GradeId).FirstOrDefault();
                            var sub = _userService.GetSubjectsForNotifications(user.UserId);
                            var subjects = sub.Where(x => x.TeacherSpecialityId == absenceDetail.SpecialityTypeId).FirstOrDefault();
                            var cat = _userService.GetSubstituteCategories(user.UserId);
                            var categories = cat.Where(x => x.TypeId == absenceDetail.PositionId).FirstOrDefault();

                            if (user.IsSubscribedEmail && jobPostedEvent.EmailAlert && (isGradeEnabled != null ? isGradeEnabled.GradeNotification : true) &&
                                (subjects != null ? subjects.SubjectNotification : true) && (absenceDetail.OnlyCertified ? user.IsCertified == 1 : true) &&
                                (categories != null ? categories.IsNotificationSend : true))
                            {
                                if (absenceDetail.OrganizationId != "-1")
                                {
                                    var subSchools = _userService.GetSubstitutePreferredSchools(user.UserId);
                                    var isSchoolEnabled = subSchools.Where(x => x.OrganizationId == absenceDetail.OrganizationId).First();
                                    if (isSchoolEnabled.IsEnabled)
                                    {
                                        await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                                    }
                                }
                                else
                                {
                                    await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                                }
                            }

                            if (user.IsSubscribedSMS && jobPostedEvent.TextAlert && (isGradeEnabled != null ? isGradeEnabled.GradeNotification : true) &&
                                (subjects != null ? subjects.SubjectNotification : true) && (absenceDetail.OnlyCertified ? user.IsCertified == 1 : true) &&
                                (categories != null ? categories.IsNotificationSend : true))
                            {
                                message.PhoneNumber = user.PhoneNumber;
                                if (absenceDetail.OrganizationId != "-1")
                                {
                                    var subSchools = _userService.GetSubstitutePreferredSchools(user.UserId);
                                    var isSchoolEnabled = subSchools.Where(x => x.OrganizationId == absenceDetail.OrganizationId).First();
                                    if (isSchoolEnabled.IsEnabled)
                                    {
                                        message.PhoneNumber = user.PhoneNumber;
                                        CommunicationContainer.SMSProcessor.Process(message, (MailTemplateEnums)message.TemplateId);
                                    }
                                }
                                else
                                {
                                    CommunicationContainer.SMSProcessor.Process(message, (MailTemplateEnums)message.TemplateId);
                                }
                            }
                            //var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            //var jobPostedEvent = events.Where(x => x.EventId == 5).First();
                            //message.TemplateId = 7;
                            //if (user.IsSubscribedEmail)
                            //{
                            //    if (jobPostedEvent.EmailAlert)
                            //    {
                            //        message.PhoneNumber = user.PhoneNumber;
                            //        var sub = _userService.GetSubjectsForNotifications(user.UserId);
                            //        var subjects = sub.Where(x => x.TeacherSpecialityId == absenceDetail.SpecialityTypeId).FirstOrDefault();
                            //        if (absenceDetail.OnlySubjectSpecialist && subjects != null ? subjects.SubjectNotification : true &&
                            //            absenceDetail.OnlyCertified ? user.IsCertified == 1 : true)
                            //            await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                            //    }
                            //}

                            //if (user.IsSubscribedSMS)
                            //{
                            //    if (jobPostedEvent.TextAlert)
                            //    {
                            //        message.PhoneNumber = user.PhoneNumber;
                            //        var sub = _userService.GetSubjectsForNotifications(user.UserId);
                            //        var subjects = sub.Where(x => x.TeacherSpecialityId == absenceDetail.SpecialityTypeId).FirstOrDefault();
                            //        if (absenceDetail.OnlySubjectSpecialist && subjects != null ? subjects.SubjectNotification : true &&
                            //            absenceDetail.OnlyCertified ? user.IsCertified == 1 : true)
                            //            CommunicationContainer.SMSProcessor.Process(message, (MailTemplateEnums)message.TemplateId);
                            //    }
                            //}

                        }

                        else if (user.RoleId == 4 && absenceDetail.AbsenceType != 2 && absenceDetail.AbsenceType != 5 && absenceDetail.SubstituteRequired)
                        {
                            message.TemplateId = 1;
                            var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            var jobPostedEvent = events.Where(x => x.EventId == 2).First();
                            var grade = _userService.GetGradeLevelsForNotification(user.UserId);
                            var isGradeEnabled = grade.Where(x => x.TeachingLevelId == absenceDetail.GradeId).FirstOrDefault();
                            var sub = _userService.GetSubjectsForNotifications(user.UserId);
                            var subjects = sub.Where(x => x.TeacherSpecialityId == absenceDetail.SpecialityTypeId).FirstOrDefault();
                            var cat = _userService.GetSubstituteCategories(user.UserId);
                            var categories = cat.Where(x => x.TypeId == absenceDetail.PositionId).FirstOrDefault();

                            if (user.IsSubscribedEmail && jobPostedEvent.EmailAlert && (isGradeEnabled != null ? isGradeEnabled.GradeNotification : true) &&
                                (subjects != null ? subjects.SubjectNotification : true) && (absenceDetail.OnlyCertified ? user.IsCertified == 1 : true) &&
                                (categories != null ? categories.IsNotificationSend : true))
                            {
                                if (absenceDetail.OrganizationId != "-1")
                                {
                                    var subSchools = _userService.GetSubstitutePreferredSchools(user.UserId);
                                    var isSchoolEnabled = subSchools.Where(x => x.OrganizationId == absenceDetail.OrganizationId).First();
                                    if (isSchoolEnabled.IsEnabled)
                                    {
                                        await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                                    }
                                }
                                else
                                {
                                    await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                                }
                            }

                            if (user.IsSubscribedSMS && jobPostedEvent.TextAlert && (isGradeEnabled != null ? isGradeEnabled.GradeNotification : true) &&
                                (subjects != null ? subjects.SubjectNotification : true) && (absenceDetail.OnlyCertified ? user.IsCertified == 1 : true) &&
                                (categories != null ? categories.IsNotificationSend : true))
                            {
                                message.PhoneNumber = user.PhoneNumber;
                                if (absenceDetail.OrganizationId != "-1")
                                {
                                    var subSchools = _userService.GetSubstitutePreferredSchools(user.UserId);
                                    var isSchoolEnabled = subSchools.Where(x => x.OrganizationId == absenceDetail.OrganizationId).First();
                                    if (isSchoolEnabled.IsEnabled)
                                    {
                                        message.PhoneNumber = user.PhoneNumber;
                                        CommunicationContainer.SMSProcessor.Process(message, (MailTemplateEnums)message.TemplateId);
                                    }
                                }
                                else
                                {
                                    CommunicationContainer.SMSProcessor.Process(message, (MailTemplateEnums)message.TemplateId);
                                }
                            }
                            //message.TemplateId = 1;
                            //if (user.IsSubscribedEmail)
                            //{
                            //    var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            //    var jobPostedEvent = events.Where(x => x.EventId == 2).First();
                            //    if (jobPostedEvent.EmailAlert)
                            //    {
                            //        var sub = _userService.GetSubjectsForNotifications(user.UserId);
                            //        var subjects = sub.Where(x => x.TeacherSpecialityId == absenceDetail.SpecialityTypeId).FirstOrDefault();
                            //        if (absenceDetail.OnlySubjectSpecialist && subjects != null ? subjects.SubjectNotification : true &&
                            //            absenceDetail.OnlyCertified ? user.IsCertified == 1 : true)
                            //            await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                            //    }

                            //}
                            //if (user.IsSubscribedSMS)
                            //{
                            //    message.PhoneNumber = user.PhoneNumber;
                            //    var sub = _userService.GetSubjectsForNotifications(user.UserId);
                            //    var subjects = sub.Where(x => x.TeacherSpecialityId == absenceDetail.SpecialityTypeId).FirstOrDefault();
                            //    if (absenceDetail.OnlySubjectSpecialist && subjects != null ? subjects.SubjectNotification : true &&
                            //        absenceDetail.OnlyCertified ? user.IsCertified == 1 : true)
                            //        CommunicationContainer.SMSProcessor.Process(message, (MailTemplateEnums)message.TemplateId);
                            //}
                        }
                        else if (user.RoleId == 3 && absenceDetail.AbsenceType != 2)
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

                        else if (user.RoleId == 3 && absenceDetail.AbsenceType == 2)
                        {
                            message.TemplateId = 8;
                            if (user.IsSubscribedEmail)
                            {
                                var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                                var jobPostedEvent = events.Where(x => x.EventId == 2).First();
                                if (jobPostedEvent.EmailAlert)
                                    await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                            }
                        }

                        //For Admins
                        else if ((user.RoleId == 1 || user.RoleId == 2) && absenceDetail.AbsenceType == 2)
                        {
                            message.TemplateId = 8;
                            if (user.IsSubscribedEmail)
                            {
                                var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                                var jobPostedEvent = events.Where(x => x.EventId == 2).First();
                                if (jobPostedEvent.EmailAlert)
                                    await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                            }
                        }

                        else if ((user.RoleId == 1 || user.RoleId == 2) && absenceDetail.AbsenceType != 2)
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