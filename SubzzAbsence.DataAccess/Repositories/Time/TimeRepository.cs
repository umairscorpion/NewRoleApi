using Dapper;
using Microsoft.Extensions.Configuration;
using SubzzAbsence.DataAccess.Repositories.Absence.Interface;
using SubzzAbsence.DataAccess.Repositories.Base;
using SubzzAbsence.DataAccess.Repositories.Time.Interface;
using SubzzV2.Core.Entities;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SubzzAbsence.DataAccess.Repositories.Time
{
    public class TimeRepository : EntityRepository, ITimeRepository    {        public int InsertClockInTime(TimeClock model)        {            using (var connection = base.GetConnection)            {                var sql = "[Subzz_Users].[dbo].[InsertClockInInfo]";                var queryParams = new DynamicParameters();                queryParams.Add("@UserId", model.UserId);                queryParams.Add("@Date", model.ClockInDate);                queryParams.Add("@ClockInTime", model.ClockInTime);                queryParams.Add("@ClockOutTime", null);                queryParams.Add("@Activity", model.Activity);                queryParams.Add("@Status", 0);                connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);            }            return 1;        }        public int InsertClockOutTime(TimeClock model)        {            using (var connection = base.GetConnection)            {                var sql = "[Subzz_Users].[dbo].[InsertClockInInfo]";                var queryParams = new DynamicParameters();                queryParams.Add("@UserId", model.UserId);                queryParams.Add("@Date", null);                queryParams.Add("@ClockInTime", null);                queryParams.Add("@ClockOutTime", model.ClockOutTime);                queryParams.Add("@Activity", model.Activity);                queryParams.Add("@Status", null);                connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);            }            return 1;        }        public int TimeClockBreakStatus(TimeClock model)        {            using (var connection = base.GetConnection)            {                var sql = "[Subzz_Users].[dbo].[InsertClockInInfo]";                var queryParams = new DynamicParameters();                queryParams.Add("@UserId", model.UserId);                queryParams.Add("@Date", null);                queryParams.Add("@ClockInTime", null);                queryParams.Add("@ClockOutTime", null);                queryParams.Add("@Activity", model.Activity);                queryParams.Add("@Status", 1);                connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);            }            return 1;        }        public int TimeClockReturnStatus(TimeClock model)        {            using (var connection = base.GetConnection)            {                var sql = "[Subzz_Users].[dbo].[InsertClockInInfo]";                var queryParams = new DynamicParameters();                queryParams.Add("@UserId", model.UserId);                queryParams.Add("@Date", null);                queryParams.Add("@ClockInTime", null);                queryParams.Add("@ClockOutTime", null);                queryParams.Add("@Activity", model.Activity);                queryParams.Add("@Status", null);                connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);            }            return 1;        }        public async Task<IEnumerable<TimeClock>> GetTimeClockData(TimeClock model)        {            using (var connection = base.GetConnection)            {
                try                {                    var sql = "[Subzz_Users].[dbo].[GetTimeClockData]";                    var queryParams = new DynamicParameters();                    queryParams.Add("@UserId", model.UserId);                    return await connection.QueryAsync<TimeClock>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);                }                catch (Exception ex)                {                }                finally                {

                }            }            return null;        }        public async Task<IEnumerable<TimeClock>> GetTimeClockSummaryWithFilter(TimeclockFilter model)        {            using (var connection = base.GetConnection)            {                try                {                    var sql = "[Subzz_Users].[dbo].[GetTimeClockData1122]";                    var queryParams = new DynamicParameters();                    queryParams.Add("@StartDate", model.StartDate);                    queryParams.Add("@EndDate", model.EndDate);                    queryParams.Add("@IsDaysSelected", model.IsDaysSelected);                    queryParams.Add("@UserId", model.UserId);                    return await connection.QueryAsync<TimeClock>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);                }                catch (Exception ex)                {                }                finally                {                }            }            return null;        }        public async Task<IEnumerable<TimeClock>> GetTimeTrackerSummary(TimeclockFilter model)        {            using (var connection = base.GetConnection)            {                try                {                    var sql = "[Subzz_Users].[dbo].[GetTimeTrackerData]";                    var queryParams = new DynamicParameters();                    queryParams.Add("@StartDate", model.StartDate);                    queryParams.Add("@EndDate", model.EndDate);                    queryParams.Add("@IsDaysSelected", model.IsDaysSelected);                    queryParams.Add("@UserId", model.UserId);                    return await connection.QueryAsync<TimeClock>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);                }                catch (Exception ex)                {                }                finally                {                }            }            return null;        }        public string CheckTimeClockStatus(TimeClock model)        {            using (var connection = base.GetConnection)            {                var sql = "[Subzz_Users].[Users].[CheckTimeClockStatus]";                var queryParams = new DynamicParameters();                queryParams.Add("@UserId", model.UserId);                return connection.ExecuteScalar<string>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);            }        }

        public List<TimeClock> GetTimeTrackerDataWithFilter(TimeclockFilter filter)
        {
            using (var connection = base.GetConnection)
            {
                try
                {
                    var sql = "[Subzz_Users].[dbo].[GetTimeTrackerData1122]";
                    var param = new DynamicParameters();
                        param.Add("@FromDate", filter.FromDate);
                    param.Add("@ToDate", filter.ToDate);
                    param.Add("@LocationId", filter.OrganizationId);
                    param.Add("@DistrictId", filter.DistrictId);
                    param.Add("@UserId", filter.UserId);
                    return connection.Query<TimeClock>(sql, param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                }
                return null;
            }
        }    }
}