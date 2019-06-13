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
        public IEnumerable<LeaveRequestModel> GetLeaveRequests(int districtId, string organizationId)
        {
            var sql = "[Leaves].[GetLeaveRequests]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", districtId);
            queryParams.Add("@OrganizationId", organizationId);
            return Db.Query<LeaveRequestModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }
        public LeaveRequestModel InsertLeaveRequest(LeaveRequestModel model)
        {
            var sql = "[Leaves].[InsertLeaveRequest]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@EmployeeId", model.EmployeeId);
            queryParams.Add("@CreatedById", model.CreatedById);
            queryParams.Add("@Description", model.Description);
            queryParams.Add("@LeaveTypeId", 1);
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
            queryParams.Add("@IsArchived", model.IsArchived);
            queryParams.Add("@UserId", model.EmployeeId);
            Db.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            return model;
        }

        public IEnumerable<LeaveTypeModel> GetLeaveTypes(int districtId, string organizationId)
        {
            var sql = "[Leaves].[GetLeaveType]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@districtId", districtId);
            queryParams.Add("@organizationId", organizationId);
            return Db.Query<LeaveTypeModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
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
            queryParams.Add("@leaveTypeId", model.LeaveTypeId);
            queryParams.Add("@Name", model.LeaveTypeName);
            queryParams.Add("@StartingBalance", model.StartingBalance);
            queryParams.Add("@IsSubtractAllowance", model.IsSubtractAllowance);
            queryParams.Add("@IsApprovalRequired", model.IsApprovalRequired);
            queryParams.Add("@IsVisible", model.IsVisible);
            queryParams.Add("@AllowanceType", model.AllowanceType);
            queryParams.Add("@DistrictId", model.DistrictId);
            queryParams.Add("@OrganizationId", model.OrganizationId);
            queryParams.Add("@CreatedDate", DateTime.Now);
            queryParams.Add("@ModifiedDate", DateTime.Now);
            Db.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            return model;
        }

        public LeaveTypeModel GetleaveTypeById(int leaveTypeId)
        {
            var sql = "[Leaves].[GetleaveTypeById]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@leaveTpeId", leaveTypeId);
            return Db.Query<LeaveTypeModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
        }

        public IEnumerable<LeaveBalance> GetEmployeeLeaveBalance(LeaveBalance leaveBalance)
        {
            var sql = "[Leaves].[sp_getEmployeeLeaveBalance]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", leaveBalance.DistrictId);
            queryParams.Add("@OrgId", leaveBalance.OrganizationId);
            queryParams.Add("@Year", leaveBalance.Year);
            if (leaveBalance.UserId.Length >= 10)
            queryParams.Add("@UserId", leaveBalance.UserId);
            return Db.Query<LeaveBalance>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public IEnumerable<LeaveBalance> GetLeaveBalance(LeaveBalance leaveBalance)
        {
            var sql = "[Leaves].[sp_getBalance]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", leaveBalance.DistrictId);
            queryParams.Add("@OrgId", leaveBalance.OrganizationId);
            queryParams.Add("@Year", leaveBalance.Year);
            if (leaveBalance.UserId.Length >= 10)
                queryParams.Add("@UserId", leaveBalance.UserId);
            return Db.Query<LeaveBalance>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public int DeleteLeaveType(int leaveTypeId)
        {
            var sql = "[Leaves].[DeleteLeaveType]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@leaveTpeId", leaveTypeId);
            return Db.Execute(sql, param: queryParams, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
