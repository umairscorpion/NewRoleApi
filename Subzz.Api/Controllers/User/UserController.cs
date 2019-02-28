using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Subzz.Api.Controllers.Base;
using Subzz.Business.Services.Users.Interface;
using SubzzV2.Core.Models;

namespace Subzz.Api.Controllers.User
{
    [Route("api/user")]
    public class UserController : BaseApiController
    {
        private readonly IUserService _service;
        public UserController(IUserService service, IUserAuthenticationService authSerive)
        {
            _service = service;
        }
        [Route("reference/GetUserClaims")]
        [HttpGet]
        public UserReference GetUserClaims()
        {
            var id = base.CurrentUser.Id;
            var userModel = _service.GetUserDetail(id);
            var referenceModel = new UserReference()
            {
                Id = userModel.UserId,
                UserId = userModel.UserId,
                UserName = userModel.UserName,
                UserTypeId = userModel.UserTypeId,
                RoleId = userModel.RoleId,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Email = userModel.Email,
                PhoneNumber = userModel.PhoneNumber,
                ProfilePicture = userModel.ProfilePicture,
                OrganizationId = userModel.OrganizationId,
                DistrictId = userModel.DistrictId,
                UserLevel = userModel.UserLevel
            };
            return referenceModel;
        }
        [HttpGet]
        [Route("GetUserResources/{resourceTypeId}/{parentResourceTypeId}/{IsAdminPortal}")]
        public IEnumerable<UserResource> GetUserResourses(int resourceTypeId, int parentResourceTypeId, int IsAdminPortal)
        {
            var UserId = base.CurrentUser.Id;
            return _service.GetUserResources(UserId, resourceTypeId, parentResourceTypeId, IsAdminPortal);
        }
        // Functions related to Employees
        [Route("insertUser")]
        [HttpPost]
        public SubzzV2.Core.Entities.User InsertUser([FromBody]SubzzV2.Core.Entities.User model)
        {
            ////In Case Of Substitute
            //if (model.RoleId == 4)
            //{
            //    model.TeachingLevel = 0;
            //    model.Speciality = "N/A";
            //    model.OrganizationId = "N/A";
            //}
            var userModel = _service.InsertUser(model);
            return model;
        }

        [Route("updateUser")]
        [HttpPatch]
        public SubzzV2.Core.Entities.User UpdateUser([FromBody]SubzzV2.Core.Entities.User model)
        {
            //In Case Of Substitute
            //if (model.RoleId == 4)
            //{
            //    model.TeachingLevel = 0 ;
            //    model.Speciality = "N/A" ;
            //    model.OrganizationId = "N/A";
            //}
            return _service.UpdateUser(model);
        }

        [HttpGet]
        [Route("getUsers/{roleId}/{orgId}/{districtId}")]
        public IEnumerable<SubzzV2.Core.Entities.User> GetUsers(int roleId, string orgId, int districtId)
        {
            var UserId = base.CurrentUser.Id;
            var Users = _service.GetUsers(UserId, roleId, districtId, orgId);
            return Users;
        }

        [HttpGet]
        [Route("getUserById/{Id}")]
        public IEnumerable<SubzzV2.Core.Entities.User> GetUserById(string Id)
        {
            var UserId = base.CurrentUser.Id;
            var userModel = _service.GetUserDetail(Id);
            return new List<SubzzV2.Core.Entities.User> { userModel };
        }

        [HttpGet]
        [Route("getEmployeeSuggestions/{searchText}/{isSearchSubstitute}/{orgId}/{districtId}")]
        public IEnumerable<SubzzV2.Core.Entities.User> GetEmployeeSuggestions(string searchText, int isSearchSubstitute, string orgId, int districtId)
        {
            return _service.GetEmployeeSuggestions(searchText, isSearchSubstitute, orgId, districtId);
        }

        [Route("{id}")]
        [HttpDelete]
        public bool Delete(string id)
        {
            return _service.DeleteUser(id);
        }

        [Route("{id}")]
        [HttpGet]
        public IEnumerable<SubzzV2.Core.Entities.User> GetEmployee(int id)
        {
            return _service.GetEmployee(id);
        }

        [Route("getUserTypes")]
        [HttpGet]
        public IEnumerable<LookupModel> GetUserTypes()
        {
            return _service.GetUserTypes();
        }
        
        [Route("insertExternalUser")]
        [HttpPost]
        public ExternalUser InsertExternalUser([FromBody]ExternalUser externalUser)
        {
            var userModel = _service.InsertExternalUser(externalUser);
            return externalUser;
        }

        [Route("getSubstituteCategories")]
        [HttpGet]
        public IEnumerable<SubstituteCategoryModel> GetSubstituteCategories()
        {
            var UserId = base.CurrentUser.Id;
            var Categories = _service.GetSubstituteCategories(UserId);
            return Categories;
        }

        [Route("updateUserCategories")]
        [HttpPatch]
        public int UpdateUserCategories([FromBody]SubstituteCategoryModel substituteCategoryModel)
        {
            var UserId = base.CurrentUser.Id;
            var Categories = _service.UpdateUserCategories(substituteCategoryModel);
            return Categories;
        }

        [Route("getUserLocationTime/{userId}/{userLevel}")]
        [HttpGet]
        public LocationTime GetUserLocationTime(string userId, int userLevel)
        {
            var Categories = _service.GetUserLocationTime(userId, userLevel);
            return Categories;
        }

        [Route("updateSubstitutePreferrence")]
        [HttpPost]
        public async Task<IActionResult> UpdateSubstitutePeferrence([FromBody] SubstitutePreferenceModel substitutePreferenceModel)
        {
            await _service.UpdateSubstitutePeferrence(substitutePreferenceModel);
            return Json("success");
        }

        [Route("getFavoriteSubstitutes/{UserId}")]
        [HttpGet]
        public IEnumerable<SubzzV2.Core.Entities.User> GetFavoriteSubstitutes(string UserId)
        {
            return _service.GetFavoriteSubstitutes(UserId);
        }

        [Route("getBlockedSubstitutes/{UserId}")]
        [HttpGet]
        public IEnumerable<SubzzV2.Core.Entities.User> GetBlockedSubstitutes(string UserId)
        {
            return _service.GetBlockedSubstitutes(UserId);
        }

        [Route("getSubstitutePreferredSchools/{UserId}")]
        [HttpGet]
        public IEnumerable<PreferredSchoolModel> GetSubstitutePreferredSchools(string UserId)
        {
            return _service.GetSubstitutePreferredSchools(UserId);
        }

        [Route("updateEnabledSchools")]
        [HttpGet]
        public async Task<IActionResult> UpdateEnabledSchools(PreferredSchoolModel preferredSchoolModel)
        {
            await _service.UpdateEnabledSchools(preferredSchoolModel);
            return Json("success");
        }

    }
}