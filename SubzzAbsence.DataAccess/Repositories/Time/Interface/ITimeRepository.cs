using Microsoft.Extensions.Configuration;
using SubzzV2.Core.Entities;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzAbsence.DataAccess.Repositories.Time.Interface
{
    public interface ITimeRepository
    {
        int InsertClockInTime(TimeClock model);
        int InsertClockOutTime(TimeClock model);
        int TimeClockBreakStatus(TimeClock model);
        int TimeClockReturnStatus(TimeClock model);
        Task<IEnumerable<TimeClock>> GetTimeClockData(TimeClock model);
    }
}