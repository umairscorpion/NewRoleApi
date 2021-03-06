﻿using Dapper;
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
    public class TimeRepository : EntityRepository, ITimeRepository
    {
        public int InsertClockInTime(TimeClock model)
        {
            using (var connection = base.GetConnection)

            {
                var sql = "[Subzz_Users].[Users].[InsertClockInInfo]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@UserId", model.UserId);
                queryParams.Add("@Date", model.ClockInDate);
                queryParams.Add("@ClockInTime", model.ClockInTime);
                queryParams.Add("@ClockOutTime", null);
                queryParams.Add("@Activity", model.Activity);
                queryParams.Add("@Status", 0);
                connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            }
            return 1;
        }

        public int InsertClockInTimeNew(TimeClock model)
        {
            using (var connection = base.GetConnection)

            {
                var sql = "[Subzz_Users].[Users].[InsertTimeClockInfo]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@UserId", model.UserIdd);
                queryParams.Add("@UpdatedOn", model.UpdatedOn);
                queryParams.Add("@ActivityTime", model.ActivityTime);
                queryParams.Add("@BreakTime", null);
                queryParams.Add("@ReturnFromBreakTime", null);
                queryParams.Add("@Activity", model.Activity);
                queryParams.Add("@ParentId", null);
                connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            }
            return 1;
        }

        public int InsertClockOutTime(TimeClock model)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Subzz_Users].[Users].[InsertClockInInfo]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@UserId", model.UserId);
                queryParams.Add("@Date", null);
                queryParams.Add("@ClockInTime", null);
                queryParams.Add("@ClockOutTime", model.ClockOutTime);
                queryParams.Add("@Activity", model.Activity);
                queryParams.Add("@Status", null);
                connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            }
            return 1;
        }

        public int InsertClockOutTimeNew(TimeClock model)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Subzz_Users].[Users].[InsertTimeClockInfo]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@UserId", model.UserIdd);
                queryParams.Add("@UpdatedOn", model.UpdatedOn);
                queryParams.Add("@ActivityTime", model.ActivityTime);
                queryParams.Add("@BreakTime", null);
                queryParams.Add("@ReturnFromBreakTime", null);
                queryParams.Add("@Activity", model.Activity);
                queryParams.Add("@ParentId", null);
                connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            }
            return 1;
        }

        public int TimeClockBreakStatus(TimeClock model)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Subzz_Users].[Users].[InsertClockInInfo]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@UserId", model.UserId);
                queryParams.Add("@Date", null);
                queryParams.Add("@ClockInTime", null);
                queryParams.Add("@ClockOutTime", null);
                queryParams.Add("@Activity", model.Activity);
                queryParams.Add("@Status", 1);
                connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            }
            return 1;
        }

        public int TimeClockBreakStatusNew(TimeClock model)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Subzz_Users].[Users].[InsertTimeClockInfo]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@UserId", model.UserIdd);
                queryParams.Add("@UpdatedOn", model.UpdatedOn);
                queryParams.Add("@ActivityTime", model.ActivityTime);
                queryParams.Add("@BreakTime", model.BreakTime);
                queryParams.Add("@ReturnFromBreakTime", null);
                queryParams.Add("@Activity", model.Activity);
                queryParams.Add("@ParentId", null);
                connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            }
            return 1;
        }

        public int TimeClockReturnStatus(TimeClock model)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Subzz_Users].[Users].[InsertClockInInfo]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@UserId", model.UserId);
                queryParams.Add("@Date", null);
                queryParams.Add("@ClockInTime", null);
                queryParams.Add("@ClockOutTime", null);
                queryParams.Add("@Activity", model.Activity);
                queryParams.Add("@Status", null);
                connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            }
            return 1;
        }

        public int TimeClockReturnStatusNew(TimeClock model)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Subzz_Users].[Users].[InsertTimeClockInfo]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@UserId", model.UserIdd);
                queryParams.Add("@UpdatedOn", model.UpdatedOn);
                queryParams.Add("@ActivityTime", model.ActivityTime);
                queryParams.Add("@BreakTime", null);
                queryParams.Add("@ReturnFromBreakTime", model.ReturnFromBreakTime);
                queryParams.Add("@Activity", model.Activity);
                queryParams.Add("@ParentId", null);
                connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            }
            return 1;
        }

        public async Task<IEnumerable<TimeClock>> GetTimeClockData(TimeClock model)
        {
            using (var connection = base.GetConnection)
            {
                try
                {
                    var sql = "[Subzz_Users].[Users].[GetTimeClockData]";
                    var queryParams = new DynamicParameters();
                    queryParams.Add("@UserId", model.UserId);
                    return await connection.QueryAsync<TimeClock>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                }
                finally
                {

                }

            }
            return null;
        }

    
        public async Task<IEnumerable<TimeClock>> GetTimeClockSummaryWithFilter(TimeclockFilter model)
        {
            using (var connection = base.GetConnection)
            {
                try
                {
                    var sql = "[Subzz_Users].[Users].[GetTimeClockData]";
                    var queryParams = new DynamicParameters();
                    queryParams.Add("@StartDate", model.StartDate);
                    queryParams.Add("@EndDate", model.EndDate);
                    queryParams.Add("@IsDaysSelected", model.IsDaysSelected);
                    queryParams.Add("@UserId", model.UserId);
                    return await connection.QueryAsync<TimeClock>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                }
                finally
                {

                }

            }
            return null;
        }
   

        public async Task<IEnumerable<TimeClock>> GetTimeTrackerSummary(TimeclockFilter model)
        {
            using (var connection = base.GetConnection)
            {
                try
                {
                    var sql = "[Subzz_Users].[Users].[GetTimeTrackerData]";
                    var queryParams = new DynamicParameters();
                    queryParams.Add("@StartDate", model.StartDate);
                    queryParams.Add("@EndDate", model.EndDate);
                    queryParams.Add("@IsDaysSelected", model.IsDaysSelected);
                    queryParams.Add("@UserId", model.UserId);
                    return await connection.QueryAsync<TimeClock>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                }
                finally
                {

                }

            }
            return null;
        }
     

        public string CheckTimeClockStatus(TimeClock model)
        {
            using (var connection = base.GetConnection)
            {
                try
                {
                    var sql = "[Subzz_Users].[Users].[CheckTimeCLockStatus]";
                    var queryParams = new DynamicParameters();
                    queryParams.Add("@UserId", model.UserId);
                    return connection.ExecuteScalar<string>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                }
                finally
                {
                }
                return null;
            }
        }

     

        public List<TimeClock> GetTimeTrackerDataWithFilter(TimeclockFilter filter)
        {
            using (var connection = base.GetConnection)
            {
                try
                {
                    var sql = "[Subzz_Users].[Users].[GetTimeTrackerData]";
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
        }

        public int UpdateTimeClockData(TimeClock model)
        {
            using (var connection = base.GetConnection)
            {
                try
                {
                    var queryParams = new DynamicParameters();
                    var sql = "[Subzz_Users].[Users].[UpdateTimeClockData]";
                    queryParams.Add("@UserId", model.UserId);
                    queryParams.Add("@TimeClockId", model.TimeClockId);
                    queryParams.Add("@ClockInDate", model.ClockInDate);
                    queryParams.Add("@ClockInTime", model.ClockInTime);
                    queryParams.Add("@ClockOutTime", model.ClockOutTime);
                    int numberOfEffectedRow = connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
                    return numberOfEffectedRow;
                }
                catch (Exception ex)
                {
                }
                finally {
                }
                return 0;
            }

        }
    }
}