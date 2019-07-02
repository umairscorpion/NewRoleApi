using Subzz.Integration.Core.Domain;
using SubzzV2.Integration.Core.Notification.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace SubzzV2.Integration.Core.Notification
{
	public class MessagingClientWrapper: IMessagingClientWrapper
	{
		private readonly TwilioRestClient _twilioRestClient;
        private readonly string SenderPhoneNumber;
        private readonly bool isActive;
        private readonly string AccountId;
        private readonly string AuthToken;
        public MessagingClientWrapper(MessagingSetting messagingSettings)
		{
            AccountId = messagingSettings.AccountId;
            AuthToken = messagingSettings.AuthToken;
            SenderPhoneNumber = messagingSettings.SenderPhoneNumber;
            isActive = messagingSettings.isActive;
        }

		public void SendMessage(string from, string to, string body)
		{
            if (isActive)
            {
                Twilio.TwilioClient.Init(AccountId, AuthToken);
                var message = MessageResource.Create(
                        to: new PhoneNumber(to),
                        from: new PhoneNumber(from),
                        body: body);
                if (message.ErrorMessage != null)
                {
                    TryToSendOnError(from, to, body);
                }
            }
		}
        public void SendMessage(string to, string body)
        {
            if (isActive)
            {
                Twilio.TwilioClient.Init(AccountId, AuthToken);
                var message = MessageResource.Create(
                        to: new PhoneNumber(to),
                        from: new PhoneNumber(SenderPhoneNumber),
                        body: body);
                if (message.ErrorMessage != null)
                {
                    var error = message.ErrorMessage;
                    TryToSendOnError(SenderPhoneNumber, to, body);
                }
            }
        }
        private void TryToSendOnError(string from, string to, string body)
		{
			try
			{
                var message = MessageResource.Create(
                        to: new PhoneNumber(to),
                        from: new PhoneNumber(from),
                        body: body);
                if (message.ErrorMessage != null)
				{
					var error = message.ErrorMessage;
				}
			}
			catch (System.Exception ex)
			{
			}
		}
	}
}
