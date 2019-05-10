using Microsoft.Extensions.Configuration;
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
        private readonly ITimeRepository _repo;        public TimeService()        {            _repo = new TimeRepository();        }        int ITimeService.InsertClockInTime(TimeClock model)        {            var clockin = _repo.InsertClockInTime(model);            return clockin;        }        int ITimeService.InsertClockOutTime(TimeClock model)        {            var clockout = _repo.InsertClockOutTime(model);            return clockout;        }        int ITimeService.TimeClockBreakStatus(TimeClock model)        {            var br = _repo.TimeClockBreakStatus(model);            return br;        }        int ITimeService.TimeClockReturnStatus(TimeClock model)        {            var ret = _repo.TimeClockReturnStatus(model);            return ret;        }        public async Task<IEnumerable<TimeClock>> GetTimeClockData(TimeClock model)        {            return await _repo.GetTimeClockData(model);        }        string ITimeService.CheckTimeClockStatus(TimeClock model)        {            var ret = _repo.CheckTimeClockStatus(model);            return ret;        }        public async Task<IEnumerable<TimeClock>> GetTimeClockSummaryWithFilter(TimeclockFilter model)        {            return await _repo.GetTimeClockSummaryWithFilter(model);        }        public async Task<IEnumerable<TimeClock>> GetTimeTrackerSummary(TimeclockFilter model)        {            return await _repo.GetTimeTrackerSummary(model);        }
        public List<TimeClock> GetTimeTrackerDataWithFilter(TimeclockFilter model)
        {
            return _repo.GetTimeTrackerDataWithFilter(model);
        }
    }
}