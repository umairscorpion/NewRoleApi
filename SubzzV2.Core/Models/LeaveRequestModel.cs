using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class LeaveRequestModel
    {
        public int LeaveRequestId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeId { get; set; }
        public string CreatedById { get; set; }
        public int LeaveTypeId { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDeniend { get; set; }
        public string LeaveTypeName { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string CreatedDate { get; set; }
        public string ApprovedDate { get; set; }
        public string DeniedDate { get; set; }
        public bool IsArchived { get; set; }
        public int TotalDays { get; set; }
        public string AcceptedDate { get; set; }
        public string SubstituteName { get; set; }
        public string PostedByName { get; set; }
        public string PostedOn { get; set; }
        public string AbsenceId { get; set; }
        public string ApprovedBy { get; set; }
        public string DeniedBy { get; set; }
        public string CancelledDate { get; set; }
        public string CancelledBy { get; set; }
    }
}
