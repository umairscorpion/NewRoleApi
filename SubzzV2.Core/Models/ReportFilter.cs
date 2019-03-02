using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class ReportFilter
    {
        public string ReportType { get; set; }
        public string ReportTitle { get; set; }
        public string FromDate { get; set; }
        public string ToDate{ get; set; }
        public string JobNumber { get; set; }
        public int EmployeeTypeId { get; set; }
        public int AbsenceTypeId { get; set; }
        public int LocationId { get; set; }
        public int DistrictId { get; set; }
        public int ReasonId { get; set; }
        public string EmployeeName { get; set; }
    }
}
