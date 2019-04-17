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
        
        // functions related to Employee

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

        public IEnumerable<SubstituteCategoryModel> GetSubstituteCategories(string SubstituteId)
        {
            return _repo.GetSubstituteCategories(SubstituteId);
        }

        public int UpdateUserCategories(SubstituteCategoryModel substituteCategoryModel)
        {
            return _repo.UpdateUserCategories(substituteCategoryModel);
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
    }
}
