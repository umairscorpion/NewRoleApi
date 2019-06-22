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
            //SendEmailAndSmsNotificationToAll();
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
                    SendEmailAndSmsNotificationToAll(preferredSubstitute);
                }
            }
        }
        public void SendEmailAndSmsNotificationToAll(PreferredSubstituteModel preferredSchoolModel)
        {
            AbsenceModel absenceDetail = _absenceService.GetAbsenceDetailByAbsenceId(preferredSchoolModel.AbsenceId);
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
            message.Duration = absenceDetail.DurationType == 1 ? "Full Day" : absenceDetail.DurationType == 2 ? "First Half" : absenceDetail.DurationType == 3 ? "Second Half" : "Custom";
            
            message.Photo = absenceDetail.EmployeeProfilePicUrl;
            message.Password = preferredSchoolModel.Password;
            message.UserName = preferredSchoolModel.SubstituteName;
            message.SendTo = preferredSchoolModel.EmailId;
            //For Substitutes
            message.TemplateId = 1;
            CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                //IEnumerable<SubzzV2.Core.Entities.User> users = _userService.GetAdminListByAbsenceId(Convert.ToInt32(absenceDetail.AbsenceId));
                //foreach (var User in users)
                //{
                //    try
                //    {
                //        message.Password = User.Password;
                //        message.UserName = User.FirstName;
                //        message.SendTo = User.Email;
                //        //For Substitutes
                //        if (User.RoleId == 4)
                //        {
                //            message.TemplateId = 1;
                //            CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                //        }
                //        else if (User.RoleId == 3)
                //        {
                //            message.TemplateId = 10;
                //            CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                //        }
                //        //For Admins
                //        else
                //        {
                //            message.TemplateId = 2;
                //            CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //    }
                //}
            }
    }
}
