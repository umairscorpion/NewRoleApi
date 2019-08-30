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
using SubzzManage.Business.Manage.Interface;
using SubzzV2.Core.Enum;
using SubzzV2.Core.Models;

namespace Subzz.Api.Controllers.Manage
{
    [Produces("application/json")]
    [Route("api/Job")]
    public class JobController : BaseApiController
    {
        private readonly IJobService _jobService, IUserService;
        private readonly IAbsenceService _absenceService;
        private readonly IUserService _userService;
        private readonly IAuditingService _audit;
        public JobController(IJobService jobService, IAbsenceService absenceService, IUserService userService, IAuditingService audit)
        {
            _jobService = jobService;
            _absenceService = absenceService;
            _userService = userService;
            _audit = audit;
        }
        private CommunicationContainer _communicationContainer;
        public virtual CommunicationContainer CommunicationContainer
        {
            get
            {
                return _communicationContainer ?? (_communicationContainer = new CommunicationContainer());
            }
        }

        [Route("getAvailableJobs")]
        [HttpPost]
        public async Task<IEnumerable<AbsenceModel>> GetAvailableJobs([FromBody]AbsenceModel absenceModel)
        {
            var result = await _jobService.GetAvailableJobs(absenceModel.StartDate, absenceModel.EndDate, absenceModel.SubstituteId, absenceModel.OrganizationId, absenceModel.DistrictId, absenceModel.Status, absenceModel.Requested);
            return result;
        }

        [Route("getRunningLate")]
        [HttpGet]
        public IEnumerable<AbsenceModel> GetRunningLate()
        {
            try
            {
                var msg = _jobService.GetRunningLate();
                return msg;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;

        }

        [Route("acceptJob/{AbsenceId}/{SubstituteId}/{AcceptVia}")]
        [HttpGet]
        public async Task<string> AcceptJob(int AbsenceId, string SubstituteId, string AcceptVia)
        {
            try
            {
                string AcceptJob = await _jobService.AcceptJob(AbsenceId, SubstituteId, AcceptVia);
                if (AcceptJob == "success")
                {
                    //Send Notification here
                    AbsenceModel absenceDetail = _absenceService.GetAbsenceDetailByAbsenceId(AbsenceId);
                    IEnumerable<SubzzV2.Core.Entities.User> users = _userService.GetAdminListByAbsenceId(AbsenceId);
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
                    message.Photo = absenceDetail.SubstituteProfilePicUrl;
                    message.AttachedFileName = absenceDetail.AttachedFileName;
                    message.FileContentType = absenceDetail.FileContentType;
                    message.Duration = absenceDetail.DurationType == 1 ? "Full Day" : absenceDetail.DurationType == 2 ? "First Half" : absenceDetail.DurationType == 3 ? "Second Half" : "Custom";
                    //Notification notification = new Notification();
                    Task.Run(() => SendJobAcceptEmails(users, message, absenceDetail));

                    // Audit Log
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = absenceDetail.ConfirmationNumber.ToString(),
                        EntityType = AuditLogs.EntityType.Absence,
                        ActionType = AuditLogs.ActionType.Accepted,
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
                return AcceptJob;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        [Route("declineJob/{AbsenceId}")]
        [HttpGet]
        public string DeclineJob(int AbsenceId)
        {
            try
            {
                AbsenceModel absenceDetail = _absenceService.GetAbsenceDetailByAbsenceId(AbsenceId);
                IEnumerable<SubzzV2.Core.Entities.User> users = _userService.GetAdminListByAbsenceId(AbsenceId);
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
                message.Photo = absenceDetail.EmployeeProfilePicUrl;
                message.AttachedFileName = absenceDetail.AttachedFileName;
                message.FileContentType = absenceDetail.FileContentType;
                message.Duration = absenceDetail.DurationType == 1 ? "Full Day" : absenceDetail.DurationType == 2 ? "First Half" : absenceDetail.DurationType == 3 ? "Second Half" : "Custom";
                //Notification notification = new Notification();
                if(absenceDetail.AbsenceType == 2)
                    Task.Run(() => SendJobDeclinEmails(users, message));

                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = absenceDetail.ConfirmationNumber.ToString(),
                    EntityType = AuditLogs.EntityType.Absence,
                    ActionType = AuditLogs.ActionType.Declined,
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

                return "Declined";
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        async Task SendJobAcceptEmails(IEnumerable<SubzzV2.Core.Entities.User> users, Subzz.Integration.Core.Domain.Message message, AbsenceModel absenceDetail)
        {
            foreach (var user in users)
            {
                try
                {
                    message.Password = user.Password;
                    message.UserName = user.FirstName;
                    message.SendTo = user.Email;
                    //For Substitutes
                    if (user.RoleId == 4)
                    {
                        message.TemplateId = 12;
                        var grade = _userService.GetGradeLevelsForNotification(user.UserId);
                        var isGradeEnabled = grade.Where(x => x.TeachingLevelId == absenceDetail.GradeId).FirstOrDefault();
                        var sub = _userService.GetSubjectsForNotifications(user.UserId);
                        var subjects = sub.Where(x => x.TeacherSpecialityId == absenceDetail.SpecialityTypeId).FirstOrDefault();
                        var cat = _userService.GetSubstituteCategories(user.UserId);
                        var categories = cat.Where(x => x.TypeId == absenceDetail.PositionId).FirstOrDefault();

                        if (user.IsSubscribedEmail && (isGradeEnabled != null ? isGradeEnabled.GradeNotification : true) &&
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

                        if (user.IsSubscribedSMS && (isGradeEnabled != null ? isGradeEnabled.GradeNotification : true) &&
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
                        //if (user.IsSubscribedEmail)
                        //{
                        //    var sub = _userService.GetSubjectsForNotifications(user.UserId);
                        //    var subjects = sub.Where(x => x.TeacherSpecialityId == absenceDetail.SpecialityTypeId).FirstOrDefault();
                        //    if (absenceDetail.OnlySubjectSpecialist && subjects != null ? subjects.SubjectNotification : true &&
                        //        absenceDetail.OnlyCertified ? user.IsCertified == 1 : true)
                        //        await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
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

                    else if (user.RoleId == 3)
                    {
                        message.TemplateId = 11;
                        if (user.IsSubscribedEmail)
                        {
                            await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                        }
                    }

                    //For Admins
                    else
                    {
                        message.TemplateId = 3;
                        if (user.IsSubscribedEmail)
                        {
                            await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                        }
                    }
                    

                }
                catch (Exception ex)
                {
                }
            }
        }

        async Task SendJobDeclinEmails(IEnumerable<SubzzV2.Core.Entities.User> users, Subzz.Integration.Core.Domain.Message message)
        {
            foreach (var user in users)
            {
                try
                {
                    message.Password = user.Password;
                    message.UserName = user.FirstName;
                    message.SendTo = user.Email;
                    //For Substitutes
                    if (user.RoleId == 4)
                    {
                        //message.TemplateId = 1;
                        //await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                    }

                    else if (user.RoleId == 3)
                    {
                        if (user.IsSubscribedEmail)
                        {
                            message.TemplateId = 24;
                            await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                        }
                    }
                    //For Admins
                    else
                    {
                        if (user.IsSubscribedEmail)
                        {
                            message.TemplateId = 23;
                            await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                        }
                    }

                }
                catch (Exception ex)
                {
                }
            }
        }
        async Task SendRunningLateEmail(IEnumerable<SubzzV2.Core.Entities.User> users, Subzz.Integration.Core.Domain.Message message, AbsenceModel absenceDetail)
        {
            foreach (var user in users)
            {
                try
                {
                    message.Password = user.Password;
                    message.UserName = user.FirstName;
                    message.SendTo = user.Email;
                    //For Admins And Substitutes
                    if (user.RoleId != 4)
                    {
                        message.TemplateId = 27;
                        if (user.IsSubscribedEmail)
                        {
                            await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                }
            }
        }
        [Route("sendRunningLateMessage/{RunningMessage}/{AbsenceId}")]
        [HttpGet]
        public async Task<string> SendRunningLateMessage(string RunningMessage, int AbsenceId)
        {
            try
            {
                //Send Notification here
                AbsenceModel absenceDetail = _absenceService.GetAbsenceDetailByAbsenceId(AbsenceId);
                IEnumerable<SubzzV2.Core.Entities.User> users = _userService.GetAdminListByAbsenceId(AbsenceId);
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
                message.EmployeeName = absenceDetail.EmployeeName;
                message.Position = absenceDetail.PositionDescription;
                message.Subject = absenceDetail.SubjectDescription;
                message.Grade = absenceDetail.Grade;
                message.Location = absenceDetail.AbsenceLocation;
                message.School = absenceDetail.OrganizationName;
                message.Notes = absenceDetail.SubstituteNotes;
                message.SubstituteName = absenceDetail.SubstituteName;
                message.Reason = absenceDetail.AbsenceReasonDescription;
                message.Photo = absenceDetail.SubstituteProfilePicUrl;
                message.AttachedFileName = absenceDetail.AttachedFileName;
                message.FileContentType = absenceDetail.FileContentType;
                message.Duration = absenceDetail.DurationType == 1 ? "Full Day" : absenceDetail.DurationType == 2 ? "First Half" : absenceDetail.DurationType == 3 ? "Second Half" : "Custom";
                message.RunningLateMessage = RunningMessage;
                //Notification notification = new Notification();
                Task.Run(() => SendRunningLateEmail(users, message, absenceDetail));

                // Audit Log
                //var audit = new AuditLog
                //{
                //    UserId = CurrentUser.Id,
                //    EntityId = absenceDetail.ConfirmationNumber.ToString(),
                //    EntityType = AuditLogs.EntityType.Absence,
                //    ActionType = AuditLogs.ActionType.Accepted,
                //    DistrictId = CurrentUser.DistrictId,
                //    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                //};
                //_audit.InsertAuditLog(audit);
                return "Sent";
            }

            catch (Exception ex)
            {

            }
            return null;
        }
    }
}