using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class DistrictModel
    {
        public string DistrictName { get; set; }
        public int DistrictId { get; set; }
        public int DistrictStateId { get; set; }
        public string City { get; set; }
        public string DistrictAddress { get; set; }
        public string DistrictPhone { get; set; }
        public string DistrictEmail { get; set; }
        public int DistrictEmployees { get; set; }
        public int DistrictSubstitutes { get; set; }
        public int DistrictWorkLoad { get; set; }
        public int DistrictStaffingComp { get; set; }
        public int DistrictTimeZone { get; set; }
        public TimeSpan DistrictStartTime { get; set; }
        public TimeSpan DistrictEndTime { get; set; }
        public TimeSpan District1stHalfEnd { get; set; }
        public TimeSpan District2ndHalfStart { get; set; }
        public int DistrictZipCode { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public int WeeklyHourLimit { get; set; }
        public bool IsWeeklyLimitApplicable { get; set; }
        public string DeductAfterTime { get; set; }
        public bool IsDeductOnBreak { get; set; }
        public bool IsActive { get; set; }
        public string DistrictTimeZoneTitle { get; set; }
    }
}
