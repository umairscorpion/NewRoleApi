﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class TimeClock
    {
        public string UserId { get; set; }
        public int TimeClockId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}