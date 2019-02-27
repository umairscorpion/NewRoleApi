using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SubzzV2.Integration.Core.Notification.Interface
{
	public interface ILogger
	{
		void LogError(System.Exception ex);
		void LogError(string exception, string messagePrefix, string appName);
		void LogError(System.Exception ex, string messagePrefix, string appName);
		void LogMailMessage(string messageId, string ipAddress, int action, bool mailboxDatabaseExists);

	}
}
