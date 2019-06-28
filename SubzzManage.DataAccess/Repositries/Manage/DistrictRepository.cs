using Dapper;
using Microsoft.Extensions.Configuration;
using SubzzManage.DataAccess.Repositries.Base;
using SubzzManage.DataAccess.Repositries.Manage.Interface;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzManage.DataAccess.Repositries.Manage
{
    public class DistrictRepository : IDistrictRepository
    {
        public DistrictRepository()
        {
        }
        public DistrictRepository(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected IDbConnection Db
        {
            get
            {
                return new SqlConnection(Configuration.GetConnectionString("LocationsContext"));
            }
        }
        public IConfiguration Configuration { get; }
        public DistrictModel InsertDistrict(DistrictModel model)
        {
            var sql = "[Location].[InsertAnUpdateDistrict]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", model.DistrictId);
            queryParams.Add("@DistrictName", model.DistrictName);
            queryParams.Add("@DistrictStateId", model.DistrictStateId);
            queryParams.Add("@DistrictAddress", model.DistrictAddress);
            queryParams.Add("@DistrictPhone", model.DistrictPhone);
            queryParams.Add("@DistrictEmail", model.DistrictEmail);           
            queryParams.Add("@DistrictEmployees", model.DistrictEmployees);
            queryParams.Add("@DistrictSubstitutes", model.DistrictSubstitutes);
            queryParams.Add("@DistrictWorkLoad", model.DistrictWorkLoad);
            queryParams.Add("@DistrictStaffingComp", model.DistrictStaffingComp);
            queryParams.Add("@DistrictTimeZone", model.DistrictTimeZone);
            queryParams.Add("@DistrictStartTime", model.DistrictStartTime);
            queryParams.Add("@DistrictEndTime", model.DistrictEndTime);
            queryParams.Add("@District1stHalfEnd", model.District1stHalfEnd);
            queryParams.Add("@District2ndHalfStart", model.District2ndHalfStart);
            queryParams.Add("@DistrictZipCode", model.DistrictZipCode);
            queryParams.Add("@City", model.City);
            queryParams.Add("@IsActive", model.IsActive);
            queryParams.Add("@CountryId", model.CountryId);
            model.DistrictId = Db.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            return model;
        }
        public DistrictModel UpdateDistrict(DistrictModel model)
        {

            var sql = "[Location].[InsertAnUpdateDistrict]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", model.DistrictId);
            queryParams.Add("@DistrictName", model.DistrictName);
            queryParams.Add("@DistrictStateId", model.DistrictStateId);
            queryParams.Add("@DistrictAddress", model.DistrictAddress);
            queryParams.Add("@DistrictPhone", model.DistrictPhone);
            queryParams.Add("@DistrictEmail", model.DistrictEmail);
            queryParams.Add("@DistrictEmployees", model.DistrictEmployees);
            queryParams.Add("@DistrictSubstitutes", model.DistrictSubstitutes);
            queryParams.Add("@DistrictWorkLoad", model.DistrictWorkLoad);
            queryParams.Add("@DistrictStaffingComp", model.DistrictStaffingComp);
            queryParams.Add("@DistrictTimeZone", model.DistrictTimeZone);
            queryParams.Add("@DistrictStartTime", model.DistrictStartTime);
            queryParams.Add("@DistrictEndTime", model.DistrictEndTime);
            queryParams.Add("@District1stHalfEnd", model.District1stHalfEnd);
            queryParams.Add("@District2ndHalfStart", model.District2ndHalfStart);
            queryParams.Add("@DistrictZipCode", model.DistrictZipCode);
            queryParams.Add("@City", model.City);
            queryParams.Add("@IsActive", model.IsActive);
            queryParams.Add("@CountryId", model.CountryId);
            model.DistrictId = Db.ExecuteScalar<int>(sql, queryParams, commandType: CommandType.StoredProcedure);
            return model;
        }
        public IEnumerable<DistrictModel> GetDistricts()
        {
            var sql = "[Location].[GetDistricts]";
            var queryParams = new DynamicParameters();
            return Db.Query<DistrictModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public bool DeleteDistrict(int id)
        {
            int hasSucceeded = 0;
            var sql = "[Location].[DeleteDistrict]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", id);
            queryParams.Add("@HasSucceeded", hasSucceeded, null, ParameterDirection.Output);
            var result = Delete(sql, queryParams, CommandType.StoredProcedure);
            return result;

        }
        public IEnumerable<DistrictModel> GetDistrict(int id)
        {
            var sql = "[Location].[GetDistrictById]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", id);
            return Db.Query<DistrictModel>(sql, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }

        public DistrictModel UpdateSettings(DistrictModel model)
        {
            var sql = "[Location].[sp_updateDistrictSettings]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", model.DistrictId);
            queryParams.Add("@WeeklyHourLimit", model.WeeklyHourLimit);
            queryParams.Add("@IsWeeklyLimitApplicable", model.IsWeeklyLimitApplicable ? 1: 0);
            queryParams.Add("@DeductAfterTime", model.DeductAfterTime);
            queryParams.Add("@IsDeductOnBreak", model.IsDeductOnBreak);
            return Db.Query<DistrictModel>(sql, queryParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public Allowance AddAllowance(Allowance allowance)
        {
            var sql = allowance.Id > 0 ? "[Location].[sp_updateAllowance]" : "[Location].[sp_insertAllowance]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@Id", allowance.Id);
            queryParams.Add("@DistrictId", allowance.DistrictId);
            queryParams.Add("@Title", allowance.Title);
            queryParams.Add("@YearlyAllowance", allowance.YearlyAllowance);
            queryParams.Add("@IsDeductAllowance", allowance.IsDeductAllowance);
            queryParams.Add("@IsResidualDays", allowance.IsResidualDays);
            queryParams.Add("@IsEnalbled", allowance.IsEnalbled);
            queryParams.Add("@ExpirationStartDate", allowance.ExpirationStartDate);
            queryParams.Add("@ExpirationEndDate", allowance.ExpirationEndDate);
            queryParams.Add("@IsExpired", allowance.IsExpired);
            queryParams.Add("@IsExpiredAtEndOfYear", allowance.IsExpiredAtEndOfYear);
            return Db.Query<Allowance>(sql, queryParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public IEnumerable<Allowance> GetAllowances(string districtId)
        {
            var sql = "[Location].[sp_getAllowances]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", districtId);
            return Db.Query<Allowance>(sql, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }

        public bool DeleteAllowance(int id)
        {
            int hasSucceeded = 0;
            var sql = "[Location].[sp_deleteAllowance]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@Id", id);
            queryParams.Add("@HasSucceeded", hasSucceeded, null, ParameterDirection.Output);
            var result = Delete(sql, queryParams, CommandType.StoredProcedure);
            return result;
        }

        public bool Delete(string sql, DynamicParameters param, CommandType commandType)
        {
            Db.Execute(sql, param: param, commandType: commandType);
            return Convert.ToBoolean(param.Get<int>("@HasSucceeded"));
        }
    }
}
