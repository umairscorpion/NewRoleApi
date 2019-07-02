using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subzz.Integration.Core.Domain
{
	public class MessagingSetting
	{
		public string AccountId { set; get; }
		public string AuthToken { set; get; }
        public string SenderPhoneNumber { set; get; }
        public bool isActive { set; get; }
    }
}