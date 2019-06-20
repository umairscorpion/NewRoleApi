using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Subzz.Business.Services.Users.Interface;
using Subzz.Integration.Core.Container;
using Subzz.Integration.Core.Domain;
using Subzz.Integration.Core.Helper;
using SubzzAbsence.Business.Absence.Interface;
using SubzzV2.Core.Entities;
using SubzzV2.Core.Enum;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Subzz.Api.Custom
{
    public class Notification
    {
        private readonly IUserService _userService;
        private readonly IAbsenceService _absenceService;
        public Notification()
        {
        }
        public Notification(IUserService userService, IAbsenceService absenceService)
        {
            _userService = userService;
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

        public delegate string SendEmail(AbsenceModel absenceModel);
        public static string ProcessRequest(AbsenceModel absenceModel)
        {
            try
            {
                const String from = "no-reply@loginsubzz.com";
                // Construct an object to contain the recipient address.
                Destination destination = new Destination();
                destination.ToAddresses = (new List<string>() { "" });
                Content emailSubject = new Content("asdasd");
                Content textBody = new Content("asdsad");
                Body body = new Body();
                body.Html = textBody;
                // Create a message with the specified subject and body.
                Amazon.SimpleEmail.Model.Message message = new Amazon.SimpleEmail.Model.Message(emailSubject, body);
                SendEmailRequest request = new SendEmailRequest(from, destination, message);
                Amazon.RegionEndpoint regionEndPoint = Amazon.RegionEndpoint.USWest2;
                AmazonSimpleEmailServiceConfig config = new AmazonSimpleEmailServiceConfig();
                config.RegionEndpoint = regionEndPoint;
                var key = "AKIAISEURJBJ3YL4J6EA";
                var sec = "7iiVlPs51EEoZw5+TjlE+z/XdEy9WCa0rvF91u6O";
                // Instantiate an Amazon SES client, which will make the service call.
                AmazonSimpleEmailServiceClient client = new AmazonSimpleEmailServiceClient(key, sec, config);
                client.SendEmailAsync(request);
            }

            catch (Exception ex)
            {
                //log into database.

                string exception = "Email Sending Failed Amazon SES";
                if (ex.StackTrace != null)
                {
                    exception = exception + "Main Stack Trace: " + ex.StackTrace;
                    exception = exception + " Main Message " + Convert.ToString(ex.Message);
                }
                if (ex.InnerException != null)
                {
                    exception = exception + "Inner Stack Trace: " + ex.StackTrace;
                    exception = exception + " Inner Message " + Convert.ToString(ex.Message);
                }


                
            }
            return "";
        }

        public async Task SendJobDeclinEmails(IEnumerable<SubzzV2.Core.Entities.User> users, Subzz.Integration.Core.Domain.Message message)
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
                        //message.TemplateId = 1;
                        //await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                    }

                    if (User.RoleId == 3)
                    {
                        message.TemplateId = 24;
                        await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                    }
                    //For Admins
                    else
                    {
                        message.TemplateId = 23;
                        await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                    }

                }
                catch (Exception ex)
                {
                }
            }
        }

        public async Task SendJobAcceptEmails(IEnumerable<SubzzV2.Core.Entities.User> users, Subzz.Integration.Core.Domain.Message message)
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
                        await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                    }

                    if (User.RoleId == 3)
                    {
                        message.TemplateId = 11;
                        await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
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

        public async Task SendNotificationsOnJobApprovedOrDenied(LeaveRequestModel leave)
        {
            AbsenceModel absenceDetail = _absenceService.GetAbsenceDetailByAbsenceId(Convert.ToInt32(leave.AbsenceId));
            Subzz.Integration.Core.Domain.Message message = new Subzz.Integration.Core.Domain.Message();
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
            var user = _userService.GetUserDetail(absenceDetail.EmployeeId);
            if (leave.IsApproved)
                message.TemplateId = 16;
            else message.TemplateId = 19;
            message.Photo = absenceDetail.EmployeeProfilePicUrl;
            try
            {
                message.Password = user.Password;
                message.UserName = user.FirstName;
                message.SendTo = user.Email;
                await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);

            }
            catch (Exception ex)
            {
            }

            if (leave.IsApproved)
            {
                IEnumerable<SubzzV2.Core.Entities.User> users = _userService.GetAdminListByAbsenceId(Convert.ToInt32(leave.AbsenceId));
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
                            await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                        }
                        else if (User.RoleId == 3)
                        {
                            message.TemplateId = 10;
                            await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                        }
                        //For Admins
                        else
                        {
                            message.TemplateId = 2;
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
}
