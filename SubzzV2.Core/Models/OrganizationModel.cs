using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class OrganizationModel
    {
        public string SchoolName { get; set; }
        public string SchoolId { get; set; }
        public int SchoolDistrictId { get; set; }
        public string DistrictName { get; set; }
        public string SchoolCity { get; set; }
        public string SchoolAddress { get; set; }
        public string SchoolEmail { get; set; }
        public string SchoolPhone { get; set; }
        public string SchoolTimeZone { get; set; }
        public TimeSpan SchoolStartTime { get; set; }
        public TimeSpan School1stHalfEnd { get; set; }
        public TimeSpan School2ndHalfStart { get; set; }
        public TimeSpan SchoolEndTime { get; set; }
        public int SchoolZipCode { get; set; }
        public int SchoolEmployees { get; set; }
        public string ReleaseJobTime { get; set; }
        public string NotifyOthersTime { get; set; }
        public int DailyAbenceLimit { get; set; }
        public bool IsAbsenceLimit { get; set; }
    }
}
