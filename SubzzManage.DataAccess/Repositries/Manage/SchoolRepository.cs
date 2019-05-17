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
    public class SchoolRepository : ISchoolRepository
    {
        public SchoolRepository()
        {
        }
        public SchoolRepository(IConfiguration configuration)
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
        public OrganizationModel InsertSchool(OrganizationModel model)
        {
            var sql = "[Location].[InsertAndUpdateSchool]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@SchoolId", model.SchoolId);
            queryParams.Add("@SchoolName", model.SchoolName);
            queryParams.Add("@SchoolEmail", model.SchoolEmail);
            queryParams.Add("@SchoolDistrictId", model.SchoolDistrictId);
            queryParams.Add("@SchoolAddress", model.SchoolAddress);
            queryParams.Add("@SchoolEmployees", model.SchoolEmployees);
            queryParams.Add("@SchoolTimeZone", model.SchoolTimeZone);
            queryParams.Add("@SchoolStartTime", model.SchoolStartTime);
            queryParams.Add("@SchoolEndTime", model.SchoolEndTime);
            queryParams.Add("@School1stHalfEnd", model.School1stHalfEnd);
            queryParams.Add("@School2ndHalfStart", model.School2ndHalfStart);
            queryParams.Add("@SchoolCity", model.SchoolCity);
            queryParams.Add("@SchoolPhone", model.SchoolPhone);
            queryParams.Add("@SchoolZipCode", model.SchoolZipCode);
            queryParams.Add("@IsActive", model.IsActive);
            queryParams.Add("@ReleaseJobTime", model.ReleaseJobTime);
            queryParams.Add("@NotifyOthersTime", model.NotifyOthersTime);
            queryParams.Add("@DailyAbenceLimit", model.DailyAbenceLimit);
            queryParams.Add("@IsAbsenceLimit", model.IsAbsenceLimit);
            Db.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            return model;
        }
        public OrganizationModel UpdateSchool(OrganizationModel model)
        {
            try
            {
                var sqll = "[Location].[InsertAndUpdateSchool]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@SchoolId", model.SchoolId);
                queryParams.Add("@SchoolName", model.SchoolName);
                queryParams.Add("@SchoolEmail", model.SchoolEmail);
                queryParams.Add("@SchoolDistrictId", model.SchoolDistrictId);
                queryParams.Add("@SchoolAddress", model.SchoolAddress);
                queryParams.Add("@SchoolEmployees", model.SchoolEmployees);
                queryParams.Add("@SchoolTimeZone", model.SchoolTimeZone);
                queryParams.Add("@SchoolStartTime", model.SchoolStartTime);
                queryParams.Add("@SchoolEndTime", model.SchoolEndTime);
                queryParams.Add("@School1stHalfEnd", model.School1stHalfEnd);
                queryParams.Add("@School2ndHalfStart", model.School2ndHalfStart);
                queryParams.Add("@SchoolCity", model.SchoolCity);
                queryParams.Add("@SchoolPhone", model.SchoolPhone);
                queryParams.Add("@SchoolZipCode", model.SchoolZipCode);
                queryParams.Add("@ReleaseJobTime", model.ReleaseJobTime);
                queryParams.Add("@NotifyOthersTime", model.NotifyOthersTime);
                queryParams.Add("@DailyAbenceLimit", model.DailyAbenceLimit);
                queryParams.Add("@IsAbsenceLimit", model.IsAbsenceLimit);
                queryParams.Add("@IsActive", model.IsActive);
                Db.ExecuteScalar<int>(sqll, queryParams, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {

            }
            
            return model;
        }
        public IEnumerable<OrganizationModel> GetSchools()
        {
            var sql = "[Location].[GetSchools]";
            var queryParams = new DynamicParameters();
            return Db.Query<OrganizationModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }
        public bool DeleteSchool(string SchoolId)
        {
            int hasSucceeded = 0;
            var sql = "[Location].[DeleteSchool]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@SchoolId", SchoolId);
            queryParams.Add("@HasSucceeded", hasSucceeded, null, ParameterDirection.Output);
            var result = Delete(sql, queryParams, CommandType.StoredProcedure);
            return result;

        }

        public IEnumerable<OrganizationModel> GetSchool(string SchoolId)
        {
            var sql = "[Location].[GetSchoolById]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@SchoolId", SchoolId);
            return Db.Query<OrganizationModel>(sql, queryParams, commandType: CommandType.StoredProcedure).ToList();
            
        }

        public IEnumerable<OrganizationModel> GetOrganizationsByDistrictId(int districtId)
        {
            var sql = "[Location].[GetOrganizationsByDistrictId]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@districtId", districtId);
            return Db.Query<OrganizationModel>(sql, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }

        public LocationTime GetOrganizationTimeByOrganizationId(string OrganizationId)
        {
            var sql = "[Location].[GetOrganizationTimeByOrganizationId]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@OrganizationId", OrganizationId);
            return Db.Query<LocationTime>(sql, queryParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public bool Delete(string sql, DynamicParameters param, CommandType commandType)
        {
            Db.Execute(sql, param: param, commandType: commandType);
            return Convert.ToBoolean(param.Get<int>("@HasSucceeded"));
        }
    }
}
