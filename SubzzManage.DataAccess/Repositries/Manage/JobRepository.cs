﻿using Dapper;
using SubzzManage.DataAccess.Repositries.Base;
using SubzzManage.DataAccess.Repositries.Manage.Interface;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SubzzManage.DataAccess.Repositries.Manage
{
    public class JobRepository : EntityRepository, IJobRepository
    {
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public async Task<IEnumerable<AbsenceModel>> GetAvailableJobs(DateTime StartDate, DateTime EndDate, string UserId, string OrganizationId, int DistrictId, int status, bool Requested)
        {
            using (var connection = base.GetConnection)
            {
                try
                {
                    var sql = "[Job].[GetJobs]";
                    var queryParams = new DynamicParameters();
                    queryParams.Add("@StartDate", StartDate);
                    queryParams.Add("@EndDate", EndDate);
                    queryParams.Add("@UserId", UserId);
                    queryParams.Add("@OrganizationId", OrganizationId == null ? "-1": OrganizationId);
                    queryParams.Add("@DistrictId", DistrictId);
                    queryParams.Add("@Status", status);
                    queryParams.Add("@IsAgainstAllSchool", OrganizationId?.Length == 5 ? 0 : 1);
                    queryParams.Add("@IamRequested", !Requested ? 0 : 1);
                    return await connection.QueryAsync<AbsenceModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
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

        public async Task<string> AcceptJob(int AbsenceId, string SubstituteId, string AcceptVia)
        {
            try
            {
                using (var transaction = base.GetConnection.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    var sql = "[Job].[AcceptJob]";
                    var queryParams = new DynamicParameters();
                    queryParams.Add("@AbsenceId", AbsenceId);
                    queryParams.Add("@SubstituteId", SubstituteId);
                    queryParams.Add("@AcceptVia", AcceptVia);
                    return await base.GetConnection.ExecuteScalarAsync<string>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            catch (Exception ex) { }
            finally { }
            return null;
        }
        public IEnumerable<AbsenceModel> GetRunningLate()
        {
            var connection = base.GetConnection;
            var sql = "[Job].[GetRunningLate]";
            var queryParams = new DynamicParameters();
            return connection.Query<AbsenceModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }
    }
}
