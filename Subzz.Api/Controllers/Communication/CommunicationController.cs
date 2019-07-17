using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Subzz.Business.Services.Users.Interface;
using Subzz.Integration.Core.Container;
using Subzz.Integration.Core.Domain;
using Subzz.Integration.Core.Helper;
using SubzzAbsence.Business.Absence.Interface;
using SubzzV2.Core.Enum;
using SubzzV2.Core.Models;

namespace Subzz.Api.Controllers.Communication
{
    [Produces("application/json")]
    [Route("api/Communication")]
    public class CommunicationController : Controller
    {
        private readonly IUserService _service;
        private readonly IAbsenceService _absenceService;
        public CommunicationController(IUserService service, IAbsenceService absenceService)
        {
            _service = service;
            _absenceService = absenceService;
        }
        private CommunicationContainer _communicationContainer;
        public virtual CommunicationContainer CommunicationContainer
        {
            get
            {
                return _communicationContainer ?? (_communicationContainer = new CommunicationContainer());
            }
        }

        [Route("sendWellcomeLetter")]
        [HttpPost]
        public IActionResult SendWellcomeLetter(SubzzV2.Core.Entities.User user)
        {
            try
            {
                Subzz.Integration.Core.Domain.Message message = new Integration.Core.Domain.Message();
                message.Email = user.Email;
                message.EmployeeName = user.FirstName;
                message.PhoneNumber = user.PhoneNumber;
                //CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                return Ok();
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("ResendJob")]
        [HttpPost]
        public IActionResult ResendJob([FromBody]AbsenceModel model)
        {
            try
            {
                AbsenceModel absenceDetail = _absenceService.GetAbsenceDetailByAbsenceId(model.AbsenceId);
                IEnumerable<SubzzV2.Core.Entities.User> users = _service.GetAdminListByAbsenceId(model.AbsenceId);
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
                message.School = absenceDetail.OrganizationName;
                message.Photo = absenceDetail.EmployeeProfilePicUrl;
                message.Duration = absenceDetail.DurationType == 1 ? "Full Day" : absenceDetail.DurationType == 2 ? "First Half" : absenceDetail.DurationType == 3 ? "Second Half" : "Custom";
                
                message.TemplateId = 10;
                message.Photo = absenceDetail.EmployeeProfilePicUrl;
                Task.Run(() => SendEmails(users, message, absenceDetail));
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        async Task SendEmails(IEnumerable<SubzzV2.Core.Entities.User> users, Subzz.Integration.Core.Domain.Message message, AbsenceModel absenceModel)
        {
            if (absenceModel.IsApprovalRequired && absenceModel.SubstituteRequired)
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
                            message.TemplateId = 1;
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
                            message.TemplateId = 10;
                            if (user.IsSubscribedEmail)
                            {
                                await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                            }
                        }

                        //For Admins
                        else
                        {
                            message.TemplateId = 2;
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
        }
    }
}