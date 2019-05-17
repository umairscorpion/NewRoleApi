﻿using Microsoft.Extensions.Configuration;
using SubzzAbsence.Business.Absence.Interface;
using SubzzAbsence.Business.Time.Interface;
using SubzzAbsence.DataAccess.Repositories.Time.Interface;
using SubzzAbsence.DataAccess.Repositories.Time;
using SubzzV2.Core.Entities;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzAbsence.Business.Time
{
    public class TimeService : ITimeService
    {
        private readonly ITimeRepository _repo;
        public List<TimeClock> GetTimeTrackerDataWithFilter(TimeclockFilter model)
        {
            return _repo.GetTimeTrackerDataWithFilter(model);
        }
        public int UpdateTimeClockData(TimeClock user)
        {
            return _repo.UpdateTimeClockData(user);
        }
    }
}