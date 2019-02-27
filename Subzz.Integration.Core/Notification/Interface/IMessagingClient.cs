using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Integration.Core.Notification.Interface
{
	/// <summary>
	///Generic interface that provide options to sms messages 
	/// </summary>
	public interface IMessagingClient
	{
		void SendMessage(string from, string to, string body);
        void SendMessage(string to, string body);
    }
}
