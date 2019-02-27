using Subzz.Integration.Core.Domain;
using SubzzV2.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Integration.Core.Notification.Interface
{
	public interface IEmailProcessor
	{
		void Process(Message message, MailTemplateEnums mailTemplateEnums);
        Task ProcessAsync(Message message, MailTemplateEnums mailTemplateEnums);
    }
}
