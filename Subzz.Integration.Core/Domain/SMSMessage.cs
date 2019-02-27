using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Subzz.Integration.Core.Domain { 

    [DataContract]
	public class SMSMessage
	{
		[DataMember]
		public string From { get; set; }
		[DataMember]
		public List<string> To { get; set; }
		[DataMember]
		public string MessageContent { get; set; }
	}
}
