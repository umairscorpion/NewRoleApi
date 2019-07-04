using Subzz.Integration.Core.Container;
using Subzz.Integration.Core.Domain;
using Subzz.Integration.Core.Helper;
using SubzzV2.Core.Enum;
using SubzzV2.Integration.Core.Notification.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Integration.Core.Notification
{
	public class SMSProcessor: ISMSProcessor
	{
		private CommunicationContainer _communicationContainer;
		public virtual CommunicationContainer CommunicationContainer
		{
			get
			{
				return _communicationContainer ?? (_communicationContainer = new CommunicationContainer());
			}
		}

		public void Process(Message message, MailTemplateEnums mailTemplateEnums)
		{
            SmsTemplate smsTemplate = CommunicationContainer.MailTemplatesBuilder
                .GetSmsTemplateById((int)mailTemplateEnums);

            string expiringDates = String.Empty;
            if (message.ExpiringDates != null)
            {
                expiringDates = message.ExpiringDates.ToString();
            }

            Dictionary<string, string> param = new Dictionary<string, string>()
            {
                ["{User Name}"] = message.UserName ?? "",
                ["{Confirmation}"] = message.AbsenceId > 0 ? message.AbsenceId.ToString() : "",
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
                ["{StartDateAndTime}"] = !string.IsNullOrEmpty(message.StartTime) ? message.StartTime + " " + message.StartDate : "",
                ["{EndDateAndTime}"] = !string.IsNullOrEmpty(message.EndTime) ? message.EndTime + " " + message.EndDate : "",
                ["{Location}"] = !string.IsNullOrEmpty(message.Location) ? message.Location : "",
                ["{Notes}"] = !string.IsNullOrEmpty(message.Notes) ? message.Notes : "",
                ["{Duration}"] = message.Duration ?? "",
                ["{AcceptUrl}"] = message.AcceptUrl ?? "",
                ["{DeclineUrl}"] = message.DeclineUrl ?? "",
                ["{resetPasswordKey}"] = !string.IsNullOrEmpty(message.resetPassUrl) ? message.resetPassUrl : "",
                ["{photo}"] = !string.IsNullOrEmpty(message.ProfilePicUrl) ? message.ProfilePicUrl : "",
                ["{newLine}"] = Environment.NewLine,
            };

            string body = PrepareBodyMessage(param, smsTemplate.TextContent);

            CommunicationContainer.MessagingClient.SendMessage(message.PhoneNumber, body, message.AbsenceId);
           
		}

        public void Process(string to, string from, string message, int absenceId)
        {
            CommunicationContainer.MessagingClient.SendMessage(from, to, message, absenceId);
        }

        private string PrepareBodyMessage(Dictionary<string, string> param, string body)
		{
			foreach (var p in param)
			{
				body = body.BlindReplace(p.Key, p.Value);
			}
			return body;
		}
	}
}
