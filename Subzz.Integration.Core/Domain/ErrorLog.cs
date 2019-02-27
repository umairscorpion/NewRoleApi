using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subzz.Integration.Core.Domain
{
	public class ErrorLog
	{
		public string App { get; set; }

		public string ExceptionName { get; set; }

		public string LogText { get; set; }

	}
}
