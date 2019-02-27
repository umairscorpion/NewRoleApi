using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subzz.Integration.Core.Domain
{
	public class SmsTemplate
	{
		public int Id { set; get; }
		public string UserType { set; get; }		
		public string TextContent { set; get; }		
		public string SendTo { set; get; }		
	}
}
