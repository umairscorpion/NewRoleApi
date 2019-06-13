﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class DashboardTopCounters
    {
        public int TotalCount { get; set; }
        public int Filled { get; set; }
        public int Unfilled { get; set; }
        public int NoSubRequired { get; set; }
        public int TotalPrevious { get; set; }
        public int FilledPrevious { get; set; }
        public int UnfilledPrevious { get; set; }
    }
}
