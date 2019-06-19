
using Microsoft.Extensions.Configuration;
using Subzz.Integration.Core.Container;
using Subzz.Integration.Core.Domain;
using Subzz.Integration.Core.Helper;
using SubzzV2.Core.Enum;
using SubzzV2.Integration.Core.Notification.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SubzzV2.Integration.Core.Notification
{
    public class EmailProcessor : IEmailProcessor
    {

        private CommunicationContainer _communicationContainer;
        public virtual CommunicationContainer CommunicationContainer
        {
            get
            {
                return _communicationContainer ?? (_communicationContainer = new CommunicationContainer());
            }
        }

        public async Task ProcessAsync(Message message, MailTemplateEnums mailTemplateEnums)
        {
            try
            {
                var configurationBuilder = new ConfigurationBuilder();
                var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                configurationBuilder.AddJsonFile(path, false);
                var root = configurationBuilder.Build();
                string url = root.GetSection("URL").GetSection("LiveSiteUrl").Value;
                message.ProfilePicUrl = url + "/Profile/" + message.Photo;
                if (message.TemplateId == 1)
                {
                    message.AcceptUrl = url + "/?pa=" + message.Password + "&email=" + message.SendTo + "&job=" + message.AbsenceId;
                }
                if (message.TemplateId == 9)
                {
                    message.resetPassUrl = url + "/resetPassword/?key=" + message.ActivationCode + "&email=" + message.SendTo;
                }
                MailTemplate mailTemplate = await CommunicationContainer.MailTemplatesBuilder
                    .GetMailTemplateByIdAsync((int)mailTemplateEnums);
                string[] to;
                    to = new string[] { message.SendTo };
                var param = GetParam(message);
                string body = PrepareBodyMessage(param, mailTemplate.EmailContent);
                if (mailTemplate.EmailDisclaimerNeeded)
                {
                    body += mailTemplate.EmailDisclaimerContent;
                }
                await CommunicationContainer.MailClient.SendAsync(body, mailTemplate.Title, to,
                     mailTemplate.SenderEmail, true, message.ImageBase64);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public void Process(Message message, MailTemplateEnums mailTemplateEnums)
        {
            try
            {               
                //System.Diagnostics.Debugger.Launch();
                MailTemplate mailTemplate = CommunicationContainer.MailTemplatesBuilder
                    .GetMailTemplateById((int)mailTemplateEnums);
                var to = SetSendTo(mailTemplate, message);
                var param = GetParam(message);
                string body = PrepareBodyMessage(param, mailTemplate.EmailContent);

                if (mailTemplate.EmailDisclaimerNeeded)
                {
                    body += mailTemplate.EmailDisclaimerContent;
                }

                if (!string.IsNullOrEmpty(message.ImageBase64))
                    body = body.Replace("<img style=\"width: 150px\" src=\"cid:image1\"/>", "");
              
                CommunicationContainer.MailClient.Send(body, mailTemplate.Title.Replace("{DaysLeft}", "").Replace("{TimeInterval}", message.TimeInterval ?? ""), to,
                    mailTemplate.SenderEmail, true, message.ImageBase64, message.MailAttachments);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private string[] SetSendTo(MailTemplate mailTemplate, Message message)
        {
            if (mailTemplate.SendTo == "Support")
            {
                return new string[] { mailTemplate.SupportEmail };
            }
            return new string[] { message.Email };
        }
        private string PrepareBodyMessage(Dictionary<string, string> param, string body)
        {
            try
            {
                foreach (var p in param)
                {
                    body = body.BlindReplace(p.Key, p.Value);
                }
                return body;
            }
            catch (System.Exception ex)
            {
                return "Error occured due PrepareBodyMessage";
            }
        }

        private Dictionary<string, string> GetParam(Message message)
        {
            //TODO: Extract parameters validation into separate class
            string expiringDates = String.Empty;
            if (message.ExpiringDates != null)
            {
                expiringDates = message.ExpiringDates.ToString();
            }
            /*if (message.IsActivationEmail)
			{
				message.ActivationLink = message.SiteUrlLink + "/" + registrationConfirmedPath + "/" 
					+ message.ActivationCode;
			}*/
            Dictionary<string, string> param = new Dictionary<string, string>()
            {
                ["{User Name}"] = message.UserName ?? "",
                ["{Confirmation}"] = message.AbsenceId > 0 ? message.AbsenceId.ToString():  "",
                ["{Employee Name}"] = message.EmployeeName ?? "",
                ["{Substitute Name}"] = message.SubstituteName ?? "",
                ["{Position}"] = message.Position ?? "",
                ["{Start Date}"] = message.StartDate ?? "",
                ["{End Date}"] = message.EndDate ?? "",
                ["{Start Time}"] = message.StartTime ?? "",
                ["{End Time}"] = message.EndTime ?? "",
                ["{Leave Type}"] = message.Reason ?? "",
                ["{Admin Name}"] = message.ApprovedBy ?? "",
                ["{Subject}"] = !string.IsNullOrEmpty(message.Subject) ? message.Subject + "-" : "N/A-",
                ["{Grade}"] = !string.IsNullOrEmpty(message.Grade) ? message.Grade : "N/A",
                ["{StartDateAndTime}"] = !string.IsNullOrEmpty(message.StartTime)? message.StartTime + " " + message.StartDate : "",
                ["{EndDateAndTime}"] = !string.IsNullOrEmpty(message.EndTime) ? message.EndTime + " " + message.EndDate: "",
                ["{Location}"] = !string.IsNullOrEmpty(message.Location) ? message.Location: "",
                ["{Notes}"] = !string.IsNullOrEmpty(message.Notes) ? message.Notes : "N/A",
                ["{Duration}"] = message.Duration ?? "",
                ["{AcceptUrl}"] = message.AcceptUrl ?? "",
                ["{resetPasswordKey}"] = !string.IsNullOrEmpty(message.resetPassUrl) ? message.resetPassUrl: "",
                ["{photo}"] = !string.IsNullOrEmpty(message.ProfilePicUrl) ? message.ProfilePicUrl : "",
            };
            return param;
        }
    }
}
