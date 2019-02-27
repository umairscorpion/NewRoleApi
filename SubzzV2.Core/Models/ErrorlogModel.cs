using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class ErrorlogModel
    {
        public string ErrorMessage { get; set; }
        public string ErrorType { get; set; }
        public string ErrorSource { get; set; }
        public int UserId { get; set; }
        public int ErrorPriority { get; set; }
        public int ErrorCode { get; set; }
        public int ErrorLine { get; set; }
        public string ReasonPhrase { get; set; }
    }
}
