using Dapper;
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

        public AbsenceModel CreateAbsence(AbsenceModel model)
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
                queryParams.Add("@SubstituteId", model.SubstituteId.Length > 10 ? "-1" : model.SubstituteId.Length == 10 && model.AbsenceScope == 1 ? "-1":  model.SubstituteId);
                queryParams.Add("@SubstituteRequired", model.SubstituteRequired);
                queryParams.Add("@ApprovalRequired", model.IsApprovalRequired);
                queryParams.Add("@AbsenceScope", model.AbsenceScope);
                queryParams.Add("@PayrollNotes", model.PayrollNotes);
                queryParams.Add("@SubstituteNotes", model.SubstituteNotes);
                queryParams.Add("@AnyAttachment", model.AnyAttachment);
                queryParams.Add("@OnlyCertified", model.OnlyCertified);
                queryParams.Add("@OnlySubjectSpecialist", model.OnlySubjectSpecialist);
                var absenceConfirmation = connection.Query<AbsenceModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                model.AbsenceId = absenceConfirmation.AbsenceId;
                model.ConfirmationNumber = absenceConfirmation.ConfirmationNumber;
                if (model.AnyAttachment && model.AbsenceId > 0)
                {
                    sql = "[Absence].[InsertAttachment]";
                    queryParams = new DynamicParameters();
                    queryParams.Add("@AbsenceId", model.AbsenceId);
                    queryParams.Add("@AttachedFileName", model.AttachedFileName);
                    queryParams.Add("@OriginalFileName", model.OriginalFileName);
                    queryParams.Add("@CreateDate", DateTime.Now);
                    queryParams.Add("@Extension", model.FileExtention);
                    queryParams.Add("@ContentType", model.FileContentType);
                    connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            return model;
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

        public IEnumerable<AbsenceModel> GetAbsences(DateTime StartDate, DateTime EndDate, string UserId, string CampusId)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Absence].[GetAbsences]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@StartDate", StartDate);
                queryParams.Add("@EndDate", EndDate);
                queryParams.Add("@UserId", UserId);
                queryParams.Add("@CampusId", CampusId);
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
                queryParams.Add("@AbsenceId", AbsenceId);
                return connection.Query<AbsenceModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public async Task<int> CreatePreferredAbsenceHistory(IEnumerable<User> Substitutes, AbsenceModel absence)
        {
            var sql = "[Absence].[CreatePreferredAbsenceHistory]";
            var queryParams = new DynamicParameters();
            var Interval = absence.Interval;
            var TotalInterval = absence.TotalInterval;
            var timeDifference = absence.Interval;
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
                    Interval = Counter >= 1 ? Interval + timeDifference: Interval;
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

        public List<PreferredSubstituteModel> GetFavSubsForSendingSmsAndEmail(DateTime date)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Absence].[GetFavSubsForSendingSmsAndEmail]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@date", date);
                return connection.Query<PreferredSubstituteModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
        }

        public string UpdateAbsence(AbsenceModel model)
        {
            using (var connection = base.GetConnection)
            {
                var queryParams = new DynamicParameters();
                var sql = "[Absence].[UpdateAbsence]";
                queryParams.Add("@AbsenceID", model.AbsenceId);
                queryParams.Add("@StartDate", model.StartDate);
                queryParams.Add("@EndDate", model.EndDate);
                queryParams.Add("@StartTime", model.StartTime);
                queryParams.Add("@EndTime", model.EndTime);
                queryParams.Add("@LeaveType_Id", model.AbsenceReasonId);
                queryParams.Add("@IsSubstituteRequired", model.SubstituteRequired);
                queryParams.Add("@NotesToSubstitute", model.SubstituteNotes);
                queryParams.Add("@AbsenceStatus_Id", model.Status);
                queryParams.Add("@AbsenceType_Id", model.AbsenceType);
                queryParams.Add("@AnyAttachemt", model.AnyAttachment);
                queryParams.Add("@User_Id", model.EmployeeId);
                queryParams.Add("@UpdatedBy_User_Id", model.UpdatedById);
                queryParams.Add("@Substitute_Id", model.SubstituteId);
                queryParams.Add("@AbsenceDuration_Id", model.DurationType);
                string numberOfEffectedRow = connection.ExecuteScalar<string>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);

                if (model.AnyAttachment && model.AbsenceId > 0 && numberOfEffectedRow == "success")
                {
                    sql = "[Absence].[UpdateAttachment]";
                    queryParams = new DynamicParameters();
                    queryParams.Add("@AbsenceId", model.AbsenceId);
                    queryParams.Add("@AttachedFileName", model.AttachedFileName);
                    queryParams.Add("@OriginalFileName", model.OriginalFileName);
                    queryParams.Add("@CreateDate", DateTime.Now);
                    queryParams.Add("@Extension", model.FileExtention);
                    queryParams.Add("@ContentType", model.FileContentType);
                    connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
                }
                return numberOfEffectedRow;
            }

        }

        public int UpdateAbsenceStatusAndSub(int AbsenceId, int statusId, DateTime UpdateStatusDate, string UserId, string SubstituteId, bool SubstituteRequired)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Absence].[UpdateAbsenceStatusAndSubstitute]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@AbsenceId", AbsenceId);
                queryParams.Add("@statusId", statusId);
                queryParams.Add("@UpdateStatusDate", UpdateStatusDate);
                queryParams.Add("@UserId", UserId);
                queryParams.Add("@SubstituteId", SubstituteId);
                queryParams.Add("@SubstituteRequired", SubstituteRequired);
                return connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        //public List<AbsenceSummary> GetAbsenceSummary(string userId, int year)
        //{
        //    using (var connection = base.GetConnection)
        //    {
        //        var sql = "[Absence].[GetAbsenceSummary]";
        //        var param = new DynamicParameters();
        //        param.Add("@UserId", userId);
        //        param.Add("@Year", year);
        //        return connection.Query<AbsenceSummary>(sql, param, commandType: System.Data.CommandType.StoredProcedure).ToList();
        //    }
        //}

        public DashboardSummary GetAbsenceSummary(string userId, int year)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Subzz_Users].[Users].[DashboardSummary]";
                var param = new DynamicParameters();
                param.Add("@UserId", userId);
                param.Add("@Year", year);
                var Results = connection.QueryMultiple(sql, param, commandType: CommandType.StoredProcedure);
                DashboardSummary dasboardSummary = new DashboardSummary();
                dasboardSummary.AbsenceSummary = Results.Read<AbsenceSummary>().ToList();
                dasboardSummary.TopTenTeachers.AddRange(Results.Read<TopTenTeachers>().ToList());
                dasboardSummary.AbsenceBySubject.AddRange(Results.Read<AbsenceBySubject>().ToList());
                dasboardSummary.AbsenceByGradeLevel.AddRange(Results.Read<AbsenceByGradeLevel>().ToList());
                dasboardSummary.TopFourAbsenceReasons.AddRange(Results.Read<TopFourAbsenceReasons>().ToList());
                return dasboardSummary;
            }
        }

        public List<AbsenceSummary> GetTopTenTeachers(string userId)
        {
            using (var connection = base.GetConnection)
            {
                try
                {
                    var sql = "[Subzz_Users].[Users].[TopTenTeachers]";
                    var param = new DynamicParameters();
                    param.Add("@UserId", userId);
                    return connection.Query<AbsenceSummary>(sql, param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }
                catch (Exception ex)
                {
                }
                finally {
                }
                return null;
            }
        }

        public List<Event> GetEvents(DateTime startDate, DateTime endDate, string userId)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Subzz_Users].[Users].[GetEvents]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@StartDate", null);
                queryParams.Add("@EndDate", null);
                queryParams.Add("@UserId", userId);
                return connection.Query<Event>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
        }

        public void UpdateMailAndSmsFlag(int id, bool IsSendSms, bool IsSendEmail)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Absence].[UpdateMailAndSmsFlag]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@Id", id);
                queryParams.Add("@IsSendSms", IsSendSms);
                queryParams.Add("@IsSendEmail", IsSendEmail);
                connection.Query<Event>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
        }

        public void UpdateNotificationflagForAll (int absenceId)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Absence].[UpdateNotificationflagForAll]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@AbsenceId", absenceId);
                connection.Query<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public int GetAbsenceIdByConfirmationNumber(string ConfirmationNumber)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Absence].[getAbsenceIdByConfirmationNumber]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@ConfirmationNumber", ConfirmationNumber);
                return connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public IEnumerable<AbsenceModel> GetAbsencesForSharedCalendar(AbsenceModel model)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Absence].[GetAbsencesForSharedCalendar]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@StartDate", model.StartDate);
                queryParams.Add("@EndDate", model.EndDate);
                queryParams.Add("@UserId", model.EmployeeId);
                queryParams.Add("@DistrictId", model.DistrictId);
                queryParams.Add("@OrganizationId", model.OrganizationId);
                return connection.Query<AbsenceModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
        }

        public async Task<IEnumerable<AbsenceModel>> GetAbsencesForCalendar(DateTime StartDate, DateTime EndDate, string UserId)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Absence].[GetAbsencesForCalendar]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@StartDate", StartDate);
                queryParams.Add("@EndDate", EndDate);
                queryParams.Add("@UserId", UserId);
                return await connection.QueryAsync<AbsenceModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            }
        }
    }
}
