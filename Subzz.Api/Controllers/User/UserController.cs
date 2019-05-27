using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
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
        private IHostingEnvironment _hostingEnvironment;
        private IUserAuthenticationService _authService;
        public UserController(IUserService service, IUserAuthenticationService authService, IHostingEnvironment hostingEnvironment)
        {
            _service = service;
            _hostingEnvironment = hostingEnvironment;
            _authService = authService;
        }

        [Route("list/summary")]
        [HttpGet]
        public IActionResult GetUsersList()
        {
            var result = _service.GetUsersSummaryList(CurrentUser.DistrictId);
            return Ok(result);
        }

        [Route("bulk/delete")]
        [HttpPut]
        public IActionResult RemoveUsers([FromBody]string[] ids)
        {
            if (ids.Length <= 0) return NotFound();
            foreach (var id in ids)
            {
                _service.DeleteUser(id);
            }
            return Ok();
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
                UserLevel = userModel.UserLevel,
                Permissions = userModel.Permissions
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

        [Route("verify")]
        [HttpPost]
        public IActionResult VerifyUser([FromBody]SubzzV2.Core.Entities.User model)
        {
            var isExists = _service.VerifyUser(model);
            if (isExists)
            {
                return BadRequest("This email address belongs to another user. Please try with other one.");
            }
            
            return Ok();
        }

        [Route("updatePassword")]
        [HttpPost]
        public IActionResult UpdatePassword([FromBody]SubzzV2.Core.Entities.User model)
        {
            var user = _service.UpdatePassword(model);
            return Ok();
        }

        [Route("checkUserPassword/{emailId}/{Password}")]
        [HttpGet]
        public IActionResult CheckUserPassword(string emailId, string Password)
        {
            var User = _authService.GetUserByCredentials(emailId, Password);
            if (User == null)
            {
                return Ok(false);
            }
            return Ok(true);
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
        [Route("updateUserStatus")]
        [HttpPatch]
        public SubzzV2.Core.Entities.User UpdateUserStatus([FromBody]SubzzV2.Core.Entities.User model)
        {
            return _service.UpdateUserStatus(model);
        }

        [HttpGet]
        [Route("searchContent/{orgId}/{districtId}/{searchQuery}")]
        public IEnumerable<SubzzV2.Core.Entities.User> SearchContent( string orgId, int districtId, string searchQuery)
        {
            var UserId = base.CurrentUser.Id;
            var Users = _service.SearchContent(UserId, districtId, orgId, searchQuery);
            return Users;
        }

        [HttpGet]
        [Route("getUserById/{Id}")]
        public IEnumerable<SubzzV2.Core.Entities.User> GetUserById(string Id)
        {
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

        [Route("getSubstituteNotificationEvents")]
        [HttpGet]
        public IEnumerable<SubstituteCategoryModel> GetSubstituteNotificationEvents()
        {
            var UserId = base.CurrentUser.Id;
            var events = _service.GetSubstituteNotificationEvents(UserId);
            return events;
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
        [HttpPatch]
        public async Task<IActionResult> UpdateEnabledSchools([FromBody]PreferredSchoolModel preferredSchoolModel)
        {
            await _service.UpdateEnabledSchools(preferredSchoolModel);
            return Json("success");
        }

        [HttpPost]
        [Route("getAvailableSubstitutes")]
        public IEnumerable<SubzzV2.Core.Entities.User> GetAvailableSubstitutes([FromBody]AbsenceModel absenceModel)
        {
            return _service.GetAvailableSubstitutes(absenceModel);
        }

        [HttpPost]
        [Route("positions")]
        public PositionDetail InsertPositions([FromBody]PositionDetail position)
        {
            return _service.InsertPositions(position);
        }

        [HttpGet]
        [Route("positions/{districtId}")]
        public IActionResult GetPositions(int districtId)
        {
            var positions = _service.GetPositions(districtId);
            return Ok(positions);
        }

        [Route("positions")]
        [HttpPatch]
        public IActionResult UpdatePositions([FromBody]PositionDetail position)
        {
            var positions = _service.InsertPositions(position);
            return Ok(positions);
        }

        [Route("deletePosition/{id}")]
        [HttpDelete]
        public IActionResult DeletePosition(int id)
        {
            var allowance = _service.DeletePosition(id);
            return Ok(allowance);
        }

        #region PayRateSetting
        [Route("payRate")]
        [HttpPost]
        public PayRateSettings InsertPayRate([FromBody] PayRateSettings payRateSettings)
        {
            var Settings = _service.InsertPayRate(payRateSettings);
            return Settings;
        }

        [Route("payRate")]
        [HttpPatch]
        public IActionResult UpdatePayRate([FromBody]PayRateSettings payRateSettings)
        {
            var positions = _service.InsertPayRate(payRateSettings);
            return Ok(positions);
        }

        [Route("getPayRate/{districtId}")]
        [HttpGet]
        public IActionResult GetPayRates(int districtId)
        {
            var positions = _service.GetPayRates(districtId);
            return Ok(positions);
        }

        [Route("deletePayRate/{id}")]
        [HttpDelete]
        public IActionResult DeletePayRate(int id)
        {
            var payRate = new PayRateSettings();
            payRate.Id = id;
            payRate.ArchivedBy = base.CurrentUser.Id; ;
            var result = _service.DeletePayRate(payRate);
            return Ok(result);
        }

        [Route("payRateRule")]
        [HttpPost]
        public IActionResult InsertPayRateRule([FromBody]PayRateRule payRateRule)
        {
            var rule = _service.InsertPayRateRule(payRateRule);
            return Ok(rule);
        }

        [Route("payRateRule")]
        [HttpPatch]
        public IActionResult UpdatePayRateRule([FromBody]PayRateRule payRateRule)
        {
            var rule = _service.InsertPayRateRule(payRateRule);
            return Ok(rule);
        }

        [Route("getPayRateRule/{districtId}")]
        [HttpGet]
        public IActionResult GetPayRateRules(int districtId)
        {
            var positions = _service.GetPayRateRules(districtId);
            return Ok(positions);
        }

        [Route("deletePayRateRule/{id}")]
        [HttpDelete]
        public IActionResult DeletePayRateRule(int id)
        {
            var payRate = new PayRateRule();
            payRate.Id = id;
            payRate.ArchivedBy = base.CurrentUser.Id; ;
            var result = _service.DeletePayRateRule(payRate);
            return Ok(result);
        }
        #endregion

        #region Substitutes
        [Route("schoolSubList")]
        [HttpGet]
        public IEnumerable<SchoolSubList> GetSchoolSubList()
        {
            var userId = base.CurrentUser.Id;
            var districtId = base.CurrentUser.DistrictId;
            var schoolSubs = _service.GetSchoolSubList(userId, districtId);
            return schoolSubs;
        }

        [Route("schoolSubList")]
        [HttpPatch]
        public async Task<ActionResult> UpdateSchoolSubList([FromBody]SchoolSubList schoolSubList)
        {
            schoolSubList.ModifyByUserId = base.CurrentUser.Id;
            schoolSubList.DistrictId = base.CurrentUser.DistrictId;
            var schoolSubs = await _service.UpdateSchoolSubList(schoolSubList);
            return Ok(schoolSubs);
        }

        [Route("blockedSchoolSubList")]
        [HttpGet]
        public IEnumerable<SchoolSubList> GetBlockedSchoolSubList()
        {
            var userId = base.CurrentUser.Id;
            var districtId = base.CurrentUser.DistrictId;
            var schoolSubs = _service.GetBlockedSchoolSubList(userId, districtId);
            return schoolSubs;
        }

        [Route("blockedSchoolSubList")]
        [HttpPatch]
        public async Task<ActionResult> UpdateBlockedSchoolSubList([FromBody]SchoolSubList schoolSubList)
        {
            schoolSubList.ModifyByUserId = base.CurrentUser.Id;
            schoolSubList.DistrictId = base.CurrentUser.DistrictId;
            var schoolSubs = await _service.UpdateBlockedSchoolSubList(schoolSubList);
            return Ok(schoolSubs);
        }
      
        #endregion
    }
}