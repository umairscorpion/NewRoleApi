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
    }
}
