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
            queryParams.Add("@CountryId", model.CountryId);
            queryParams.Add("@SchoolStateId", model.SchoolStateId);
            model.SchoolId = Db.ExecuteScalar<string>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            return model;
        }

        public OrganizationModel InsertSchoolTemporary(OrganizationModel model, int DistrictId, string Status)
        {
            var sql = "[Location].[InsertSchoolTemporary]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@SchoolName", model.SchoolName);
            queryParams.Add("@SchoolAddress", model.SchoolAddress);
            queryParams.Add("@SchoolStartTime", model.SchoolStartTime);
            queryParams.Add("@SchoolEndTime", model.SchoolEndTime);
            queryParams.Add("@School1stHalfEnd", model.School1stHalfEnd);
            queryParams.Add("@School2ndHalfStart", model.School2ndHalfStart);
            queryParams.Add("@SchoolCity", model.SchoolCity);
            queryParams.Add("@SchoolPhone", model.SchoolPhone);
            queryParams.Add("@SchoolZipCode", model.SchoolZipCode);
            queryParams.Add("@CountryName", model.CounrtyCode);
            queryParams.Add("@StateName", model.StateName);
            queryParams.Add("@Status", Status);
            queryParams.Add("@SchoolDistrictId", DistrictId);
            model.SchoolId = Db.ExecuteScalar<string>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
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
                queryParams.Add("@CountryId", model.CountryId);
                queryParams.Add("@SchoolStateId", model.SchoolStateId);
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
        public IEnumerable<OrganizationModel> GetTemporarySchools()
        {
            var sql = "[Location].[GetTemporarySchools]";
            var queryParams = new DynamicParameters();
            return Db.Query<OrganizationModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public int DeleteSchool(string SchoolId)
        {
            int hasSucceeded = 0;
            var sql = "[Location].[DeleteSchool]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@SchoolId", SchoolId);
            queryParams.Add("@HasSucceeded", hasSucceeded, null, ParameterDirection.Output);
            return Db.Query<int>(sql, param: queryParams, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();


        }

        public int DeleteTemporarySchools(int DistrictId)
        {
            var sql = "[Location].[DeleteTemporarySchools]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", DistrictId);
            return Db.Query<int>(sql, param: queryParams, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();


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

        public IEnumerable<AbsenceScope> GetAbsenceScopes(OrganizationModel organizationModel)
        {
            var sql = "[Location].[sp_getAbsenceTypes]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@districtId", organizationModel.SchoolDistrictId);
            queryParams.Add("@OrgId", organizationModel.SchoolId);
            return Db.Query<AbsenceScope>(sql, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }

        public AbsenceScope UpdateAbsenceScope(AbsenceScope absenceScope)
        {
            var sql = "[Location].[sp_updateAbsenceTypes]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@OrgId", absenceScope.OrganizatonId);
            queryParams.Add("@DistrictId", absenceScope.DistrictId);
            queryParams.Add("@Visibility", absenceScope.Visibility);
            queryParams.Add("@Id", absenceScope.Id);
            queryParams.Add("@absenceType", absenceScope.AbsenceType);
            return Db.Query<AbsenceScope>(sql, queryParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public bool Delete(string sql, DynamicParameters param, CommandType commandType)
        {
            Db.Execute(sql, param: param, commandType: commandType);
            return Convert.ToBoolean(param.Get<int>("@HasSucceeded"));
        }


        public int GetDistrictId(string districtName)
        {
           var sql = "[Location].[GetDistrictIdByDistrictName]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictName", districtName);
            return Db.Query<int>(sql, param: queryParams, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
        }

        public int GetCountryId(string districtName)
        {
            var sql = "[Location].[GetCountryIdByCountryName]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictName", districtName);
            return Db.Query<int>(sql, param: queryParams, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
        }

        public int GetStateId(string districtName)
        {
            var sql = "[Location].[GetStateIdByStateName]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictName", districtName);
            return Db.Query<int>(sql, param: queryParams, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
        }

        public IEnumerable<CountryModel> GetCountries()
        {
            var sql = "[Subzz_Lookups].[Lookup].[GetCountries]";
            var queryParams = new DynamicParameters();
            return Db.Query<CountryModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public IEnumerable<StateModel> GetStateByCountryId(int counrtyId)
        {
            var sql = "[Subzz_Lookups].[Lookup].[GetStatesByCounrtyId]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@counrtyId", counrtyId);
            return Db.Query<StateModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }


    }
}
