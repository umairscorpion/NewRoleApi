﻿using Dapper;
using Microsoft.Extensions.Configuration;
using SubzzAbsence.DataAccess.Repositories.Absence.Interface;
using SubzzAbsence.DataAccess.Repositories.Base;
using SubzzV2.Core.Entities;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SubzzAbsence.DataAccess.Repositories.Absence
{
    public class AbsenceRepository : EntityRepository, IAbsenceRepository
    {
        //private readonly object _lock = new object();
        public AbsenceRepository()
        {
        }

        public int CreateAbsence(AbsenceModel model)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Absence].[CreateAbsence]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@AbsenceCreatedByEmployeeId", model.AbsenceCreatedByEmployeeId);
                queryParams.Add("@EmployeeId", model.EmployeeId);
                queryParams.Add("@StartDate", model.StartDate);
                queryParams.Add("@EndDate", model.EndDate);
                queryParams.Add("@StartTime", model.StartTime);
                queryParams.Add("@EndTime", model.EndTime);
                queryParams.Add("@AbsenceReasonId", model.AbsenceReasonId);
                queryParams.Add("@DurationType", model.DurationType);
                queryParams.Add("@PositionId", model.PositionId);
                queryParams.Add("@OrganizationId", model.OrganizationId);
                queryParams.Add("@Status", model.Status);
                queryParams.Add("@DistrictId", model.DistrictId);
                queryParams.Add("@RegionId", -1);
                //If Length is greater than 10 ,it means its not direct assign so we are not saving substitute ID
                queryParams.Add("@SubstituteId", model.SubstituteId.Length > 10 ? "-1" : model.SubstituteId);
                queryParams.Add("@SubstituteRequired", model.SubstituteRequired);
                queryParams.Add("@ApprovalRequired", 1);
                queryParams.Add("@AbsenceScope", model.AbsenceScope);
                queryParams.Add("@PayrollNotes", model.PayrollNotes);
                queryParams.Add("@SubstituteNotes", model.SubstituteNotes);
                queryParams.Add("@AnyAttachment", model.AnyAttachment);
                model.AbsenceId = connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
                if (model.AnyAttachment && model.AbsenceId > 0)
                {
                    sql = "[Absence].[InsertAttachment]";
                    queryParams = new DynamicParameters();
                    queryParams.Add("@AbsenceId", model.AbsenceId);
                    queryParams.Add("@AttachedFileName", model.AttachedFileName);
                    queryParams.Add("@CreateDate", DateTime.Now);
                    queryParams.Add("@Extension", model.FileExtention);
                    queryParams.Add("@ContentType", model.FileContentType);
                    connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            return model.AbsenceId;
        }

        public async Task<int> SaveAsSingleDayAbsence(DataTable Absences)
        {
            using (var connection = base.GetConnection)
            {
                var sqlBulk = new SqlBulkCopy(connection);
                sqlBulk.DestinationTableName = "Absence.AbsenceSchedule";
                Task task = sqlBulk.WriteToServerAsync(Absences);
            }
            return 1;
        }

        public IEnumerable<AbsenceModel> GetAbsences(DateTime StartDate, DateTime EndDate, string UserId)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Absence].[GetAbsences]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@StartDate", StartDate);
                queryParams.Add("@EndDate", EndDate);
                queryParams.Add("@UserId", UserId);
                return connection.Query<AbsenceModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
        }

        public IEnumerable<EmployeeSchedule> GetAbsencesScheduleEmployee(DateTime StartDate, DateTime EndDate, string UserId)
        {
            using (var connection = base.GetConnection)
            {   
                var sql = "[Absence].[GetAbsencesScheduleEmployee]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@StartDate", StartDate);
                queryParams.Add("@EndDate", EndDate);
                queryParams.Add("@UserId", UserId);
                return connection.Query<EmployeeSchedule>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
        }

        public IEnumerable<AbsenceModel> GetAbsencesByStatus(int StatusId)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Absence].[GetAbsencesByStatus]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@StatusId", StatusId);
                return connection.Query<AbsenceModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
        }

        public int UpdateAbsenceStatus(int AbsenceId, int statusId, DateTime UpdateStatusDate, string UserId)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Absence].[UpdateAbsenceStatus]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@AbsenceId", AbsenceId);
                queryParams.Add("@statusId", statusId);
                queryParams.Add("@UpdateStatusDate", UpdateStatusDate);
                queryParams.Add("@UserId", UserId);
                return connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public AbsenceModel GetAbsenceDetailByAbsenceId(int AbsenceId)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Absence].[GetAbsenceDetailByAbsenceId]";
                var queryParams = new DynamicParameters();
                return connection.Query<AbsenceModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public async Task<int> CreatePreferredAbsenceHistory(IEnumerable<User> Substitutes, AbsenceModel absence)
        {
            var sql = "[Absence].[CreatePreferredAbsenceHistory]";
            var queryParams = new DynamicParameters();
            var Interval = absence.Interval;
            var TotalInterval = absence.TotalInterval;
            int Counter = 0;
            using (var connection = base.GetConnection)
            {
                foreach(var sub in Substitutes)
                {
                    queryParams = new DynamicParameters();
                    queryParams.Add("@SubstituteId", sub.UserId);
                    queryParams.Add("@AbsenceId", absence.AbsenceId);
                    queryParams.Add("@Interval", Interval > 0 && Counter == 1 ? Interval : Interval > 0 && Counter == 0 ? 0 : 0);
                    queryParams.Add("@IntervalToSendAll", TotalInterval);
                    queryParams.Add("@IsSendSms",0);
                    queryParams.Add("@IsSendMail", 0);
                    queryParams.Add("@CreatedDate", DateTime.Now);
                    queryParams.Add("@IsSendAll", 0);
                    await connection.ExecuteAsync(sql, queryParams, commandType: CommandType.StoredProcedure);
                    Interval = Interval + Interval;
                    Counter = Counter + 1;
                }
            }
            return 1;
        }

        public IEnumerable<PreferredSubstituteModel> GetFavSubsForSendingSms(DateTime date)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Absence].[GetFavSubsForSendingSms]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@date", date);
                return connection.Query<PreferredSubstituteModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
        }

    }
}