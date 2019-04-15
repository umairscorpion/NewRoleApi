using Dapper;
using Microsoft.Extensions.Configuration;
using SubzzV2.Core.Entities;
using SubzzV2.Core.Models;
using Subzz.DataAccess.Repositories.Users.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Subzz.DataAccess.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        public UserRepository()
        {
        }
        private SqlConnection Db
        {
            get
            {
                var configurationBuilder = new ConfigurationBuilder();
                var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                configurationBuilder.AddJsonFile(path, false);
                var root = configurationBuilder.Build();
                string Conn = root.GetSection("ConnectionStrings").GetSection("MembershipContext").Value;
                SqlConnection connection = new SqlConnection(Conn);
                return connection;
            }
        }
        public IConfiguration Configuration { get; }
        public UserLogin GetUserByCredentials(string userName, string password)
        {
            var sql = "[Users].[GetUserByCredentials]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserName", userName);
            queryParams.Add("@Password", password);

            return Db.Query<UserLogin>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).SingleOrDefault();
        }

        public User GetUserDetail(string userId)
        {
            var sql = "[Users].[uspGetUserDetail]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", userId);
            return Db.Query<User>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).SingleOrDefault();
        }
        public IEnumerable<UserResource> GetUserResourses(string UserId, int resourceTypeId, int parentResourceTypeId, int IsAdminPortal)
        {
            var sql = "[Users].[GetUserResources]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", UserId);
            queryParams.Add("@ResourceTypeId", resourceTypeId);
            queryParams.Add("@ParentResourceId", parentResourceTypeId);
            queryParams.Add("@IsAdminPortal", IsAdminPortal);
            var sds =  Db.Query<UserResource>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
            return sds;
        }

        // functions related to User

        public User InsertUser(User model)
        {
            var sql = "[Users].[InsertUser]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@FirstName", model.FirstName);
            queryParams.Add("@LastName", model.LastName);
            queryParams.Add("@UserTypeId", model.UserTypeId);
            queryParams.Add("@TeacherLevel", model.TeachingLevel);
            queryParams.Add("@Speciality", model.Speciality);
            queryParams.Add("@RoleId", model.RoleId);
            queryParams.Add("@Gender", model.Gender);
            queryParams.Add("@IsCertified", model.IsCertified);
            queryParams.Add("@DistrictId", model.DistrictId);
            queryParams.Add("@OrganizationId", model.OrganizationId);
            queryParams.Add("@Email", model.Email);
            queryParams.Add("@PhoneNumber", model.PhoneNumber);
            queryParams.Add("@IsSubscribedSMS", 1);
            queryParams.Add("@IsSubscribedEmail", 1);
            queryParams.Add("@Isdeleted", 0);
            queryParams.Add("@ProfilePicture", model.ProfilePicture);
            Db.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            return model;
        }

        public User UpdateUser(User model)
        {
            var sql = "[Users].[UpdateUser]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", model.UserId);
            queryParams.Add("@FirstName", model.FirstName);
            queryParams.Add("@LastName", model.LastName);
            queryParams.Add("@UserTypeId", model.UserTypeId);
            queryParams.Add("@TeacherLevel", model.TeachingLevel);
            queryParams.Add("@Speciality", model.Speciality);
            queryParams.Add("@OrganizationId", model.OrganizationId);
            queryParams.Add("@RoleId", model.RoleId);
            queryParams.Add("@Gender", model.Gender);
            queryParams.Add("@IsCertified", model.IsCertified);
            queryParams.Add("@DistrictId", model.DistrictId);
            queryParams.Add("@Email", model.Email);
            queryParams.Add("@PhoneNumber", model.PhoneNumber);
            queryParams.Add("@Isdeleted", 0);
            queryParams.Add("@ProfilePicture", model.ProfilePicture);
            Db.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            return model;
        }

        public User UpdateEmployee(User model)
        {

            var sqll = "[Location].[UpdateEmployee]";
            var queryParamss = new DynamicParameters();
            //queryParamss.Add("@Id", model.id);
            //queryParamss.Add("@Title", model.title);
            //queryParamss.Add("@Description", model.description);
            model.UserId = Db.ExecuteScalar<string>(sqll, queryParamss, commandType: CommandType.StoredProcedure);
            return model;
        }
        
        public IEnumerable<User> GetUsers(string userId, int userRole, int districtId, string organizationId)
        {
            var sql = "[Users].[GetUsers]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@userId", userId);
            queryParams.Add("@userRole", userRole);
            queryParams.Add("@districtId", districtId);
            queryParams.Add("@organizationId", organizationId);
            return Db.Query<User>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public IEnumerable<User> GetEmployeeSuggestions(string searchText, int isSearchSubstitute, string organizationId, int districtId)
        {
            var sql = "[Users].[GetEmployeeSuggestions]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@searchText", searchText);
            queryParams.Add("@IsSearchSubstitute", isSearchSubstitute);
            queryParams.Add("@districtId", districtId);
            queryParams.Add("@organizationId", organizationId);
            return Db.Query<User>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public bool DeleteUser(string UserId)
        {
            //int hasSucceeded = 0;
            var sql = "[Users].[DeleteUser]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", UserId);
            //queryParams.Add("@HasSucceeded", hasSucceeded, null, ParameterDirection.Output);
            var result = Delete(sql, queryParams, CommandType.StoredProcedure);
            return result;

        }
        public IEnumerable<User> GetEmployee(int id)
        {
            var sql = "[Location].[GetDistricts]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@Id", id);
            return Db.Query<User>(sql, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }

        public IEnumerable<LookupModel> GetUserTypes()
        {
            var sql = "[Users].[GetUserTypes]";
            var queryParams = new DynamicParameters();
            return Db.Query<LookupModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public int InsertExternalUser(ExternalUser externalUser)
        {
            var sql = "[users].[InsertExternalUser]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@Email", externalUser.Email);
            queryParams.Add("@ExternalUserId", externalUser.Id);
            queryParams.Add("@IdToken", externalUser.IdToken);
            queryParams.Add("@Image", externalUser.Image);
            queryParams.Add("@Name", externalUser.Name);
            queryParams.Add("@Providor", externalUser.Providor);
            queryParams.Add("@Token", externalUser.Token);
            return Db.ExecuteScalar<int>(sql, queryParams, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<SubstituteCategoryModel> GetSubstituteCategories(string SubstituteId)
        {
            var sql = "[users].[GetSubstituteCategories]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@SubstituteId", SubstituteId);
            return Db.Query<SubstituteCategoryModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public int UpdateUserCategories(SubstituteCategoryModel substituteCategoryModel)
        {
            var sql = "[users].[UpdateUserCategories]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@Id", substituteCategoryModel.Id);
            queryParams.Add("@IsNotify", substituteCategoryModel.IsNotificationSend);
            return Db.ExecuteScalar<int>(sql, queryParams, commandType: CommandType.StoredProcedure);
        }

        public LocationTime GetUserLocationTime(string userId, int userLevel)
        {
            var sql = "[users].[GetUserLocationTime]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@userId", userId);
            queryParams.Add("@userLevel", userLevel);
            return Db.Query<LocationTime>(sql, queryParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public AbsenceModel GetUsersForSendingAbsenceNotificationOnEntireSub(int DistrictId, string OrganizationId, int AbsenceId, string SubstituteId)
        {
            var sql = "[users].[GetUsersForSendingAbsenceNotificationOnEntireSub]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", DistrictId);
            queryParams.Add("@OrganizationId", OrganizationId);
            queryParams.Add("@AbsenceId", AbsenceId);
            queryParams.Add("@SubstituteId", SubstituteId);
            var Results = Db.QueryMultiple(sql, queryParams, commandType: CommandType.StoredProcedure);
            AbsenceModel absenceModel = new AbsenceModel();
            absenceModel = Results.Read<AbsenceModel>().FirstOrDefault();
            absenceModel.Users.AddRange(Results.Read<User>().ToList());
            return absenceModel;
        }

        public async Task<int> UpdateSubstitutePeferrence(SubstitutePreferenceModel substitutePreferenceModel)
        {
            try
            {
                var sql = "[users].[DeleteSubstitutePreference]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@UserId", substitutePreferenceModel.UserId);
                await Db.ExecuteAsync(sql, queryParams, commandType: CommandType.StoredProcedure);

                sql = "[users].[UpdateFavSubstitutes]";
                var FavSubs = JsonConvert.DeserializeObject<List<User>>(substitutePreferenceModel.FavoriteSubstituteList);
                var BlockedSubs = JsonConvert.DeserializeObject<List<User>>(substitutePreferenceModel.BlockedSubstituteList);

                foreach (var user in FavSubs)
                {
                    queryParams = new DynamicParameters();
                    queryParams.Add("@UserId", substitutePreferenceModel.UserId);
                    queryParams.Add("@SubstituteId", user.UserId);
                    await Db.ExecuteAsync(sql, queryParams, commandType: CommandType.StoredProcedure);
                }

                foreach (var user in BlockedSubs)
                {
                    sql = "[users].[UpdateBlockedSubstitutes]";
                    queryParams = new DynamicParameters();
                    queryParams.Add("@UserId", substitutePreferenceModel.UserId);
                    queryParams.Add("@SubstituteId", user.UserId);
                    await Db.ExecuteAsync(sql, queryParams, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {

            }
            return 1;
        }

        public IEnumerable<User> GetFavoriteSubstitutes(string UserId)
        {
            var sql = "[users].[GetFavoriteSubstitutes]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", UserId);
            return Db.Query<User>(sql, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }

        public IEnumerable<User> GetBlockedSubstitutes(string UserId)
        {
            var sql = "[users].[GetBlockedSubstitutes]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", UserId);
            return Db.Query<User>(sql, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }

        public IEnumerable<User> GetAdminListByAbsenceId(int AbsenceId)
        {
            var sql = "[users].[GetAdminListByAbsenceId]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@AbsenceId", AbsenceId);
            return Db.Query<User>(sql, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }

        public IEnumerable<PreferredSchoolModel> GetSubstitutePreferredSchools(string UserId)
        {
            var sql = "[users].[GetSubstitutePreferredSchools]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@SubstituteId", UserId);
            return Db.Query<PreferredSchoolModel>(sql, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }

        public async Task<int> UpdateEnabledSchools(PreferredSchoolModel preferredSchoolModel)
        {
            try
            {
                var sql = "[users].[SaveEnabledSchoolsBySubstitute]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@OrganizationId", preferredSchoolModel.OrganizationId); 
                queryParams.Add("@SubstituteId", preferredSchoolModel.UserId);
                queryParams.Add("@IsEnabled", preferredSchoolModel.IsEnabled);
                await Db.ExecuteAsync(sql, queryParams, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
            }
            return 1;
        }

        public bool Delete(string sql, DynamicParameters param, CommandType commandType)
        {
            var DeletedRecord = Db.Execute(sql, param: param, commandType: commandType);
            return Convert.ToBoolean(DeletedRecord);
            //return Convert.ToBoolean(param.Get<int>("@HasSucceeded"));
        }

        #region Substitute
        public IEnumerable<User> GetAvailableSubstitutes(AbsenceModel absence)
        {
            var sql = "[users].[GetAvailableSubstitutes]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", absence.DistrictId);
            queryParams.Add("@StartDate", absence.StartDate);
            queryParams.Add("@EndDate", absence.EndDate);
            queryParams.Add("@StartTime", absence.StartTime);
            queryParams.Add("@EndTime", absence.EndTime);
            return Db.Query<User>(sql, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }
        #endregion

        #region Availability
        public IEnumerable<UserAvailability> GetAvailabilities(UserAvailability availability)
        {
            const string query = "[Users].[GetAvailability]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", availability.UserId);
            queryParams.Add("@StartDate", availability.StartDate);
            queryParams.Add("@EndDate", availability.EndDate);
            return Db.Query<UserAvailability>(query, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }

        public UserAvailability InsertAvailability(UserAvailability availability)
        {
            const string query = "[Users].[InsertAvailability]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", availability.UserId);
            queryParams.Add("@AvailabilityStatusId", availability.AvailabilityStatusId);
            queryParams.Add("@Title", availability.Title);
            queryParams.Add("@StartDate", availability.StartDate);
            queryParams.Add("@EndDate", availability.EndDate);
            queryParams.Add("@StartTime", availability.StartTime);
            queryParams.Add("@EndTime", availability.EndTime);
            queryParams.Add("@IsAllDayOut", availability.IsAllDayOut);
            queryParams.Add("@IsRepeat", availability.IsRepeat);
            queryParams.Add("@RepeatType", availability.RepeatType);
            queryParams.Add("@RepeatValue", availability.RepeatValue);
            queryParams.Add("@RepeatOnWeekDays", availability.RepeatOnWeekDays);
            queryParams.Add("@IsEndsNever", availability.IsEndsNever);
            queryParams.Add("@EndsOnAfterNumberOfOccurrance", availability.EndsOnAfterNumberOfOccurrance);
            queryParams.Add("@EndsOnUntilDate", availability.EndsOnUntilDate);
            queryParams.Add("@Notes", availability.Notes);
            queryParams.Add("@CreatedBy", availability.CreatedBy);
            return Db.Query<UserAvailability>(query, queryParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public UserAvailability UpdateAvailability(UserAvailability availability)
        {
            const string query = "[Users].[InsertAvailability]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", availability.UserId);
            queryParams.Add("@AvailabilityStatusId", availability.AvailabilityStatusId);
            queryParams.Add("@Title", availability.Title);
            queryParams.Add("@StartDate", availability.StartDate);
            queryParams.Add("@EndDate", availability.EndDate);
            queryParams.Add("@StartTime", availability.StartTime);
            queryParams.Add("@EndTime", availability.EndTime);
            queryParams.Add("@IsAllDayOut", availability.IsAllDayOut);
            queryParams.Add("@IsRepeat", availability.IsRepeat);
            queryParams.Add("@RepeatType", availability.RepeatType);
            queryParams.Add("@RepeatValue", availability.RepeatValue);
            queryParams.Add("@RepeatOnWeekDays", availability.RepeatOnWeekDays);
            queryParams.Add("@IsEndsNever", availability.IsEndsNever);
            queryParams.Add("@EndsOnAfterNumberOfOccurrance", availability.EndsOnAfterNumberOfOccurrance);
            queryParams.Add("@EndsOnUntilDate", availability.EndsOnUntilDate);
            queryParams.Add("@Notes", availability.Notes);
            queryParams.Add("@ModifiedBy", availability.ModifiedBy);
            return Db.Query<UserAvailability>(query, queryParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public UserAvailability DeleteAvailability(UserAvailability availability)
        {
            const string query = "[Users].[InsertAvailability]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@AvailabilityId", availability.AvailabilityId);
            queryParams.Add("@ArchivedBy", availability.ArchivedBy);
            return Db.Query<UserAvailability>(query, queryParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }
        #endregion
    }
}
