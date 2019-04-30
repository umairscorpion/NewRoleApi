using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class ReportFilter
    {
        public string UserId { get; set; }
        public string ReportType { get; set; }
        public string ReportTitle { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string JobNumber { get; set; }
        public int EmployeeTypeId { get; set; }
        public int AbsenceTypeId { get; set; }
        public int DistrictId { get; set; }
        public int ReasonId { get; set; }
        public string EmployeeName { get; set; }
        public string OrganizationId { get; set; }
        public string DeleteAbsenceReason { get; set; }
        public int District { get; set; }
        public int Month { get; set; }
        public string Year { get; set; }
        public int AbsencePosition { get; set; }
        public string LocationId { get; set; }
    }
}
