using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subzz.Integration.Core.Domain
{
	public class MailTemplate
	{
		public int Id { set; get; }
		public string Title { set; get; }
		public string EmailContent { set; get; }
		public bool EmailDisclaimerNeeded { set; get; }
		public string TextContent { set; get; }
		public string Notes { set; get; }
		public string SendTo { set; get; }
		public string SenderEmail { set; get; }
		public string EmailDisclaimerContent { set; get; }
		public string SupportEmail { set; get; }
	}
}
