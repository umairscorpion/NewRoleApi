using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class LeaveBalance
    {
        public string UserId { get; set; }
        public string Personal { get; set; }
        public string Sick { get; set; }
        public string Vacation { get; set; }
        public string UserName { get; set; }
        public string OrganizationId { get; set; }
        public int DistrictId { get; set; }
        public int Year { get; set; }
    }
}
