﻿using SubzzV2.Core.Entities;
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
        int CheckEmailExistance(string emailId);
        int UpdatePasswordResetKey(User user);
        IEnumerable<LookupModel> GetUserTypes();
        LocationTime GetUserLocationTime(string userId, int userLevel);
        User UpdateUser(User user);
        UserReference UpdateUserProfile(UserReference user);

        User UpdatePassword(User user);
        User UpdatePasswordUsingActivationLink(User user);
        User UpdateUserStatus(User user);

        // functions related to Employee
        #region Employee
        User InsertUser(User model);
        User InsertTemporarySubstitutes(User model, string status);
        User InsertTemporaryStaff(User model, string status);
        User UpdateEmployee(User model);
        IEnumerable<User> GetUsers(string userId, int userRole, int districtId, string organizationId);
        IEnumerable<User> GetTemporarySubstitutes();
        int DeleteTemporarySubstitutes(int DistrictId);
        IEnumerable<User> GetTemporaryStaff();
        int DeleteTemporaryStaff(int DistrictId);
        IEnumerable<User> GetAllUserRoles();
        IEnumerable<OrganizationModel> GetSchools();
        IEnumerable<DistrictModel> GetDistricts();
        IEnumerable<LookupModel> GetTeachingLevels();
        IEnumerable<LookupModel> GetTeachingSubjects();
        IEnumerable<User> SearchContent(string userId, int districtId, string organizationId, string searchQuery);
        IEnumerable<User> GetEmployeeSuggestions(string searchText, int isSearchSubstitute, string organizationId, int districtId);
        bool DeleteUser(string UserId);
        IEnumerable<User> GetEmployee(int id);
        #endregion

        #region User Settings
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
        IEnumerable<User> GetUsersByDistrictId(int DistrictId, int userRole);
        IEnumerable<PreferredSchoolModel> GetSubstitutePreferredSchools(string UserId);
        Task<int> UpdateEnabledSchools(PreferredSchoolModel preferredSchoolModel);
        PositionDetail InsertPositions(PositionDetail position);
        IEnumerable<PositionDetail> GetPositions(int districtId);
        int DeletePosition(int id);
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
        IEnumerable<FileManager> AddFiles(FileManager fileManager);
        IEnumerable<FileManager> GetFiles(FileManager fileManager);
        IEnumerable<FileManager> DeleteFiles(FileManager fileManager);
        #endregion

        #region Availability
        IEnumerable<SubstituteAvailability> GetSubstituteAvailability(SubstituteAvailability model);
        IEnumerable<UserAvailability> GetAvailabilities(UserAvailability availability);
        UserAvailability GetAvailabilityById(int id);
        string InsertAvailability(UserAvailability availability);
        string UpdateAvailability(UserAvailability availability);
        UserAvailability DeleteAvailability(UserAvailability availability);
        int CheckSubAvailability(UserAvailability availability);
        IEnumerable<SubstituteAvailabilitySummary> GetSubstituteAvailabilitySummary(SubstituteAvailability model);
        bool GetUserAvailability(string UserId, int AbsenceId);
        #endregion

        List<UserSummary> GetUsersSummaryList(int districtId);
        bool VerifyUser(User model);

        #region Events
        Event InsertEvent(Event model);
        #endregion

        string GetUserIdByPhoneNumber(string phoneNumber);
        int UpdateSubscription(User user);

        #region SubstituteList
        SubstituteCategory InsertSubstituteCategory(SubstituteCategory substituteCategory);
        SubstituteCategory UpdateSubstituteCategory(SubstituteCategory substituteCategory);
        List<SubstituteCategory> GetSubstituteCategoryList(int districtId);
        List<SubstituteList> GetSubstituteByCategoryId(int CategoryId, int districtId);
        int DeleteSubstituteCategory(int CategoryId);
        SubstituteCategory UpdateSubstituteCategoryById(SubstituteCategory substituteCategory);
        #endregion

    }
}
