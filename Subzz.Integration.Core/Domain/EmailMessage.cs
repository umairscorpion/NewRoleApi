using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Subzz.Integration.Core.Domain
{
	[DataContract]
	public class EmailMessage
	{
		[DataMember]
		public string Subject { get; set; }

		[DataMember]
		public string Body { get; set; }

		[DataMember]
		public List<string> To { get; set; }

		[DataMember]
		public List<string> Cc { get; set; }

		[DataMember]
		public List<string> Bcc { get; set; }

		[DataMember]
		public List<string> From { get; set; }
	}
}
