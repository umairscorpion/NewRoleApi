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
            var userDetail = Db.Query<User>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).SingleOrDefault();
            if (userDetail != null)
            {
                userDetail.SecondarySchools = GetUserSecondarySchools(userId);
                userDetail.Permissions = GetUserPermissions(userDetail.RoleId);
            }
            return userDetail;
        }

        private List<string> GetUserSecondarySchools(string userId)
        {
            var sql = "[Users].[sp_getUserSecondarySchools]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", userId);
            return Db.Query<string>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        private List<RolePermission> GetUserPermissions(int userRoleId)
        {
            var sql = "[Users].[GetRolePermissions]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@RoleId", userRoleId);
            return Db.Query<RolePermission>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
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

        public User UpdatePassword(User user)
        {
            var sql = "[Users].[sp_updatePassword]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", user.UserId);
            queryParams.Add("@Password", user.Password);
            return Db.ExecuteScalar<User>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
        }

        public User UpdatePasswordUsingActivationLink(User user)
        {
            var sql = "[Users].[sp_updatePasswordByActivationLink]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@ResetPassKey", user.ActivationCode);
            queryParams.Add("@Email", user.Email);
            queryParams.Add("@Password", user.Password);
            return Db.ExecuteScalar<User>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
        }

        public User InsertUser(User model)
        {
            var sql = "[Users].[InsertUser]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@FirstName", model.FirstName);
            queryParams.Add("@LastName", model.LastName);
            queryParams.Add("@UserTypeId", model.UserTypeId);
            queryParams.Add("@TeacherLevel", model.TeachingLevel);
            queryParams.Add("@SpecialityTypeId", model.SpecialityTypeId);
            queryParams.Add("@RoleId", model.RoleId);
            queryParams.Add("@Gender", model.Gender);
            queryParams.Add("@IsCertified", model.IsCertified);
            queryParams.Add("@DistrictId", model.DistrictId);
            queryParams.Add("@OrganizationId", model.OrganizationId);
            queryParams.Add("@Email", model.Email);
            queryParams.Add("@PhoneNumber", model.PhoneNumber);
            queryParams.Add("@IsSubscribedSMS", model.IsSubscribedSMS);
            queryParams.Add("@IsSubscribedEmail", model.IsSubscribedEmail);
            queryParams.Add("@Isdeleted", 0);
            queryParams.Add("@ProfilePicture", model.ProfilePicture);
            queryParams.Add("@PayRate", Convert.ToString(model.PayRate));
            queryParams.Add("@HourLimit", model.HourLimit);
            queryParams.Add("@Password", string.IsNullOrEmpty(model.Password)? model.PhoneNumber: model.Password);
            model.UserId = Db.ExecuteScalar<string>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);

            if (model.SecondarySchools != null)
            {
                sql = "[Users].[sp_insertSecondarySchools]";
                foreach (var schoolId in model.SecondarySchools)
                {
                    queryParams = new DynamicParameters();
                    queryParams.Add("@UserId", model.UserId);
                    queryParams.Add("@LocationId", schoolId);
                    queryParams.Add("@UserLevel", 3);
                    queryParams.Add("@IsPrimary", 0);
                    Db.ExecuteScalar<string>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
                }
            }           
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
            queryParams.Add("@SpecialityTypeId", model.SpecialityTypeId);
            queryParams.Add("@OrganizationId", model.OrganizationId);
            queryParams.Add("@RoleId", model.RoleId);
            queryParams.Add("@Gender", model.Gender);
            queryParams.Add("@IsCertified", model.IsCertified);
            queryParams.Add("@IsSubscribedEmail", model.IsSubscribedEmail);
            queryParams.Add("@IsSubscribedSMS", model.IsSubscribedSMS);
            queryParams.Add("@DistrictId", model.DistrictId);
            queryParams.Add("@Email", model.Email);
            queryParams.Add("@PhoneNumber", model.PhoneNumber);
            queryParams.Add("@Isdeleted", 0);
            queryParams.Add("@ProfilePicture", model.ProfilePicture);
            queryParams.Add("@IsActive", model.IsActive);
            queryParams.Add("@PayRate", Convert.ToString(model.PayRate));
            queryParams.Add("@HourLimit", model.HourLimit);
            Db.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);

            if(model.SecondarySchools != null)
            {
                sql = "[Users].[sp_insertSecondarySchools]";
                foreach (var schoolId in model.SecondarySchools)
                {
                    queryParams = new DynamicParameters();
                    queryParams.Add("@UserId", model.UserId);
                    queryParams.Add("@LocationId", schoolId);
                    queryParams.Add("@UserLevel", 3);
                    queryParams.Add("@IsPrimary", 0);
                    Db.ExecuteScalar<string>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
                }
            }            
            return model;
        }

        public User UpdateUserStatus(User model)
        {
            var sql = "[Users].[UpdateUserStatus]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", model.UserId);
            queryParams.Add("@IsActive", model.IsActive);
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

        public IEnumerable<User> SearchContent(string userId, int districtId, string organizationId,string searchQuery)
        {
            var sql = "[Users].[SearchContent]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@userId", userId);
            queryParams.Add("@districtId", districtId);
            queryParams.Add("@organizationId", organizationId);
            queryParams.Add("@searchQuery", searchQuery);
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

        public int CheckEmailExistance(string emailId)
        {
            var sql = "[users].[sp_checkEmailExistance]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@EmailId", emailId);
            return Db.ExecuteScalar<int>(sql, queryParams, commandType: CommandType.StoredProcedure);
        }

        public int UpdatePasswordResetKey(User user)
        {
            var sql = "[users].[sp_updatePasswordResetKey]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@EmailId", user.Email);
            queryParams.Add("@ResetPassKey", user.ActivationCode);
            queryParams.Add("@validUpTo", System.DateTime.Now.AddDays(2));
            return Db.ExecuteScalar<int>(sql, queryParams, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<SubstituteCategoryModel> GetSubstituteCategories(string SubstituteId)
        {
            var sql = "[users].[GetSubstituteCategories]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@SubstituteId", SubstituteId);
            return Db.Query<SubstituteCategoryModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public IEnumerable<SubstituteCategoryModel> GetSubstituteNotificationEvents(string SubstituteId)
        {
            try
            {
                var sql = "[users].[GetSubstituteNotificationEvents]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@SubstituteId", SubstituteId);
                return Db.Query<SubstituteCategoryModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
            catch (Exception ex)
            {
            }
            finally {
            }
            return null;
            
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

        public PositionDetail InsertPositions(PositionDetail position)
        {
            var query = position.Id > 0 ? "[Users].[sp_updatePosition]" : "[Users].[sp_insertPosition]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@Id", position.Id);
            queryParams.Add("@Title", position.Title);
            queryParams.Add("@IsVisible", position.IsVisible); 
            queryParams.Add("@DistrictId", position.DistrictId);
            queryParams.Add("@CreatedDate", DateTime.Now);
            return Db.Query<PositionDetail>(query, queryParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public IEnumerable<PositionDetail> GetPositions(int districtId)
        {
            var sql = "[users].[sp_getPositions]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", districtId);
            return Db.Query<PositionDetail>(sql, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }

        public bool DeletePosition(int id)
        {
            int hasSucceeded = 0;
            var sql = "[users].[sp_deletePosition]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@Id", id);
            queryParams.Add("@HasSucceeded", hasSucceeded, null, ParameterDirection.Output);
            var result = Delete(sql, queryParams, CommandType.StoredProcedure);
            return result;
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

        public PayRateSettings InsertPayRate(PayRateSettings payRateSettings)
        {
            var query = payRateSettings.Id > 0 ? "[Users].[sp_updatePayRate]" : "[Users].[sp_insertPayRate]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@Id", payRateSettings.Id);
            queryParams.Add("@PositionId", payRateSettings.PositionId);
            queryParams.Add("@PayRate", payRateSettings.PayRate);
            queryParams.Add("@DistrictId", payRateSettings.DistrictId);
            queryParams.Add("@Period", payRateSettings.Period);
            queryParams.Add("@CreatedDate", DateTime.Now);
            return Db.Query<PayRateSettings>(query, queryParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public IEnumerable<PayRateSettings> GetPayRates(int districtId)
        {
            var query = "[Users].[sp_getPayRates]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", districtId);
            return Db.Query<PayRateSettings>(query, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }

        public PayRateRule InsertPayRateRule(PayRateRule payRateRule)
        {
            try
            {
                var query = payRateRule.Id > 0 ? "[Users].[sp_updatePayRateRule]" : "[Users].[sp_insertPayRateRule]";
                var queryParams = new DynamicParameters();
                queryParams.Add("@Id", payRateRule.Id);
                queryParams.Add("@PositionId", payRateRule.PositionId);
                queryParams.Add("@PayRate", payRateRule.PayRate);
                queryParams.Add("@DistrictId", payRateRule.DistrictId);
                queryParams.Add("@IncreaseAfterDays", payRateRule.IncreaseAfterDays);
                queryParams.Add("@CreatedDate", DateTime.Now);
                return Db.Query<PayRateRule>(query, queryParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            catch (Exception ex)
            {

            }
            return null;
            
        }

        public IEnumerable<PayRateRule> GetPayRateRules(int districtId)
        {
            var query = "[Users].[sp_getPayRateRules]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", districtId);
            return Db.Query<PayRateRule>(query, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }

        public PayRateSettings DeletePayRate(PayRateSettings payRateSettings)
        {
            var query = "[Users].[sp_deletePayRate]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@Id", payRateSettings.Id);
            queryParams.Add("@IsArchived", 1);
            queryParams.Add("@ArchivedBy", payRateSettings.ArchivedBy);
            return Db.Query<PayRateSettings>(query, queryParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public PayRateRule DeletePayRateRule(PayRateRule payRateSettings)
        {
            var query = "[Users].[sp_deletePayRateRule]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@Id", payRateSettings.Id);
            queryParams.Add("@IsArchived", 1);
            queryParams.Add("@ArchivedBy", payRateSettings.ArchivedBy);
            return Db.Query<PayRateRule>(query, queryParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public IEnumerable<SchoolSubList> GetSchoolSubList(string userId, int districtId)
        {
            var query = "[Users].[sp_getSchoolSubList]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", userId);
            queryParams.Add("@DistrictId", districtId);
            return Db.Query<SchoolSubList>(query, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }

        public async Task<int> UpdateSchoolSubList(SchoolSubList schoolSubList)
        {
            try
            {
                DeleteSubstituteList(schoolSubList.DistrictId);
                var sql = "[users].[sp_insertSchoolSubList]";
                var queryParams = new DynamicParameters();
                var schoolSubs = JsonConvert.DeserializeObject<List<SchoolSubList>>(schoolSubList.SubstituteId);
                foreach (var subs in schoolSubs)
                {
                    queryParams = new DynamicParameters();
                    queryParams.Add("@Id", schoolSubList.Id);
                    queryParams.Add("@DistrictId", schoolSubList.DistrictId);
                    queryParams.Add("@AddedByUserId", schoolSubList.ModifyByUserId);
                    queryParams.Add("@SubstituteId", subs.SubstituteId);
                    queryParams.Add("@ModifyByUserId", schoolSubList.ModifyByUserId);
                    queryParams.Add("@CreatedDate", DateTime.Now);
                    queryParams.Add("@ModifiedDate", DateTime.Now);
                    queryParams.Add("@IsAdded", 1);
                    await Db.ExecuteAsync(sql, queryParams, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {

            }
            return 1;
        }

        public IEnumerable<SchoolSubList> GetBlockedSchoolSubList(string userId, int districtId)
        {
            var query = "[Users].[sp_getBlockedSchoolSubList]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", userId);
            queryParams.Add("@DistrictId", districtId);
            return Db.Query<SchoolSubList>(query, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }

        public async Task<int> UpdateBlockedSchoolSubList(SchoolSubList schoolSubList)
        {
            try
            {
                DeleteBlockedSubstituteList(schoolSubList.DistrictId);
                var sql = "[users].[sp_insertBlockedSchoolSubList]";
                var queryParams = new DynamicParameters();
                var schoolSubs = JsonConvert.DeserializeObject<List<SchoolSubList>>(schoolSubList.SubstituteId);
                foreach (var subs in schoolSubs)
                {
                    queryParams = new DynamicParameters();
                    queryParams.Add("@Id", schoolSubList.Id);
                    queryParams.Add("@DistrictId", schoolSubList.DistrictId);
                    queryParams.Add("@AddedByUserId", schoolSubList.ModifyByUserId);
                    queryParams.Add("@SubstituteId", subs.SubstituteId);
                    queryParams.Add("@ModifyByUserId", schoolSubList.ModifyByUserId);
                    queryParams.Add("@CreatedDate", DateTime.Now);
                    queryParams.Add("@ModifiedDate", DateTime.Now);
                    queryParams.Add("@IsAdded", 1);
                    await Db.ExecuteAsync(sql, queryParams, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {

            }
            return 1;
        }

        public bool DeleteSubstituteList(int districtId)
        {
            int hasSucceeded = 0;
            var sql = "[users].[sp_deleteSubstituteList]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", districtId);
            queryParams.Add("@HasSucceeded", hasSucceeded, null, ParameterDirection.Output);
            var result = Delete(sql, queryParams, CommandType.StoredProcedure);
            return result;
        }

        public bool DeleteBlockedSubstituteList(int districtId)
        {
            int hasSucceeded = 0;
            var sql = "[users].[sp_deleteSchoolSubstituteList]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", districtId);
            queryParams.Add("@HasSucceeded", hasSucceeded, null, ParameterDirection.Output);
            var result = Delete(sql, queryParams, CommandType.StoredProcedure);
            return result;
        }

        public IEnumerable<FileManager> AddFiles(FileManager fileManager)
        {
            var query = "[Users].[InsertFile]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@FileName", fileManager.FileName);
            queryParams.Add("@OriginalFileName", fileManager.OriginalFileName);
            queryParams.Add("@FileExtentione", fileManager.FileExtention);
            queryParams.Add("@FileContentType", fileManager.FileContentType);
            queryParams.Add("@UserId", fileManager.UserId);
            queryParams.Add("@DistrictId", fileManager.DistrictId);
            queryParams.Add("@OrganizationId", fileManager.OrganizationId);
            queryParams.Add("@FileType", fileManager.FileType);
            return Db.Query<FileManager>(query, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }

        public IEnumerable<FileManager> GetFiles(FileManager fileManager)
        {
            var query = "[Users].[GetFiles]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", fileManager.UserId);
            queryParams.Add("@DistrictId", fileManager.DistrictId);
            queryParams.Add("@FileType", fileManager.FileType);
            return Db.Query<FileManager>(query, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }

        public IEnumerable<FileManager> DeleteFiles(FileManager fileManager)
        {
            var query = "[Users].[DeleteFiles]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", fileManager.UserId);
            queryParams.Add("@FileName", fileManager.FileName);
            queryParams.Add("@DistrictId", fileManager.DistrictId);
            queryParams.Add("@FileType", fileManager.FileType);
            return Db.Query<FileManager>(query, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }

        #endregion

        #region Availability
        public IEnumerable<SubstituteAvailabilitySummary> GetSubstituteAvailabilitySummary(SubstituteAvailability model)
        {
            const string query = "[Users].[GetSubstituteAvailabilitiesSummary]";
            return Db.Query<SubstituteAvailabilitySummary>(query, null, commandType: CommandType.StoredProcedure).ToList();
        }

        public List<UserSummary> GetUsersSummaryList(int districtId)
        {
            const string query = "[Users].[GetUsersSummary]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", districtId);
            return Db.Query<UserSummary>(query, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }

        public bool VerifyUser(User model)
        {
            const string query = "[Users].[VerifyUser]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", model.UserId);
            queryParams.Add("@Email", model.Email);
            return Db.Query<bool>(query, queryParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public IEnumerable<SubstituteAvailability> GetSubstituteAvailability(SubstituteAvailability model)
        {
            const string query = "[Users].[GetSubstituteAvailabilities]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@StartDate", model.StartDate);
            queryParams.Add("@AvailabilityStatusId", model.AvailabilityStatusId);
            queryParams.Add("@UserId", model.UserId);
            return Db.Query<SubstituteAvailability>(query, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }
        public IEnumerable<UserAvailability> GetAvailabilities(UserAvailability availability)
        {
            const string query = "[Users].[GetAvailability]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", availability.UserId);
            queryParams.Add("@StartDate", availability.StartDate);
            queryParams.Add("@EndDate", availability.EndDate);
            return Db.Query<UserAvailability>(query, queryParams, commandType: CommandType.StoredProcedure).ToList();
        }
        public UserAvailability GetAvailabilityById(int id)
        {
            const string query = "[Users].[GetAvailability]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@AvailabilityId", id);
            return Db.Query<UserAvailability>(query, queryParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
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
            const string query = "[Users].[UpdateAvailability]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", availability.AvailabilityId);
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
            const string query = "[Users].[DeleteAvailability]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@AvailabilityId", availability.AvailabilityId);
            queryParams.Add("@ArchivedBy", availability.ArchivedBy);
            return Db.Query<UserAvailability>(query, queryParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }
        #endregion
    }
}
