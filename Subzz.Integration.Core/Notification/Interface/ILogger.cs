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
		void LogEmail(string emailTo, string message, string subject, string exception, DateTime updatedOn, string absenceId, string statusCode);
		void LogError(System.Exception ex, string messagePrefix, string appName);
		void LogMailMessage(string messageId, string ipAddress, int action, bool mailboxDatabaseExists);
        void LogSms(string phoneNumber, string message, DateTime sentAt, string absenceId, string senderNo);

    }
}
