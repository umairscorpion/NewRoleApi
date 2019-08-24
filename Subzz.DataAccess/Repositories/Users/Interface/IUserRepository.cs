using SubzzV2.Core.Entities;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subzz.DataAccess.Repositories.Users.Interface
{
    public interface IUserRepository
    {
        UserLogin GetUserByCredentials(string userName, string password);
        IEnumerable<UserResource> GetUserResourses(string userId, int resourceTypeId, int parentResourceTypeId, int IsAdminPortal);
        int InsertExternalUser(ExternalUser externalUser);
        int CheckEmailExistance(string emailId);
        int UpdatePasswordResetKey(User user);
        IEnumerable<LookupModel> GetUserTypes();
        LocationTime GetUserLocationTime(string userId, int userLevel);

        User GetUserDetail(string userId);
        User UpdateUser(User user);
        UserReference UpdateUserProfile(UserReference user);
        User UpdateUserStatus(User user);
        // functions related to Employee
        User InsertUser(User model);
        User UpdateEmployee(User model);
        User UpdatePassword(User user);
        User UpdatePasswordUsingActivationLink(User user);
        IEnumerable<User> GetUsers(string userId, int userRole, int districtId, string organizationId);
        IEnumerable<User> SearchContent(string userId, int districtId, string organizationId, string searchQuery);
        IEnumerable<User> GetEmployeeSuggestions(string searchText, int isSearchSubstitute, string organizationId, int districtId);
        bool DeleteUser(string UserId);
        IEnumerable<User> GetEmployee(int id);

        IEnumerable<SubstituteCategoryModel> GetSubstituteCategories(string SubstituteId);
        IEnumerable<SubstituteCategoryModel> GetSubstituteNotificationEvents(string SubstituteId);
        IEnumerable<SubstituteCategoryModel> GetGradeLevelsForNotification(string SubstituteId);
        IEnumerable<SubstituteCategoryModel> GetSubjectsForNotifications(string SubstituteId);
        int UpdateUserCategories(SubstituteCategoryModel substituteCategoryModel);
        int UpdateNotificationEvents(SubstituteCategoryModel substituteEventModel);
        int UpdateGradeLevelNotification(SubstituteCategoryModel substituteEventModel);
        int UpdateSubjectNotification(SubstituteCategoryModel substituteEventModel);
        int UpdateUserNotifications(SubstituteCategoryModel substituteEventModel);
        AbsenceModel GetUsersForSendingAbsenceNotificationOnEntireSub(int DistrictId, string OrganizationId, int AbsenceId, string SubstituteId);
        Task<int> UpdateSubstitutePeferrence(SubstitutePreferenceModel substitutePreferenceModel);

        IEnumerable<User> GetFavoriteSubstitutes(string UserId);
        IEnumerable<User> GetBlockedSubstitutes(string UserId);
        IEnumerable<User> GetAdminListByAbsenceId(int AbsenceId);
        IEnumerable<User> GetUsersByDistrictId(int districtId, int userRole);
        IEnumerable<PreferredSchoolModel> GetSubstitutePreferredSchools(string UserId);
        Task<int> UpdateEnabledSchools(PreferredSchoolModel preferredSchoolModel);
        IEnumerable<SubstituteAvailability> GetSubstituteAvailability(SubstituteAvailability model);
        IEnumerable<UserAvailability> GetAvailabilities(UserAvailability availability);
        string InsertAvailability(UserAvailability availability);
        string UpdateAvailability(UserAvailability availability);
        UserAvailability DeleteAvailability(UserAvailability availability);
        int CheckSubAvailability(UserAvailability availability);
        UserAvailability GetAvailabilityById(int id);
        bool GetUserAvailability(string UserId, int AbsenceId);

        PositionDetail InsertPositions(PositionDetail position);
        IEnumerable<PositionDetail> GetPositions(int districtId);
        PayRateSettings DeletePayRate(PayRateSettings payRateSettings);
        PayRateRule DeletePayRateRule(PayRateRule payRateRule);
        int DeletePosition(int id);
        int UpdateSubscription(User user);

        #region Substitute
        IEnumerable<User> GetAvailableSubstitutes(AbsenceModel absence);
        PayRateSettings InsertPayRate(PayRateSettings payRateSettings);
        IEnumerable<PayRateSettings> GetPayRates(int districtId);
        PayRateRule InsertPayRateRule(PayRateRule payRateRule);
        IEnumerable<PayRateRule> GetPayRateRules(int districtId);
        IEnumerable<SubstituteAvailabilitySummary> GetSubstituteAvailabilitySummary(SubstituteAvailability model);
        IEnumerable<SchoolSubList> GetSchoolSubList(string userId, int districtId);
        Task<int> UpdateSchoolSubList(SchoolSubList schoolSubList);
        IEnumerable<SchoolSubList> GetBlockedSchoolSubList(string userId, int districtId);
        Task<int> UpdateBlockedSchoolSubList(SchoolSubList schoolSubList);
        IEnumerable<FileManager> AddFiles(FileManager fileManager);
        IEnumerable<FileManager> GetFiles(FileManager fileManager);
        IEnumerable<FileManager> DeleteFiles(FileManager fileManager);
        #endregion

        List<UserSummary> GetUsersSummaryList(int districtId);
        bool VerifyUser(User model);
        Event InsertEvent(Event model);
        string GetUserIdByPhoneNumber(string phoneNumber);

        #region SubstituteList
        SubstituteCategory InsertSubstituteCategory(SubstituteCategory substituteCategory);
        SubstituteCategory UpdateSubstituteCategory(SubstituteCategory substituteCategory);
        List<SubstituteCategory> GetSubstituteCategoryList(int districtId);
        List<SubstituteList> GetSubstituteByCategoryId(int CategoryId);
        int DeleteSubstituteCategory(int CategoryId);
        SubstituteCategory UpdateSubstituteCategoryById(SubstituteCategory substituteCategory);
        #endregion
    }
}
