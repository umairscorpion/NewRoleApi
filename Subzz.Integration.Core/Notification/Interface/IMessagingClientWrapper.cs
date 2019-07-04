using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Integration.Core.Notification.Interface
{
	public interface IMessagingClientWrapper
	{
		void SendMessage(string from, string to, string body, int absenceId);
        void SendMessage(string to, string body, int absenceId);
    }
}
