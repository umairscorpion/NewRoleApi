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
        IEnumerable<LookupModel> GetUserTypes();
        LocationTime GetUserLocationTime(string userId, int userLevel);

        User GetUserDetail(string userId);
        User UpdateUser(User user);
        // functions related to Employee
        User InsertUser(User model);
        User UpdateEmployee(User model);
        IEnumerable<User> GetUsers(string userId, int userRole, int districtId, string organizationId);
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
        bool DeletePosition(int id);

        #region Substitute
        IEnumerable<User> GetAvailableSubstitutes(AbsenceModel absence);
        #endregion
    }
}
