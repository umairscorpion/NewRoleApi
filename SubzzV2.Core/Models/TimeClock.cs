using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class TimeClock
    {
        public string UserId { get; set; }
        public DateTime ClockInDate { get; set; }
        public TimeSpan ClockInTime { get; set; }
        public TimeSpan ClockOutTime { get; set; }
        public string Activity { get; set; }
        public string Status { get; set; }
        public int TotalHours { get; set; }
        public int TotalMinutes { get; set; }
    }
}
