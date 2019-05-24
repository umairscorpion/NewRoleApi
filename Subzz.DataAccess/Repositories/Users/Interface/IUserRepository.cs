﻿using SubzzV2.Core.Entities;
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
        // functions related to Employee
        User InsertUser(User model);
        User UpdateEmployee(User model);
        User UpdatePassword(User user);
        IEnumerable<User> GetUsers(string userId, int userRole, int districtId, string organizationId);
        IEnumerable<User> SearchContent(string userId, int districtId, string organizationId, string searchQuery);
        IEnumerable<User> GetEmployeeSuggestions(string searchText, int isSearchSubstitute, string organizationId, int districtId);
        bool DeleteUser(string UserId);
        IEnumerable<User> GetEmployee(int id);

        IEnumerable<SubstituteCategoryModel> GetSubstituteCategories(string SubstituteId);
        int UpdateUserCategories(SubstituteCategoryModel substituteCategoryModel);
        AbsenceModel GetUsersForSendingAbsenceNotificationOnEntireSub(int DistrictId, string OrganizationId, int AbsenceId, string SubstituteId);

        Task<int> UpdateSubstitutePeferrence(SubstitutePreferenceModel substitutePreferenceModel);

        IEnumerable<User> GetFavoriteSubstitutes(string UserId);
        IEnumerable<User> GetBlockedSubstitutes(string UserId);
        IEnumerable<User> GetAdminListByAbsenceId(int AbsenceId);
        IEnumerable<PreferredSchoolModel> GetSubstitutePreferredSchools(string UserId);
        Task<int> UpdateEnabledSchools(PreferredSchoolModel preferredSchoolModel);
        IEnumerable<SubstituteAvailability> GetSubstituteAvailability(SubstituteAvailability model);
        IEnumerable<UserAvailability> GetAvailabilities(UserAvailability availability);
        UserAvailability InsertAvailability(UserAvailability availability);
        UserAvailability UpdateAvailability(UserAvailability availability);
        UserAvailability DeleteAvailability(UserAvailability availability);
        UserAvailability GetAvailabilityById(int id);

        PositionDetail InsertPositions(PositionDetail position);
        IEnumerable<PositionDetail> GetPositions(int districtId);
        PayRateSettings DeletePayRate(PayRateSettings payRateSettings);
        PayRateRule DeletePayRateRule(PayRateRule payRateRule);
        bool DeletePosition(int id);

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
    }
}
