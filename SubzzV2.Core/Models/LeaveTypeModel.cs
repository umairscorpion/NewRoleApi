using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class LeaveTypeModel
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public int StartingBalance { get; set; }
        public bool IsSubtractAllowance { get; set; }
        public bool IsApprovalRequired { get; set; }
        public bool IsVisible { get; set; }
        public int DistrictId { get; set; }
        public string OrganizationId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
