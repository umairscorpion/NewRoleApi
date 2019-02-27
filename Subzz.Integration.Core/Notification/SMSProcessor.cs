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
			var to = new string[] { };
            SmsTemplate smsTemplate = CommunicationContainer.MailTemplatesBuilder
                .GetSmsTemplateById((int)mailTemplateEnums);

			string expiringDates = String.Empty;
			if (message.ExpiringDates != null)
			{
				expiringDates = message.ExpiringDates.ToString();
			}
            Dictionary<string, string> param = new Dictionary<string, string>()
            {
                ["{First Name}"] = message.UserName,
            };

			string body = PrepareBodyMessage(param, smsTemplate.TextContent);

			CommunicationContainer.MessagingClient.SendMessage(message.PhoneNumber, body);
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
