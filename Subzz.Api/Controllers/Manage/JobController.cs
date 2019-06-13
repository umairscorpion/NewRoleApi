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

        [Route("getAvailableJobs/{StartDate}/{EndDate}/{UserId}/{OrganizationId}/{DistrictId}/{Requested}/{Status}")]
        [HttpGet]
        public async Task<IEnumerable<AbsenceModel>> GetAvailableJobs(DateTime StartDate, DateTime EndDate, string UserId, string OrganizationId, int DistrictId, int Status, bool Requested)
        {
            var result = await _jobService.GetAvailableJobs(StartDate, EndDate, UserId, OrganizationId, DistrictId, Status, Requested);
            return result;
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
                    // Audit Log
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = AbsenceId.ToString(),
                        EntityType = AuditLogs.EntityType.Absence,
                        ActionType = AuditLogs.ActionType.Accepted,
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);

                    //Send Notification here
                    AbsenceModel absenceDetail = _absenceService.GetAbsenceDetailByAbsenceId(AbsenceId);
                    IEnumerable<SubzzV2.Core.Entities.User> users = _userService.GetAdminListByAbsenceId(AbsenceId);
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
                    message.Duration = absenceDetail.DurationType == 1 ? "Full Day" : absenceDetail.DurationType == 2 ? "First Half" : absenceDetail.DurationType == 3 ? "Second Half" : "Custom";
                    //Task.Run(() => SendJobAcceptEmails(users, message));
                }
                return AcceptJob;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        async Task SendJobAcceptEmails(IEnumerable<SubzzV2.Core.Entities.User> users, Subzz.Integration.Core.Domain.Message message)
        {
            foreach (var User in users)
            {
                try
                {
                    message.Password = User.Password;
                    message.UserName = User.FirstName;
                    message.SendTo = User.Email;
                    //For Substitutes
                    if (User.RoleId == 4)
                    {
                        message.TemplateId = 1;
                        //await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                    }

                    if (User.RoleId == 3)
                    {
                        message.TemplateId = 11;
                      //  await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                    }

                    //For Admins
                    else
                    {
                        message.TemplateId = 3;
                          await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                    }

                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}