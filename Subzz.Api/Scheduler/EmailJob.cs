using Microsoft.Extensions.Configuration;
using Quartz;
using Subzz.Business.Services.Users;
using Subzz.Business.Services.Users.Interface;
using SubzzAbsence.Business.Absence;
using SubzzAbsence.Business.Absence.Interface;
using SubzzAbsence.DataAccess.Repositories.Absence.Interface;
using SubzzV2.Core.Entities;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
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
                    AbsenceModel absenceDetail = _absenceService.GetAbsenceDetailByAbsenceId(preferredSubstitute.AbsenceId);
                }
            }
        }
        public void SendEmailAndSmsNotificationToAll()
        {

        }
    }
}
