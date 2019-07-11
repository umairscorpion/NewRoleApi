using Microsoft.Extensions.Configuration;
using Quartz;
using Subzz.Business.Services.Users;
using Subzz.Business.Services.Users.Interface;
using Subzz.Integration.Core.Container;
using Subzz.Integration.Core.Domain;
using Subzz.Integration.Core.Helper;
using SubzzAbsence.Business.Absence;
using SubzzAbsence.Business.Absence.Interface;
using SubzzAbsence.DataAccess.Repositories.Absence.Interface;
using SubzzV2.Core.Entities;
using SubzzV2.Core.Enum;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Subzz.Api.Custom.Notification;

namespace Subzz.Api.Schedules
{
    public class EmailJob : IJob
    {
        private IAbsenceService _absenceService;
        private IUserService _userService;
        public IConfiguration Configuration;
        Task IJob.Execute(IJobExecutionContext context)
        {
            _absenceService = new AbsenceService();
            _userService = new UserService();
            return Task.Run(() => {
               SendNotificationToPreferredSubstitutes();
            });
        }

        private CommunicationContainer _communicationContainer;
        public virtual CommunicationContainer CommunicationContainer
        {
            get
            {
                return _communicationContainer ?? (_communicationContainer = new CommunicationContainer());
            }
        }

        public void SendNotificationToPreferredSubstitutes()
        {
            SendSmsNotificationToPreferredSubstitutes();
            SendEmailAndSmsNotificationToAll();
        }

        public void SendSmsNotificationToPreferredSubstitutes()
        {
            IEnumerable<PreferredSubstituteModel> preferredSubstituteModel =  _absenceService.GetFavSubsForSendingSms(DateTime.Now);
            foreach (PreferredSubstituteModel preferredSubstitute in preferredSubstituteModel)
            {
                DateTime CreatedDate = preferredSubstitute.CreatedDate ;
                int Interval = preferredSubstitute.Interval;
                string TimeOfAbsenceCreationInString = CreatedDate.ToString("H:mm");
                TimeSpan TimeOfAbsenceCreation = TimeSpan.Parse(TimeOfAbsenceCreationInString);
                TimeSpan IntervalIntime = TimeSpan.FromMinutes(Interval);
                TimeSpan ActualTimetoSendTextMessage = TimeOfAbsenceCreation.Add(IntervalIntime);
                var currentDateTime = DateTime.Now;
                var currentTime = currentDateTime.ToString("H:mm");
                TimeSpan CurrentTimeToCompare = TimeSpan.Parse(currentTime);
                if (CurrentTimeToCompare >= ActualTimetoSendTextMessage)
                {
                    _absenceService.UpdateMailAndSmsFlag(preferredSubstitute.Id, true, true);
                    SendEmailAndSmsNotificationToPreferredSubstitute(preferredSubstitute);
                }
            }
        }
        public void SendEmailAndSmsNotificationToPreferredSubstitute(PreferredSubstituteModel preferredSchoolModel)
        {
            AbsenceModel absenceDetail = _absenceService.GetAbsenceDetailByAbsenceId(preferredSchoolModel.AbsenceId);
            Message message = new Message();
            message.ConfirmationNumber = absenceDetail.ConfirmationNumber;
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
            message.Duration = absenceDetail.DurationType == 1 ? "Full Day" : absenceDetail.DurationType == 2 ? "First Half" : absenceDetail.DurationType == 3 ? "Second Half" : "Custom";
            
            message.Photo = absenceDetail.EmployeeProfilePicUrl;
            message.Password = preferredSchoolModel.Password;
            message.UserName = preferredSchoolModel.SubstituteName;
            message.SendTo = preferredSchoolModel.EmailId;
            message.PhoneNumber = preferredSchoolModel.SubstitutePhoneNumber;
            //For Substitutes
            message.TemplateId = 1;
            CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
            CommunicationContainer.SMSProcessor.Process(message, (MailTemplateEnums)message.TemplateId);
        }

        public void SendEmailAndSmsNotificationToAll()
        {
            List<PreferredSubstituteModel> preferredSubstituteModel = _absenceService.GetFavSubsForSendingSmsAndEmail(DateTime.Now);
            foreach (PreferredSubstituteModel preferredSubstitute in preferredSubstituteModel)
            {
                DateTime CreatedDate = preferredSubstitute.CreatedDate;
                int Interval = preferredSubstitute.IntervalToSendAll;
                string TimeOfAbsenceCreationInString = CreatedDate.ToString("H:mm");
                TimeSpan TimeOfAbsenceCreation = TimeSpan.Parse(TimeOfAbsenceCreationInString);
                TimeSpan IntervalIntime = TimeSpan.FromMinutes(Interval);
                TimeSpan ActualTimetoSendEmailsToAll = TimeOfAbsenceCreation.Add(IntervalIntime);
                var currentDateTime = DateTime.Now;
                var currentTime = currentDateTime.ToString("H:mm");
                TimeSpan CurrentTimeToCompare = TimeSpan.Parse(currentTime);
                if (CurrentTimeToCompare >= ActualTimetoSendEmailsToAll)
                {
                    _absenceService.UpdateNotificationflagForAll(preferredSubstitute.AbsenceId);
                    SendJobPostEmails(preferredSubstitute.AbsenceId, preferredSubstituteModel);
                    return;
                }
            }
        }
        public void SendJobPostEmails(int AbsenceId, List<PreferredSubstituteModel> preferredSubstituteModel)
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
            message.Notes = absenceDetail.SubstituteNotes;
            message.SubstituteName = absenceDetail.SubstituteName;
            message.Duration = absenceDetail.DurationType == 1 ? "Full Day" : absenceDetail.DurationType == 2 ? "First Half" : absenceDetail.DurationType == 3 ? "Second Half" : "Custom";
            message.TemplateId = 15;
            message.Photo = absenceDetail.EmployeeProfilePicUrl;
            foreach (var user in users)
            {
                try
                {
                    message.Password = user.Password;
                    message.UserName = user.FirstName;
                    message.SendTo = user.Email;
                    if (user.IsSubscribedSMS && user.RoleId == 4 && absenceDetail.SubstituteRequired)
                    {
                        if (!preferredSubstituteModel.Any(preferredSub => preferredSub.SubstituteId == user.UserId))
                        {
                            var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            var jobPostedEvent = events.Where(x => x.EventId == 5).First();
                            message.PhoneNumber = user.PhoneNumber;
                            if (jobPostedEvent.EmailAlert)
                                CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                            if (jobPostedEvent.TextAlert)
                                CommunicationContainer.SMSProcessor.Process(message, (MailTemplateEnums)message.TemplateId);
                        }
                    }
                    else if (user.IsSubscribedEmail && user.RoleId != 4 && user.RoleId != 3)
                    {
                        var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                        var jobPostedEvent = events.Where(x => x.EventId == 7).First();
                        if (jobPostedEvent.EmailAlert)
                            CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
    