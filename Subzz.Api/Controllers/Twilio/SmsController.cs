using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;
using Microsoft.AspNetCore.Mvc;
using SubzzManage.Business.Manage.Interface;
using SubzzAbsence.Business.Absence.Interface;
using Subzz.Business.Services.Users.Interface;
using System.Threading.Tasks;
using System;
using Subzz.Integration.Core.Container;
using System.Linq;
using System.Collections.Generic;
using Subzz.Integration.Core.Domain;
using System.Globalization;
using SubzzV2.Core.Enum;
using Subzz.Integration.Core.Helper;
using Twilio.Clients;
using SubzzV2.Core.Models;

namespace Subzz.Api.Controllers.Twilio
{
    [Route("api/[controller]")]
    public class SmsController : TwilioController
    {
        private readonly IJobService _jobService;
        private readonly IAbsenceService _absenceService;
        private readonly IUserService _userService;
        private readonly IAuditingService _audit;
        public SmsController(IJobService jobService, IAbsenceService absenceService, IUserService userService, IAuditingService audit)
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

        [HttpGet]
        public async Task<TwiMLResult> RecieveSms(SmsRequest incomingMessage)
        {
            var messagingResponse = new MessagingResponse();
            //Confirmation Number is on 0 Index and (A => Accept, D => Decline, R => Release) are on first Index 
            string[] body = incomingMessage.Body.Split(' ');
            if (body.Length == 2)
            {
                var AbsenceId = _absenceService.GetAbsenceIdByConfirmationNumber(body[0]);
                if (AbsenceId == 0)
                {
                    //CommunicationContainer.SMSProcessor.Process(incomingMessage.From, incomingMessage.To, "Invalid entry! Please enter correct Job ID", AbsenceId);
                    messagingResponse.Message("Invalid entry! Please enter correct Job ID.");
                    return TwiML(messagingResponse);
                }
                var userId = _userService.GetUserIdByPhoneNumber(incomingMessage.From);
                if (string.IsNullOrEmpty(userId))
                {
                    //CommunicationContainer.SMSProcessor.Process(incomingMessage.From, incomingMessage.To, "You're not register user", AbsenceId);
                    messagingResponse.Message("You're not register user");
                    return TwiML(messagingResponse);
                }
                var absenceDetail = _absenceService.GetAbsenceDetailByAbsenceId(AbsenceId);
                if (absenceDetail != null && absenceDetail.AbsenceId > 0)
                {
                    if (body[1].ToLower() == "a")
                    {
                        string status = await _jobService.AcceptJob(absenceDetail.AbsenceId, userId, "Sms");
                        if (status.ToLower() == "success")
                        {
                            IEnumerable<SubzzV2.Core.Entities.User> users = _userService.GetAdminListByAbsenceId(AbsenceId);
                            Message message = new Message();
                            message.ConfirmationNumber = absenceDetail.ConfirmationNumber;
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

                            message.EmployeeName = absenceDetail.EmployeeName;
                            message.Position = absenceDetail.PositionDescription;
                            message.Subject = absenceDetail.SubjectDescription;
                            message.Grade = absenceDetail.Grade;
                            message.Location = absenceDetail.AbsenceLocation;
                            message.School = absenceDetail.OrganizationName;
                            message.Notes = absenceDetail.SubstituteNotes;
                            message.SubstituteName = absenceDetail.SubstituteName;
                            message.Reason = absenceDetail.AbsenceReasonDescription;
                            message.Photo = absenceDetail.EmployeeProfilePicUrl;
                            message.Duration = absenceDetail.DurationType == 1 ? "Full Day" : absenceDetail.DurationType == 2 ? "First Half" : absenceDetail.DurationType == 3 ? "Second Half" : "Custom";
                            //Notification notification = new Notification();
                            Task.Run(() => SendJobAcceptEmails(users, message));
                        }
                        else if (status.ToLower() == "blocked")
                        {
                            //CommunicationContainer.SMSProcessor.Process(incomingMessage.From, incomingMessage.To, "You Are Blocked By Employee To Accept This Job", AbsenceId);
                            messagingResponse.Message("You Are Blocked By Employee To Accept This Job");
                        }
                        else if (status.ToLower() == "cancelled")
                        {
                            //CommunicationContainer.SMSProcessor.Process(incomingMessage.From, incomingMessage.To, "Job Has Been Cancelled", AbsenceId);
                            messagingResponse.Message("Job Has Been Cancelled");
                        }
                        else if (status.ToLower() == "accepted")
                        {
                            //CommunicationContainer.SMSProcessor.Process(incomingMessage.From, incomingMessage.To, "Job Already Accepted", AbsenceId);
                            messagingResponse.Message("Job Already Accepted");
                        }
                        else if (status.ToLower() == "conflict")
                        {
                            //CommunicationContainer.SMSProcessor.Process(incomingMessage.From, incomingMessage.To, "Already Accepted Job on This Date", AbsenceId);
                            messagingResponse.Message("Already Accepted Job on This Date");
                        }
                        else if (status.ToLower() == "declined")
                        {

                            //CommunicationContainer.SMSProcessor.Process(incomingMessage.From, incomingMessage.To, "Declined Successfully", AbsenceId);
                            messagingResponse.Message("Declined Successfully");
                        }
                        else if (status.ToLower() == "unavailable")
                        {

                            //CommunicationContainer.SMSProcessor.Process(incomingMessage.From, incomingMessage.To, "You are unavailable", AbsenceId);
                            messagingResponse.Message("You are unavailable");
                        }
                        else
                        {
                            //CommunicationContainer.SMSProcessor.Process(incomingMessage.From, incomingMessage.To, "Problem Occured While Process you Request.Please Try Again Later", AbsenceId);
                            messagingResponse.Message("Problem Occured While Process your Request.Please Try Again Later");
                        }
                    }
                    else if (body[1].ToLower() == "d")
                    {
                        // Audit Log
                        var audit = new AuditLog
                        {
                            UserId = userId,
                            EntityId = body[0],
                            EntityType = AuditLogs.EntityType.Absence,
                            ActionType = AuditLogs.ActionType.Declined,
                            DistrictId = 0,
                            OrganizationId = "N/A"
                        };
                        _audit.InsertAuditLog(audit);
                        IEnumerable<SubzzV2.Core.Entities.User> users = _userService.GetAdminListByAbsenceId(AbsenceId);
                        Message message = new Message();
                        message.ConfirmationNumber = absenceDetail.ConfirmationNumber;
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

                        message.EmployeeName = absenceDetail.EmployeeName;
                        message.Position = absenceDetail.PositionDescription;
                        message.Subject = absenceDetail.SubjectDescription;
                        message.Grade = absenceDetail.Grade;
                        message.Location = absenceDetail.AbsenceLocation;
                        message.School = absenceDetail.OrganizationName;
                        message.Notes = absenceDetail.SubstituteNotes;
                        message.SubstituteName = absenceDetail.SubstituteName;
                        message.Photo = absenceDetail.EmployeeProfilePicUrl;
                        message.Duration = absenceDetail.DurationType == 1 ? "Full Day" : absenceDetail.DurationType == 2 ? "First Half" : absenceDetail.DurationType == 3 ? "Second Half" : "Custom";
                        //Notification notification = new Notification();
                        Task.Run(() => SendJobDeclinEmails(users, message));
                        //messagingResponse.Message("Job Decline Successfully");
                    }
                    else if (body[1].ToLower() == "r")
                    {
                        var acceptedJobs = await _jobService.GetAvailableJobs(absenceDetail.StartDate, absenceDetail.EndDate, userId, absenceDetail.OrganizationId, absenceDetail.DistrictId, 2, false);
                        var IsAcceptedJob = acceptedJobs.ToList().Where(absence => absence.AbsenceId == AbsenceId).FirstOrDefault();
                        if (IsAcceptedJob.AbsenceId > 0)
                        {
                            _absenceService.UpdateAbsenceStatus(AbsenceId, 1, DateTime.Now, userId);
                            var audit = new AuditLog
                            {
                                UserId = userId,
                                EntityId = body[0],
                                EntityType = AuditLogs.EntityType.Absence,
                                ActionType = AuditLogs.ActionType.Released,
                                DistrictId = absenceDetail.DistrictId,
                                OrganizationId = absenceDetail.OrganizationId == "-1" ? null : absenceDetail.OrganizationId
                            };
                            _audit.InsertAuditLog(audit);
                            Task.Run(() => SendNotificationsOnJobReleased(AbsenceId));
                            CommunicationContainer.SMSProcessor.Process(incomingMessage.From, incomingMessage.To, "Job Release Successfully.", AbsenceId);
                            //messagingResponse.Message("Job Release Successfully");
                        }
                        else
                        {
                            //CommunicationContainer.SMSProcessor.Process(incomingMessage.From, incomingMessage.To, "You are not authorized to reject this job.", AbsenceId);
                            messagingResponse.Message("You are not authorized to reject this job.");
                        }
                    }
                }
                else
                {
                    //CommunicationContainer.SMSProcessor.Process(incomingMessage.From, incomingMessage.To, "Invalid entry! Please enter correct Job ID", AbsenceId);
                    messagingResponse.Message("Invalid entry! Please enter correct Job ID.");
                }
            }
            else
            {
                //CommunicationContainer.SMSProcessor.Process(incomingMessage.From, incomingMessage.To, "Invalid Format!", Convert.ToInt32(body[0]));
                messagingResponse.Message("Not a valid format.");
            }
            return TwiML(messagingResponse);
        }

        async Task SendJobAcceptEmails(IEnumerable<SubzzV2.Core.Entities.User> users, Subzz.Integration.Core.Domain.Message message)
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
                        if (user.IsSubscribedEmail)
                        {
                            await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                        }
                        if (user.IsSubscribedSMS)
                        {
                            message.PhoneNumber = user.PhoneNumber;
                            CommunicationContainer.SMSProcessor.Process(message, (MailTemplateEnums)message.TemplateId);
                        }
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

        async Task SendNotificationsOnJobReleased(int AbsenceId)
        {
            AbsenceModel absenceDetail = _absenceService.GetAbsenceDetailByAbsenceId(AbsenceId);
            IEnumerable<SubzzV2.Core.Entities.User> users = _userService.GetAdminListByAbsenceId(AbsenceId);
            Message message = new Message();
            message.ConfirmationNumber = absenceDetail.ConfirmationNumber;
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

            message.EmployeeName = absenceDetail.EmployeeName;
            message.Position = absenceDetail.PositionDescription;
            message.Subject = absenceDetail.SubjectDescription;
            message.Grade = absenceDetail.Grade;
            message.Location = absenceDetail.AbsenceLocation;
            message.School = absenceDetail.OrganizationName;
            message.Notes = absenceDetail.SubstituteNotes;
            message.SubstituteName = absenceDetail.SubstituteName;
            message.Reason = absenceDetail.AbsenceReasonDescription;
            message.Photo = absenceDetail.EmployeeProfilePicUrl;
            message.Duration = absenceDetail.DurationType == 1 ? "Full Day" : absenceDetail.DurationType == 2 ? "First Half" : absenceDetail.DurationType == 3 ? "Second Half" : "Custom";
            message.SubstituteName = absenceDetail.SubstituteName;
            foreach (var user in users)
            {
                try
                {
                    message.UserName = user.FirstName;
                    message.SendTo = user.Email;
                    //For Substitutes
                    if (user.RoleId == 4)
                    {
                        message.TemplateId = 21;
                        if (user.IsSubscribedEmail)
                        {
                            var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            var jobPostedEvent = events.Where(x => x.EventId == 6).First();
                            if (jobPostedEvent.EmailAlert)
                                await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                        }
                        if (user.IsSubscribedSMS)
                        {
                            var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            var jobPostedEvent = events.Where(x => x.EventId == 5).First();
                            if (jobPostedEvent.TextAlert)
                                CommunicationContainer.SMSProcessor.Process(message, (MailTemplateEnums)message.TemplateId);
                        }

                    }
                    //For Employee
                    else if (user.RoleId == 3)
                    {
                        message.TemplateId = 20;
                        if (user.IsSubscribedEmail)
                        {
                            var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            var jobPostedEvent = events.Where(x => x.EventId == 6).First();
                            if (jobPostedEvent.EmailAlert)
                                await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                        }

                    }
                    //For Admins
                    else
                    {
                        message.TemplateId = 22;
                        if (user.IsSubscribedEmail)
                        {
                            var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            var jobPostedEvent = events.Where(x => x.EventId == 6).First();
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