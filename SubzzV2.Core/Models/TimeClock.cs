﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class TimeClock
    {
        public string UserId { get; set; }        public DateTime ClockInDate { get; set; }        public TimeSpan ClockInTime { get; set; }        public TimeSpan ClockOutTime { get; set; }        public TimeSpan TakeBreakTime { get; set; }        public TimeSpan BackFromBreakTime { get; set; }        public string Activity { get; set; }        public string Status { get; set; }        public int TotalHours { get; set; }        public int TotalMinutes { get; set; }        public string FirstName { get; set; }        public string LastName { get; set; }        public int TotalBreaks { get; set; }        public int TotalNoBreaks { get; set; }        public int SchoolName { get; set; }        public int DistrictId { get; set; }        public string OrganizationId { get; set; }        public string LocationId { get; set; }        public int StatusId { get; set; }        public string EmployeeName { get; set; }        public string Location { get; set; }
        public int TimeClockId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int TotalBreakTime { get; set; }

        //New Implementation
        public string UserIdd { get; set; }
        public DateTime ActivityTime { get; set; }
        public string ActivityDesc { get; set; }
        public DateTime UpdatedOn { get; set; }
        public TimeSpan BreakTime { get; set; }
        public TimeSpan ReturnFromBreakTime { get; set; }
        public int ParentId { get; set; }
        public int TotalBreakMinutes { get; set; }
        public string hoursAndMinutes { get; set; }

    }
}
