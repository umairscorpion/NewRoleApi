using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subzz.Integration.Core.Domain
{
    public class ReferralEmail
    {
        public int UserId { get; set; }
        public string SendtoEmailList { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}