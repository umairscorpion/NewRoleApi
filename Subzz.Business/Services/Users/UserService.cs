using Subzz.Business.Services.Users.Interface;
using SubzzV2.Core.Entities;
using SubzzV2.Core.Models;
using Subzz.DataAccess.Repositories.Users.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Subzz.DataAccess.Repositories.Users;

namespace Subzz.Business.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        public UserService()
        {
            _repo = new UserRepository();
        }
        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }
        public User GetUserDetail(string userId)
        {
            return _repo.GetUserDetail(userId);
        }
        public IEnumerable<UserResource> GetUserResources(string userId, int resourceTypeId, int parentResourceTypeId, int IsAdminPortal)
        {
            return _repo.GetUserResourses(userId, resourceTypeId, parentResourceTypeId, IsAdminPortal);
        }

        public User UpdateUser(User user)
        {
            return _repo.UpdateUser(user);
        }

        public UserReference UpdateUserProfile(UserReference user)
        {
            return _repo.UpdateUserProfile(user);
        }

        public User UpdateUserStatus(User user)
        {
            return _repo.UpdateUserStatus(user);
        }

        // functions related to Employee

        public User UpdatePassword(User user)
        {
            return _repo.UpdatePassword(user);
        }

        public User UpdatePasswordUsingActivationLink(User user)
        {
            return _repo.UpdatePasswordUsingActivationLink(user);
        }

        public User InsertUser(User model)
        {
            return _repo.InsertUser(model);
        }

        public User UpdateEmployee(User model)
        {
            return _repo.UpdateEmployee(model);
        }

        public IEnumerable<User> GetUsers(string userId, int userRole, int districtId, string organizationId)
        {
            return _repo.GetUsers(userId, userRole, districtId, organizationId);
        }
        public IEnumerable<User> SearchContent(string userId, int districtId, string organizationId, string searchQuery)
        {
            return _repo.SearchContent(userId, districtId, organizationId, searchQuery);
        }

        public IEnumerable<User> GetEmployeeSuggestions(string searchText, int isSearchSubstitute, string organizationId, int districtId)
        {
            return _repo.GetEmployeeSuggestions(searchText, isSearchSubstitute, organizationId, districtId);
        }

        public bool DeleteUser(string UserId)
        {
            return _repo.DeleteUser(UserId);
        }

        public IEnumerable<User> GetEmployee(int id)
        {
            return _repo.GetEmployee(id);
        }

        public IEnumerable<LookupModel> GetUserTypes()
        {
            return _repo.GetUserTypes();
        }

        public int InsertExternalUser(ExternalUser externalUser)
        {
            return _repo.InsertExternalUser(externalUser);
        }

        public int CheckEmailExistance(string emailId)
        {
            return _repo.CheckEmailExistance(emailId);
        }

        public int UpdatePasswordResetKey(User user)
        {
            return _repo.UpdatePasswordResetKey(user);
        }

        public IEnumerable<SubstituteCategoryModel> GetSubstituteCategories(string SubstituteId)
        {
            return _repo.GetSubstituteCategories(SubstituteId);
        }

        public IEnumerable<SubstituteCategoryModel> GetSubstituteNotificationEvents(string SubstituteId)
        {
            return _repo.GetSubstituteNotificationEvents(SubstituteId);
        }

        public int UpdateUserCategories(SubstituteCategoryModel substituteCategoryModel)
        {
            return _repo.UpdateUserCategories(substituteCategoryModel);
        }

        public int UpdateNotificationEvents(SubstituteCategoryModel substituteEventModel)
        {
            return _repo.UpdateNotificationEvents(substituteEventModel);
        }

        public LocationTime GetUserLocationTime(string userId, int userLevel)
        {
            return _repo.GetUserLocationTime(userId, userLevel);
        }

        public AbsenceModel GetUsersForSendingAbsenceNotificationOnEntireSub(int DistrictId, string OrganizationId, int AbsenceId, string SubstituteId)
        {
            return _repo.GetUsersForSendingAbsenceNotificationOnEntireSub(DistrictId, OrganizationId, AbsenceId, SubstituteId);
        }

        public async Task<int> UpdateSubstitutePeferrence(SubstitutePreferenceModel substitutePreferenceModel)
        {
            return await _repo.UpdateSubstitutePeferrence(substitutePreferenceModel);
        }

        public IEnumerable<User> GetFavoriteSubstitutes(string UserId)
        {
            return _repo.GetFavoriteSubstitutes(UserId);
        }

        public IEnumerable<User> GetBlockedSubstitutes(string UserId)
        {
            return _repo.GetBlockedSubstitutes(UserId);
        }

        public IEnumerable<User> GetAdminListByAbsenceId(int AbsenceId)
        {
            return _repo.GetAdminListByAbsenceId(AbsenceId);
        }

        public IEnumerable<PreferredSchoolModel> GetSubstitutePreferredSchools(string UserId)
        {
            return _repo.GetSubstitutePreferredSchools(UserId);
        }

        public async Task<int> UpdateEnabledSchools(PreferredSchoolModel preferredSchoolModel)
        {
            return await _repo.UpdateEnabledSchools(preferredSchoolModel);
        }

        public IEnumerable<SubstituteAvailabilitySummary> GetSubstituteAvailabilitySummary(SubstituteAvailability model)
        {
            return _repo.GetSubstituteAvailabilitySummary(model);
        }

        public List<UserSummary> GetUsersSummaryList(int districtId)
        {
            return _repo.GetUsersSummaryList(districtId);
        }

        public bool VerifyUser(User model)
        {
            return _repo.VerifyUser(model);
        }

        public IEnumerable<SubstituteAvailability> GetSubstituteAvailability(SubstituteAvailability model)
        {
            return _repo.GetSubstituteAvailability(model);
        }

        public IEnumerable<UserAvailability> GetAvailabilities(UserAvailability availability)
        {
            return _repo.GetAvailabilities(availability);
        }

        public UserAvailability GetAvailabilityById(int id)
        {
            return _repo.GetAvailabilityById(id);
        }

        public UserAvailability InsertAvailability(UserAvailability availability)
        {
            return _repo.InsertAvailability(availability);
        }

        public UserAvailability UpdateAvailability(UserAvailability availability)
        {
            return _repo.UpdateAvailability(availability);
        }

        public UserAvailability DeleteAvailability(UserAvailability availability)
        {
            return _repo.DeleteAvailability(availability);
        }

        public IEnumerable<User> GetAvailableSubstitutes(AbsenceModel absence)
        {
            return _repo.GetAvailableSubstitutes(absence);
        }

        public PositionDetail InsertPositions(PositionDetail position)
        {
            return _repo.InsertPositions(position);
        }
        public IEnumerable<PositionDetail> GetPositions(int districtId)
        {
            return _repo.GetPositions(districtId);
        }

        public bool DeletePosition(int id)
        {
            return _repo.DeletePosition(id);
        }

        #region Substitute
        public PayRateSettings InsertPayRate(PayRateSettings payRateSettings)
        {
            return _repo.InsertPayRate(payRateSettings);
        }

        public IEnumerable<PayRateSettings> GetPayRates(int districtId)
        {
            return _repo.GetPayRates(districtId);
        }

        public PayRateRule InsertPayRateRule(PayRateRule payRateRule)
        {
            return _repo.InsertPayRateRule(payRateRule);
        }

        public IEnumerable<PayRateRule> GetPayRateRules(int districtId)
        {
            return _repo.GetPayRateRules(districtId);
        }

        public PayRateSettings DeletePayRate(PayRateSettings payRateSettings)
        {
            return _repo.DeletePayRate(payRateSettings);
        }

        public PayRateRule DeletePayRateRule(PayRateRule payRateRule)
        {
            return _repo.DeletePayRateRule(payRateRule);
        }

        public IEnumerable<SchoolSubList> GetSchoolSubList(string userId, int districtId)
        {
            return _repo.GetSchoolSubList(userId, districtId);
        }

        public async Task<int> UpdateSchoolSubList(SchoolSubList schoolSubList)
        {
            return await _repo.UpdateSchoolSubList(schoolSubList);
        }

        public IEnumerable<SchoolSubList> GetBlockedSchoolSubList(string userId, int districtId)
        {
            return _repo.GetBlockedSchoolSubList(userId, districtId);
        }

        public async Task<int> UpdateBlockedSchoolSubList(SchoolSubList schoolSubList)
        {
            return await _repo.UpdateBlockedSchoolSubList(schoolSubList);
        }

        public IEnumerable<FileManager> AddFiles(FileManager fileManager)
        {
            return _repo.AddFiles(fileManager);
        }

        public IEnumerable<FileManager> GetFiles(FileManager fileManager)
        {
            return _repo.GetFiles(fileManager);
        }

        public IEnumerable<FileManager> DeleteFiles(FileManager fileManager)
        {
            return _repo.DeleteFiles(fileManager);
        }

        #endregion
    }
}
