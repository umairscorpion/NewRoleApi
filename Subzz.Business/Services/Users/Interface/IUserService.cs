using SubzzV2.Core.Entities;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subzz.Business.Services.Users.Interface
{
    public interface IUserService
    {
        User GetUserDetail(string userId);
        IEnumerable<UserResource> GetUserResources(string userId , int resourceTypeId, int parentResourceTypeId, int IsAdminPortal);
        int InsertExternalUser(ExternalUser externalUser);
        IEnumerable<LookupModel> GetUserTypes();
        LocationTime GetUserLocationTime(string userId, int userLevel);
        User UpdateUser(User user);

        // functions related to Employee
        #region Employee
        User InsertUser(User model);
        User UpdateEmployee(User model);
        IEnumerable<User> GetUsers(string userId, int userRole, int districtId, string organizationId);
        IEnumerable<User> GetEmployeeSuggestions(string searchText, int isSearchSubstitute, string organizationId, int districtId);
        bool DeleteUser(string UserId);
        IEnumerable<User> GetEmployee(int id);
        #endregion

        #region User Settings
        IEnumerable<SubstituteCategoryModel> GetSubstituteCategories(string SubstituteId);
        int UpdateUserCategories(SubstituteCategoryModel substituteCategoryModel);
        AbsenceModel GetUsersForSendingAbsenceNotificationOnEntireSub(int DistrictId, string OrganizationId, int AbsenceId, string SubstituteId);
        Task<int> UpdateSubstitutePeferrence(SubstitutePreferenceModel substitutePreferenceModel);
        IEnumerable<User> GetFavoriteSubstitutes(string UserId);
        IEnumerable<User> GetBlockedSubstitutes(string UserId);
        IEnumerable<User> GetAdminListByAbsenceId(int AbsenceId);
        IEnumerable<PreferredSchoolModel> GetSubstitutePreferredSchools(string UserId);
        Task<int> UpdateEnabledSchools(PreferredSchoolModel preferredSchoolModel);
        PositionDetail InsertPositions(PositionDetail position);
        IEnumerable<PositionDetail> GetPositions(int districtId);
        bool DeletePosition(int id);
        #endregion

        #region Substitute
        IEnumerable<User> GetAvailableSubstitutes(AbsenceModel absence);
        PayRateSettings InsertPayRate(PayRateSettings payRateSettings);
        IEnumerable<PayRateSettings> GetPayRates(int districtId);
        PayRateRule InsertPayRateRule(PayRateRule payRateRule);
        IEnumerable<PayRateRule> GetPayRateRules(int districtId);
        PayRateSettings DeletePayRate(PayRateSettings payRateSettings);
        PayRateRule DeletePayRateRule(PayRateRule payRateRule);
        IEnumerable<SchoolSubList> GetSchoolSubList(string userId, int districtId);
        Task<int> UpdateSchoolSubList(SchoolSubList schoolSubList);
        IEnumerable<SchoolSubList> GetBlockedSchoolSubList(string userId, int districtId);
        Task<int> UpdateBlockedSchoolSubList(SchoolSubList schoolSubList);
        #endregion

        #region Availability
        IEnumerable<SubstituteAvailability> GetSubstituteAvailability(SubstituteAvailability model);
        IEnumerable<UserAvailability> GetAvailabilities(UserAvailability availability);
        UserAvailability GetAvailabilityById(int id);
        UserAvailability InsertAvailability(UserAvailability availability);
        UserAvailability UpdateAvailability(UserAvailability availability);
        UserAvailability DeleteAvailability(UserAvailability availability);
        IEnumerable<SubstituteAvailabilitySummary> GetSubstituteAvailabilitySummary(SubstituteAvailability model);

        #endregion
    }
}
