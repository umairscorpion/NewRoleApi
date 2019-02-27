using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using SubzzAbsence.DataAccess.Repositories.Base;
using SubzzAbsence.DataAccess.Repositories.Leaves.Interface;
using SubzzV2.Core.Models;

namespace SubzzAbsence.DataAccess.Repositories.Leaves
{
    public class LeaveRepository : ILeaveRepository
    {
        public LeaveRepository()
        {
        }
        public LeaveRepository(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected IDbConnection Db
        {
            get
            {
                return new SqlConnection(Configuration.GetConnectionString("LeaveContext"));
            }
        }
        public IConfiguration Configuration { get; }
        public IEnumerable<LeaveRequestModel> GetLeaveRequests(int IsApproved, int IsDenied)
        {
            var sql = "[Leaves].[GetLeaveRequests]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@IsApproved", IsApproved);
            queryParams.Add("@IsDeniend", IsDenied);
            return Db.Query<LeaveRequestModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }
        public LeaveRequestModel InsertLeaveRequest(LeaveRequestModel model)
        {
            var sql = "[Leaves].[InsertLeaveRequest]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@EmployeeId", model.EmployeeId);
            queryParams.Add("@CreatedById", model.CreatedById);
            queryParams.Add("@Description", model.Description);
            queryParams.Add("@LeaveTypeId", model.LeaveTypeId);
            queryParams.Add("@StartDate", model.StartDate);
            queryParams.Add("@EndDate", model.EndDate);
            queryParams.Add("@StartTime", model.StartTime);
            queryParams.Add("@EndTime", model.EndTime);
            Db.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            return model;
        }
        public LeaveRequestModel UpdateLeaveRequestStatus(LeaveRequestModel model)
        {
            var sql = "[Leaves].[UpdateLeaveRequestStatus]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@LeaveRequestId", model.LeaveRequestId);
            queryParams.Add("@IsApproved", model.IsApproved);
            queryParams.Add("@IsDeniend", model.IsDeniend);
            Db.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            return model;
        }

        public IEnumerable<LeaveTypeModel> GetLeaveTypes()
        {
            var sql = "[Leaves].[GetLeaveTypes]";
            var queryParams = new DynamicParameters();
            return Db.Query<LeaveTypeModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }
        public LeaveTypeModel InsertLeaveType(LeaveTypeModel model)
        {
            var sql = "[Leaves].[InsertLeaveType]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@Name", model.LeaveTypeName);
            queryParams.Add("@StartingBalance", model.StartingBalance);
            Db.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            return model;
        }
    }
}
