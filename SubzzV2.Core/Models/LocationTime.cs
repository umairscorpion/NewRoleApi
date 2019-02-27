using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class LocationTime
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan FirstHalfEnd { get; set; }
        public TimeSpan SecondHalfStart { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
