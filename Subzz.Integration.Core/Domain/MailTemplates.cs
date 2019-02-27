using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subzz.Integration.Core.Domain
{
	public class MailTemplates: Dictionary<int, MailTemplate>
	{
		public void Add(MailTemplate mailTemplate)
		{
			base.Add(mailTemplate.Id, mailTemplate);
		}
		public string PrepareTextContent(int templateId, Func<string, string> function)
		{
			if (!ContainsKey(templateId))
			{
				throw new ArgumentException(string.Format("Template by {0} doesn't exist", templateId));
			}
			return function(this[templateId].TextContent);
		}
	}
}
