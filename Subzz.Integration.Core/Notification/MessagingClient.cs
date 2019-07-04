using SubzzV2.Integration.Core.Notification.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Integration.Core.Notification
{
	public class MessagingClient: IMessagingClient
	{
		private readonly IMessagingClientWrapper _messagingClientWrapper;
		public MessagingClient(IMessagingClientWrapper messagingClientWrapper)
		{
			_messagingClientWrapper = messagingClientWrapper;
		}

		public void SendMessage(string from, string to, string body, int absenceId)
		{
			_messagingClientWrapper.SendMessage(from, to, body, absenceId);
		}
        public void SendMessage(string to, string body, int absenceId)
        {
            _messagingClientWrapper.SendMessage(to, body, absenceId);
        }
    }
}
