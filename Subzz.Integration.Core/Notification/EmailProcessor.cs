﻿using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using NETCore.Encrypt;
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
            var data = DataProtectionProvider.Create("Subzz");
            var protector = data.CreateProtector("secretAdmin@0192837465");
            try
            {
                var configurationBuilder = new ConfigurationBuilder();
                var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                configurationBuilder.AddJsonFile(path, false);
                var root = configurationBuilder.Build();
                string apiUrl = root.GetSection("URL").GetSection("api").Value;
                string web = root.GetSection("URL").GetSection("web").Value;
                //Private Key
                var desKey = root.GetSection("KEY").GetSection("SECkey").Value;

               // Encryption
                var encEmail = EncryptProvider.DESEncrypt(message.SendTo, desKey);
                var encPassword = EncryptProvider.DESEncrypt(message.Password, desKey);
                var encAbsenceId = EncryptProvider.DESEncrypt(message.AbsenceId.ToString(), desKey);
                message.ProfilePicUrl = apiUrl + "/Profile/" + message.Photo;
                message.UnsubscriptionUrl = web + "/unsubscribed/?email=" + message.SendTo;
                if (message.TemplateId == 14)
                {
                    message.ApproveUrl = web + "/?pa=" + encPassword + "&email=" + encEmail + "&job=" + encAbsenceId + "&ac=" + 3;
                    message.DenyUrl = web + "/?pa=" + encPassword + "&email=" + encEmail + "&job=" + encAbsenceId + "&ac=" + 4;
                }
                if (message.TemplateId == 1 || message.TemplateId == 7)
                {
                    message.AcceptUrl = web + "/?pa=" + encPassword + "&email=" + encEmail + "&job=" + encAbsenceId + "&ac=" + 1;
                    message.DeclineUrl = web + "/?pa=" + encPassword + "&email=" + encEmail + "&job=" + encAbsenceId + "&ac=" + 2;
                }
                if (message.TemplateId == 9)
                {
                    message.resetPassUrl = web + "/resetPassword/?email=" + message.SendTo + "&key=" + message.ActivationCode;
                }
                if (message.TemplateId == 25)
                {
                    //message.Password = EncryptProvider.DESEncrypt(message.Password, desKey);
                    //var EmailId = EncryptProvider.DESEncrypt(message.SendTo, desKey);
                    message.VerifyUrl = web + "/?pa=" + encPassword + "&email=" + encEmail + "&ac=" + 5;
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
                if (string.IsNullOrEmpty(message.AttachedFileName))
                {
                    await CommunicationContainer.MailClient.SendAsync(body, mailTemplate.Title, to,
                     mailTemplate.SenderEmail, true, message.ImageBase64);
                }
                else
                {
                    await CommunicationContainer.MailClient.SendRawEmail(body, mailTemplate.Title, to,
                    mailTemplate.SenderEmail, true, message.AttachedFileName, message.FileContentType);
                }
                DateTime updatedOn = DateTime.Now;
                CommunicationContainer.Logger.LogEmail(message.SendTo, body, mailTemplate.Notes, null, updatedOn, Convert.ToString(message.AbsenceId), "OK");
            }
            catch (System.Exception ex)
            {
                DateTime updatedOn = DateTime.Now;
                CommunicationContainer.Logger.LogEmail(message.SendTo, null, "Subzz Job Notification", Convert.ToString(ex), updatedOn, Convert.ToString(message.AbsenceId), "FAIL");
                //CommunicationContainer.Logger.LogError(ex, "Process", "EmailProcessor");
                //throw ex;
            }
            finally
            {
                protector = null;
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
                ["{Confirmation}"] = message.ConfirmationNumber ?? "N/A",
                ["{Employee Name}"] = message.EmployeeName ?? "",
                ["{Substitute Name}"] = message.SubstituteName ?? "",
                ["{Position}"] = message.Position ?? "",
                ["{Email}"] = message.SendTo ?? "",
                ["{Password}"] = message.Password ?? "",
                ["{VerifyUrl}"] = message.VerifyUrl ?? "",
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
                ["{Location}"] = !string.IsNullOrEmpty(message.School) && message.School != "N/A" ? message.School : !string.IsNullOrEmpty(message.Location) && message.School == "N/A" ? message.Location : "" ,
                ["{Notes}"] = !string.IsNullOrEmpty(message.Notes) ? message.Notes : "",
                ["{Duration}"] = message.Duration ?? "",
                ["{AcceptUrl}"] = message.AcceptUrl ?? "",
                ["{DeclineUrl}"] = message.DeclineUrl ?? "",
                ["{resetPasswordKey}"] = !string.IsNullOrEmpty(message.resetPassUrl) ? message.resetPassUrl: "",
                ["{photo}"] = !string.IsNullOrEmpty(message.ProfilePicUrl) ? message.ProfilePicUrl : "",
                ["{UnsubscriptionUrl}"] = !string.IsNullOrEmpty(message.UnsubscriptionUrl) ? message.UnsubscriptionUrl : "",
                ["{RunningLateMessage}"] = message.RunningLateMessage ?? "",
                ["{ApproveUrl}"] = message.ApproveUrl ?? "",
                ["{DenyUrl}"] = message.DenyUrl ?? "",
            };
            return param;
        }
    }
}
