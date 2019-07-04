using Subzz.Integration.Core.Container;
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
        private CommunicationContainer _communicationContainer;
        public virtual CommunicationContainer CommunicationContainer
        {
            get
            {
                return _communicationContainer ?? (_communicationContainer = new CommunicationContainer());
            }
        }
        public MessagingClientWrapper(MessagingSetting messagingSettings)
		{
            AccountId = messagingSettings.AccountId;
            AuthToken = messagingSettings.AuthToken;
            SenderPhoneNumber = messagingSettings.SenderPhoneNumber;
            isActive = messagingSettings.isActive;
        }

		public void SendMessage(string from, string to, string body, int absenceId)
		{
            try
            {
                if (isActive)
                {
                    Twilio.TwilioClient.Init(AccountId, AuthToken);
                    var message = MessageResource.Create(
                            to: new PhoneNumber(to),
                            from: new PhoneNumber(from),
                            body: body);
                    var a = message.Sid;
                    var b = message.Status;
                    var c = message.ErrorMessage;
                    var d = message.ErrorCode;
                    var e = message.AccountSid;
                    CommunicationContainer.Logger.LogSms(to, body, DateTime.Now, Convert.ToString(absenceId), from, null, "OK", message.Sid);
                    if (message.ErrorMessage != null)
                    {
                        TryToSendOnError(from, to, body, absenceId);
                    }
                }
            }
            catch(Exception ex)
            {
                CommunicationContainer.Logger.LogSms(to, body, DateTime.Now, Convert.ToString(absenceId), from, Convert.ToString(ex), "Not Sent", null);
            }
            finally
            {

            }
		}
        public void SendMessage(string to, string body, int absenceId)
        {
            try
            {
                if (isActive)
                {
                    Twilio.TwilioClient.Init(AccountId, AuthToken);
                    var message = MessageResource.Create(
                            to: new PhoneNumber(to),
                            from: new PhoneNumber(SenderPhoneNumber),
                            body: body);
                    var a = message.Sid;
                    var b = message.Status;
                    var c = message.ErrorMessage;
                    var d = message.ErrorCode;
                    var e = message.AccountSid;
                    CommunicationContainer.Logger.LogSms(to, body, DateTime.Now, Convert.ToString(absenceId), SenderPhoneNumber, null, "OK", message.Sid);
                    if (message.ErrorMessage != null)
                    {
                        var error = message.ErrorMessage;
                        TryToSendOnError(SenderPhoneNumber, to, body, absenceId);
                    }
                }
            }
            catch(Exception ex)
            {
                CommunicationContainer.Logger.LogSms(to, body, DateTime.Now, Convert.ToString(absenceId), SenderPhoneNumber, Convert.ToString(ex), "Not Sent", null);
            }
            finally
            {

            }
        }
        private void TryToSendOnError(string from, string to, string body, int absenceId)
		{
			try
			{
                var message = MessageResource.Create(
                        to: new PhoneNumber(to),
                        from: new PhoneNumber(from),
                        body: body);
                CommunicationContainer.Logger.LogSms(to, body, DateTime.Now, Convert.ToString(absenceId), from, null, "OK", message.Sid);
                if (message.ErrorMessage != null)
				{
                    var error = message.ErrorMessage;
				}
			}
			catch (System.Exception ex)
			{
                CommunicationContainer.Logger.LogSms(to, body, DateTime.Now, Convert.ToString(absenceId), SenderPhoneNumber, Convert.ToString(ex), "Not Sent", null);
            }
		}
	}
}
