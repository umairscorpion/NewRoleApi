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
            try
            {
                var result = _service.GetUsersSummaryList(CurrentUser.DistrictId);
                return Ok(result);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("bulk/delete")]
        [HttpPut]
        public IActionResult RemoveUsers([FromBody]string[] ids)
        {
            try
            {
                if (ids.Length <= 0) return NotFound();
                foreach (var id in ids)
                {
                    _service.DeleteUser(id);
                }
                return Ok();
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("reference/GetUserClaims")]
        [HttpGet]
        public UserReference GetUserClaims()
        {
            try
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
                    Permissions = userModel.Permissions,
                    IsViewedNewVersion = userModel.IsViewedNewVersion,
                    UserRoleDesciption = userModel.UserRoleDesciption
                };
                return referenceModel;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [HttpGet]
        [Route("GetUserResources/{resourceTypeId}/{parentResourceTypeId}/{IsAdminPortal}")]
        public IEnumerable<UserResource> GetUserResourses(int resourceTypeId, int parentResourceTypeId, int IsAdminPortal)
        {
            try
            {
                var UserId = base.CurrentUser.Id;
                return _service.GetUserResources(UserId, resourceTypeId, parentResourceTypeId, IsAdminPortal);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("verify")]
        [HttpPost]
        public IActionResult VerifyUser([FromBody]SubzzV2.Core.Entities.User model)
        {
            try
            { 
                var isExists = _service.VerifyUser(model);
                if (isExists)
                {
                    return Ok(true);
                }
                return Ok(false);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("updatePassword")]
        [HttpPost]
        public IActionResult UpdatePassword([FromBody]SubzzV2.Core.Entities.User model)
        {
            try
            { 
                var user = _service.UpdatePassword(model);
                return Ok();
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("checkUserPassword/{emailId}/{Password}")]
        [HttpGet]
        public IActionResult CheckUserPassword(string emailId, string Password)
        {
            try
            {
                var User = _authService.GetUserByCredentials(emailId, Password);
                if (User == null)
                {
                    return Ok(false);
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        // Functions related to Employees
        [Route("insertUser")]
        [HttpPost]
        public SubzzV2.Core.Entities.User InsertUser([FromBody]SubzzV2.Core.Entities.User model)
        {
            try
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
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("updateUser")]
        [HttpPatch]
        public SubzzV2.Core.Entities.User UpdateUser([FromBody]SubzzV2.Core.Entities.User model)
        {
            try
            {
                return _service.UpdateUser(model);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("updateUserProfile")]
        [HttpPatch]
        public UserReference UpdateUserProfile([FromBody]UserReference model)
        {
            try
            { 
                return _service.UpdateUserProfile(model);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [HttpGet]
        [Route("getUsers/{roleId}/{orgId}/{districtId}")]
        public IEnumerable<SubzzV2.Core.Entities.User> GetUsers(int roleId, string orgId, int districtId)
        {
            try
            {
                var UserId = base.CurrentUser.Id;
                var Users = _service.GetUsers(UserId, roleId, districtId, orgId);
                return Users;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }
        [Route("updateUserStatus")]
        [HttpPatch]
        public SubzzV2.Core.Entities.User UpdateUserStatus([FromBody]SubzzV2.Core.Entities.User model)
        {
            try
            { 
                return _service.UpdateUserStatus(model);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [HttpGet]
        [Route("searchContent/{orgId}/{districtId}/{searchQuery}")]
        public IEnumerable<SubzzV2.Core.Entities.User> SearchContent( string orgId, int districtId, string searchQuery)
        {
            try
            {
                var UserId = base.CurrentUser.Id;
                var Users = _service.SearchContent(UserId, districtId, orgId, searchQuery);
                return Users;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [HttpGet]
        [Route("getUserById/{Id}")]
        public IEnumerable<SubzzV2.Core.Entities.User> GetUserById(string Id)
        {
            try
            { 
                var userModel = _service.GetUserDetail(Id);
                return new List<SubzzV2.Core.Entities.User> { userModel };
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [HttpGet]
        [Route("getEmployeeSuggestions/{searchText}/{isSearchSubstitute}/{orgId}/{districtId}")]
        public IEnumerable<SubzzV2.Core.Entities.User> GetEmployeeSuggestions(string searchText, int isSearchSubstitute, string orgId, int districtId)
        {
            try
            { 
                return _service.GetEmployeeSuggestions(searchText, isSearchSubstitute, orgId, districtId);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
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
            try
            {
                return _service.GetEmployee(id);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getUserTypes")]
        [HttpGet]
        public IEnumerable<LookupModel> GetUserTypes()
        {
            try
            { 
                return _service.GetUserTypes();
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }
        
        [Route("insertExternalUser")]
        [HttpPost]
        public ExternalUser InsertExternalUser([FromBody]ExternalUser externalUser)
        {
            try
            { 
                var userModel = _service.InsertExternalUser(externalUser);
                return externalUser;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getSubstituteCategories")]
        [HttpGet]
        public IEnumerable<SubstituteCategoryModel> GetSubstituteCategories()
        {
            try
            {
                var UserId = base.CurrentUser.Id;
                var Categories = _service.GetSubstituteCategories(UserId);
                return Categories;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getSubstituteNotificationEvents")]
        [HttpGet]
        public IEnumerable<SubstituteCategoryModel> GetSubstituteNotificationEvents()
        {
            try
            {
                var UserId = base.CurrentUser.Id;
                var events = _service.GetSubstituteNotificationEvents(UserId);
                return events;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        //[Route("updateUserCategories")]
        //[HttpPatch]
        //public int UpdateUserCategories([FromBody]SubstituteCategoryModel substituteCategoryModel)
        //{
        //    var UserId = base.CurrentUser.Id;
        //    var Categories = _service.UpdateUserCategories(substituteCategoryModel);
        //    return Categories;
        //}

        [Route("updateUserCategories")]
        [HttpPatch]
        public int UpdateUserCategories([FromBody] List<SubstituteCategoryModel> substituteCategoryModel)
        {
            try
            {
                foreach (SubstituteCategoryModel cat in substituteCategoryModel)
                {
                    var UserId = base.CurrentUser.Id;
                    var Categories = _service.UpdateUserCategories(cat);
                }
                return 1;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return 0;
        }

        [Route("updateNotificationEvents")]
        [HttpPatch]
        public int UpdateNotificationEvents([FromBody] List<SubstituteCategoryModel> substituteEventModel)
        {
            try
            {
                foreach (SubstituteCategoryModel cate in substituteEventModel)
                {
                    cate.SubstituteId = base.CurrentUser.Id;
                    var events = _service.UpdateNotificationEvents(cate);
                }
                return 1;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return 0;

        }

        [Route("getUserLocationTime/{userId}/{userLevel}")]
        [HttpGet]
        public LocationTime GetUserLocationTime(string userId, int userLevel)
        {
            try
            {
                var Categories = _service.GetUserLocationTime(userId, userLevel);
                return Categories;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("updateSubstitutePreferrence")]
        [HttpPost]
        public async Task<IActionResult> UpdateSubstitutePeferrence([FromBody] SubstitutePreferenceModel substitutePreferenceModel)
        {
            try
            { 
                await _service.UpdateSubstitutePeferrence(substitutePreferenceModel);
                return Json("success");
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getFavoriteSubstitutes/{UserId}")]
        [HttpGet]
        public IEnumerable<SubzzV2.Core.Entities.User> GetFavoriteSubstitutes(string UserId)
        {
            try
            {
                return _service.GetFavoriteSubstitutes(UserId);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getBlockedSubstitutes/{UserId}")]
        [HttpGet]
        public IEnumerable<SubzzV2.Core.Entities.User> GetBlockedSubstitutes(string UserId)
        {
            try
            { 
                return _service.GetBlockedSubstitutes(UserId);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getSubstitutePreferredSchools/{UserId}")]
        [HttpGet]
        public IEnumerable<PreferredSchoolModel> GetSubstitutePreferredSchools(string UserId)
        {
            try
            {
                return _service.GetSubstitutePreferredSchools(UserId);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        //[Route("updateEnabledSchools")]
        //[HttpPatch]
        //public async Task<IActionResult> UpdateEnabledSchools([FromBody]PreferredSchoolModel preferredSchoolModel)
        //{
        //    await _service.UpdateEnabledSchools(preferredSchoolModel);
        //    return Json("success");
        //}

        [Route("updateEnabledSchools")]
        [HttpPatch]
        public async Task<IActionResult> UpdateEnabledSchools([FromBody] List<PreferredSchoolModel> preferredSchoolModel)
        {
            try
            { 
                foreach (PreferredSchoolModel cat in preferredSchoolModel)
                {
                    cat.UserId = base.CurrentUser.Id;
                    await _service.UpdateEnabledSchools(cat);
                }
            return Json("success");
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;

        }

        [HttpPost]
        [Route("getAvailableSubstitutes")]
        public IEnumerable<SubzzV2.Core.Entities.User> GetAvailableSubstitutes([FromBody]AbsenceModel absenceModel)
        {
            try
            { 
                return _service.GetAvailableSubstitutes(absenceModel);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [HttpPost]
        [Route("positions")]
        public PositionDetail InsertPositions([FromBody]PositionDetail position)
        {
            try
            { 
                return _service.InsertPositions(position);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [HttpGet]
        [Route("positions/{districtId}")]
        public IActionResult GetPositions(int districtId)
        {
            try
            { 
                var positions = _service.GetPositions(districtId);
                return Ok(positions);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("positions")]
        [HttpPatch]
        public IActionResult UpdatePositions([FromBody]PositionDetail position)
        {
            try
            { 
                var positions = _service.InsertPositions(position);
                return Ok(positions);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("deletePosition/{id}")]
        [HttpDelete]
        public IActionResult DeletePosition(int id)
        {
            try
            { 
                var allowance = _service.DeletePosition(id);
                return Ok(allowance);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        #region PayRateSetting
        [Route("payRate")]
        [HttpPost]
        public PayRateSettings InsertPayRate([FromBody] PayRateSettings payRateSettings)
        {
            try
            { 
                var Settings = _service.InsertPayRate(payRateSettings);
                return Settings;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("payRate")]
        [HttpPatch]
        public IActionResult UpdatePayRate([FromBody]PayRateSettings payRateSettings)
        {
            try
            { 
                var positions = _service.InsertPayRate(payRateSettings);
                return Ok(positions);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getPayRate/{districtId}")]
        [HttpGet]
        public IActionResult GetPayRates(int districtId)
        {
            try
            { 
                var positions = _service.GetPayRates(districtId);
                return Ok(positions);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("deletePayRate/{id}")]
        [HttpDelete]
        public IActionResult DeletePayRate(int id)
        {
            try
            {
                var payRate = new PayRateSettings();
                payRate.Id = id;
                payRate.ArchivedBy = base.CurrentUser.Id; ;
                var result = _service.DeletePayRate(payRate);
                return Ok(result);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("payRateRule")]
        [HttpPost]
        public IActionResult InsertPayRateRule([FromBody]PayRateRule payRateRule)
        {
            try
            {
                var rule = _service.InsertPayRateRule(payRateRule);
                return Ok(rule);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("payRateRule")]
        [HttpPatch]
        public IActionResult UpdatePayRateRule([FromBody]PayRateRule payRateRule)
        {
            try
            { 
                var rule = _service.InsertPayRateRule(payRateRule);
                return Ok(rule);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getPayRateRule/{districtId}")]
        [HttpGet]
        public IActionResult GetPayRateRules(int districtId)
        {
            try
            { 
                var positions = _service.GetPayRateRules(districtId);
                return Ok(positions);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("deletePayRateRule/{id}")]
        [HttpDelete]
        public IActionResult DeletePayRateRule(int id)
        {
            try
            { 
                var payRate = new PayRateRule();
                payRate.Id = id;
                payRate.ArchivedBy = base.CurrentUser.Id; ;
                var result = _service.DeletePayRateRule(payRate);
                return Ok(result);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }
        #endregion

        #region Substitutes
        [Route("schoolSubList")]
        [HttpGet]
        public IEnumerable<SchoolSubList> GetSchoolSubList()
        {
            try
            { 
                var userId = base.CurrentUser.Id;
                var districtId = base.CurrentUser.DistrictId;
                var schoolSubs = _service.GetSchoolSubList(userId, districtId);
                return schoolSubs;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("schoolSubList")]
        [HttpPatch]
        public async Task<ActionResult> UpdateSchoolSubList([FromBody]SchoolSubList schoolSubList)
        {
            try
            { 
                schoolSubList.ModifyByUserId = base.CurrentUser.Id;
                schoolSubList.DistrictId = base.CurrentUser.DistrictId;
                var schoolSubs = await _service.UpdateSchoolSubList(schoolSubList);
                return Ok(schoolSubs);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("blockedSchoolSubList")]
        [HttpGet]
        public IEnumerable<SchoolSubList> GetBlockedSchoolSubList()
        {
            try
            { 
                var userId = base.CurrentUser.Id;
                var districtId = base.CurrentUser.DistrictId;
                var schoolSubs = _service.GetBlockedSchoolSubList(userId, districtId);
                return schoolSubs;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("blockedSchoolSubList")]
        [HttpPatch]
        public async Task<ActionResult> UpdateBlockedSchoolSubList([FromBody]SchoolSubList schoolSubList)
        {
            try
            { 
                schoolSubList.ModifyByUserId = base.CurrentUser.Id;
                schoolSubList.DistrictId = base.CurrentUser.DistrictId;
                var schoolSubs = await _service.UpdateBlockedSchoolSubList(schoolSubList);
                return Ok(schoolSubs);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }
      
        #endregion
    }
}