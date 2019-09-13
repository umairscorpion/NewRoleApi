using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Subzz.Api.Controllers.Base;
using Subzz.Business.Services.Users.Interface;
using Subzz.Integration.Core.Container;
using Subzz.Integration.Core.Domain;
using Subzz.Integration.Core.Helper;
using SubzzV2.Core.Enum;
using SubzzV2.Core.Models;

namespace Subzz.Api.Controllers.User
{
    [Route("api/user")]
    public class UserController : BaseApiController
    {
        private readonly IUserService _service;
        private IHostingEnvironment _hostingEnvironment;
        private IUserAuthenticationService _authService;
        private readonly IAuditingService _audit;
        public UserController(IUserService service, IUserAuthenticationService authService,
            IHostingEnvironment hostingEnvironment, IAuditingService audit)
        {
            _service = service;
            _hostingEnvironment = hostingEnvironment;
            _authService = authService;
            _audit = audit;
        }

        private CommunicationContainer _communicationContainer;
        public virtual CommunicationContainer CommunicationContainer
        {
            get
            {
                return _communicationContainer ?? (_communicationContainer = new CommunicationContainer());
            }
        }

        [Route("list/summary/{districtId}")]
        [HttpGet]
        public IActionResult GetUsersList(int districtId)
        {
            try
            {
                var result = _service.GetUsersSummaryList(districtId);
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
                    UserRoleDesciption = userModel.UserRoleDesciption,
                    OrganizationName = userModel.OrganizationName,
                    DistrictName = userModel.DistrictName,
                    SecondarySchools = userModel.SecondarySchools
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
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = model.UserId.ToString(),
                    EntityType = AuditLogs.EntityType.ChangedPassword,
                    ActionType = AuditLogs.ActionType.ChangedPassword,
                    PostValue = Serializer.Serialize(model),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);
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
                var userModel = _service.InsertUser(model);
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = model.UserId.ToString(),
                    EntityType = model.RoleId == 4 ? AuditLogs.EntityType.Substitute : AuditLogs.EntityType.Staff,
                    ActionType = model.RoleId == 4 ? AuditLogs.ActionType.CreatedSubstitute : AuditLogs.ActionType.CreatedEmployee,
                    PostValue = Serializer.Serialize(model),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

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

        [Route("insertUserAndSendWelcomeEmail")]
        [HttpPost]
        public SubzzV2.Core.Entities.User InsertUserAndSendWelcomeEmail([FromBody]SubzzV2.Core.Entities.User model)
        {
            try
            {
                var userModel = _service.InsertUser(model);
                SendWelcomeLetter(userModel);
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = model.UserId.ToString(),
                    EntityType = model.RoleId == 4 ? AuditLogs.EntityType.Substitute : AuditLogs.EntityType.Staff,
                    ActionType = model.RoleId == 4 ? AuditLogs.ActionType.CreatedSubAndSWE : AuditLogs.ActionType.CreatedEmpAndSWE,
                    PostValue = Serializer.Serialize(model),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

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

        //[Route("resendWelcomeLetter")]
        //[HttpPost]
        //public void ResendWelcomeLetter([FromBody]SubzzV2.Core.Entities.User user)
        //{
            
        //        SendWelcomeLetter(user);
        //}

        [Route("resendWelcomeLetter")]
        [HttpPost]
        public IActionResult ResendWelcomeLetter([FromBody]SubzzV2.Core.Entities.User user)
        {
            try
                Task.Run(() => SendWelcomeLetter(user));
            // Audit Log
            var audit = new AuditLog
            {
                UserId = CurrentUser.Id,
                EntityId = user.UserId.ToString(),
                EntityType = user.RoleId == 4 ? AuditLogs.EntityType.Substitute : AuditLogs.EntityType.Staff,
                ActionType = user.RoleId == 4 ? AuditLogs.ActionType.SentWelcomeEmailToSub : AuditLogs.ActionType.SentWelcomeEmailToEmp,
                PostValue = Serializer.Serialize(user),
                DistrictId = CurrentUser.DistrictId,
                OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
            };
            _audit.InsertAuditLog(audit);
            return Ok();
            }
            catch(Exception ex)
            {

            }
            return null;
        }
        [Route("sendWellcomeLetterToAll/{DistrictId}/{UserRole}")]
        [HttpGet]
        public IActionResult SendWelcomeLetterToAll(int districtId, int userRole)
        {          
            try
            {              
                 IEnumerable<SubzzV2.Core.Entities.User> users = _service.GetUsersByDistrictId(districtId, userRole);
                 Task.Run(() => SendWellcomeEmailToAll(users));
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = districtId.ToString(),
                    EntityType = userRole == 4 ? AuditLogs.EntityType.Substitute : AuditLogs.EntityType.Staff,
                    ActionType = userRole == 4 ? AuditLogs.ActionType.SentWelcomeEmailToAllSub : AuditLogs.ActionType.SentWelcomeEmailToAllEmp,
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit); 
                return Ok();                         
            }
            catch(Exception ex)
            {
            }
            return null;         
        }

        async Task SendWellcomeEmailToAll(IEnumerable<SubzzV2.Core.Entities.User> users)
        {
            foreach (var user in users)
            {
                try
                {
                    Message message = new Message();
                    message.Password = user.Password;
                    message.UserName = user.FirstName;
                    message.SendTo = user.Email;
                    message.Photo = user.ProfilePicture;
                    message.TemplateId = 25;
                    await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                }
                catch (Exception ex)
                {
                }
            }
        }

        //private void SendWelcomeLetter(SubzzV2.Core.Entities.User user)
        //{
        //    Message message = new Message();
        //    message.Password = user.Password;
        //    message.UserName = user.FirstName;
        //    message.SendTo = user.Email;
        //    message.Photo = user.ProfilePicture;
        //    message.TemplateId = 25;
        //    CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
        //}

        async Task SendWelcomeLetter(SubzzV2.Core.Entities.User user)
        {
            try
            {
                Message message = new Message();
                message.Password = user.Password;
                message.UserName = user.FirstName;
                message.SendTo = user.Email;
                message.Photo = user.ProfilePicture;
                message.TemplateId = 25;
                await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
            }
            catch(Exception ex)
            {

            }
        }
        [Route("updateUser")]
        [HttpPatch]
        public SubzzV2.Core.Entities.User UpdateUser([FromBody]SubzzV2.Core.Entities.User model)
        {
            try
            {
                var UserModel = _service.UpdateUser(model);
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = model.UserId.ToString(),
                    EntityType = model.RoleId == 4 ? AuditLogs.EntityType.Substitute : AuditLogs.EntityType.Staff,
                    ActionType = model.RoleId == 4 ? AuditLogs.ActionType.UpdatedSubstitute : AuditLogs.ActionType.UpdatedEmployee,
                    PostValue = Serializer.Serialize(model),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

                return UserModel;
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
                var Profile = _service.UpdateUserProfile(model);
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = model.UserId.ToString(),
                    EntityType = AuditLogs.EntityType.User,
                    ActionType = AuditLogs.ActionType.UpdatedProfile,
                    PostValue = Serializer.Serialize(model),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);
                return Profile;
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

        [HttpGet]
        [Route("getTemporarySubstitutes")]
        public IEnumerable<SubzzV2.Core.Entities.User> GetTemporarySubstitutes()
        {
            try
            {
                var Users = _service.GetTemporarySubstitutes();
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

        [Route("deleteTemporarySubstitutes")]
        [HttpGet]
        public IActionResult DeleteTemporarySubstitutes()
        {
            try
            {
                var DistrictId = 0;
                var DeleteTemporarySchools = _service.DeleteTemporarySubstitutes(DistrictId);
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

        [HttpGet]
        [Route("getTemporaryStaff")]
        public IEnumerable<SubzzV2.Core.Entities.User> GetTemporaryStaff()
        {
            try
            {
                var Users = _service.GetTemporaryStaff();
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

        [Route("deleteTemporaryStaff")]
        [HttpGet]
        public IActionResult DeleteTemporaryStaff()
        {
            try
            {
                var DistrictId = 0;
                var DeleteTemporarySchools = _service.DeleteTemporaryStaff(DistrictId);
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

        [Route("updateUserStatus")]
        [HttpPatch]
        public SubzzV2.Core.Entities.User UpdateUserStatus([FromBody]SubzzV2.Core.Entities.User model)
        {
            try
            { 
                var Model = _service.UpdateUserStatus(model);
                // Audit Log
                if (model.IsActive == true)
                {
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = model.UserId.ToString(),
                        EntityType = model.RoleId == 4 ? AuditLogs.EntityType.Substitute : AuditLogs.EntityType.Staff,
                        ActionType = model.RoleId == 4 ? AuditLogs.ActionType.SubstituteActive : AuditLogs.ActionType.EmployeeActive,
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
                else
                {
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = model.UserId.ToString(),
                        EntityType = model.RoleId == 4 ? AuditLogs.EntityType.Substitute : AuditLogs.EntityType.Staff,
                        ActionType = model.RoleId == 4 ? AuditLogs.ActionType.SubstituteInactive : AuditLogs.ActionType.EmployeeInactive,
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
                return Model;
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
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = userModel.UserId.ToString(),
                    EntityType = userModel.RoleId == 4 ? AuditLogs.EntityType.Substitute : AuditLogs.EntityType.Staff,
                    ActionType = userModel.RoleId == 4 ? AuditLogs.ActionType.ViewedSubstitute : AuditLogs.ActionType.ViewedEmployee,
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);
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
                var EmployeeSuggestions = _service.GetEmployeeSuggestions(searchText, isSearchSubstitute, orgId, districtId);
                return EmployeeSuggestions;
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
            var result = _service.DeleteUser(id);
            // Audit Log
            var audit = new AuditLog
            {
                UserId = CurrentUser.Id,
                EntityId = id.ToString(),
                EntityType = AuditLogs.EntityType.Staff,
                ActionType = AuditLogs.ActionType.DeletedEmployee,
                DistrictId = CurrentUser.DistrictId,
                OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
            };
            _audit.InsertAuditLog(audit);
            return result;
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

        [Route("getSubstituteCategoriesById/{id}")]
        [HttpGet]
        public IEnumerable<SubstituteCategoryModel> GetSubstituteCategoriesById(string id)
        {
            try
            {
                var Categories = _service.GetSubstituteCategories(id);
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

        [Route("getGradeLevelsForNotification")]
        [HttpGet]
        public IEnumerable<SubstituteCategoryModel> GetGradeLevelsForNotification()
        {
            try
            {
                var UserId = base.CurrentUser.Id;
                var events = _service.GetGradeLevelsForNotification(UserId);
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

        [Route("getGradeLevelsForNotificationById/{id}")]
        [HttpGet]
        public IEnumerable<SubstituteCategoryModel> GetGradeLevelsForNotificationById(string id)
        {
            try
            {
                var events = _service.GetGradeLevelsForNotification(id);
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

        [Route("getSubjectsForNotifications")]
        [HttpGet]
        public IEnumerable<SubstituteCategoryModel> GetSubjectsForNotifications()
        {
            try
            {
                var UserId = base.CurrentUser.Id;
                var events = _service.GetSubjectsForNotifications(UserId);
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

        [Route("getSubjectsForNotificationsById/{id}")]
        [HttpGet]
        public IEnumerable<SubstituteCategoryModel> GetSubjectsForNotificationsById(string id)
        {
            try
            {
                var events = _service.GetSubjectsForNotifications(id);
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
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityType = AuditLogs.EntityType.User,
                    ActionType = AuditLogs.ActionType.UpdatedCategorySettings,
                    PostValue = Serializer.Serialize(substituteCategoryModel),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

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
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityType = AuditLogs.EntityType.User,
                    ActionType = AuditLogs.ActionType.UpdatedNotifySettings,
                    PostValue = Serializer.Serialize(substituteEventModel),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

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

        [Route("updateGradeLevelNotification")]
        [HttpPatch]
        public int UpdateGradeLevelNotification([FromBody] List<SubstituteCategoryModel> substituteEventModel)
        {
            try
            {
                foreach (SubstituteCategoryModel cate in substituteEventModel)
                {
                    //cate.SubstituteId = base.CurrentUser.Id;
                    var events = _service.UpdateGradeLevelNotification(cate);
                }
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityType = AuditLogs.EntityType.User,
                    ActionType = AuditLogs.ActionType.UpdatedNotifySettings,
                    PostValue = Serializer.Serialize(substituteEventModel),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

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

        [Route("updateSubjectNotification")]
        [HttpPatch]
        public int UpdateSubjectNotification([FromBody] List<SubstituteCategoryModel> substituteEventModel)
        {
            try
            {
                foreach (SubstituteCategoryModel cate in substituteEventModel)
                {
                    cate.SubstituteId = base.CurrentUser.Id;
                    var events = _service.UpdateSubjectNotification(cate);
                }
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityType = AuditLogs.EntityType.User,
                    ActionType = AuditLogs.ActionType.UpdatedNotifySettings,
                    PostValue = Serializer.Serialize(substituteEventModel),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

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

        [Route("updateUserNotifications")]
        [HttpPatch]
        public int UpdateUserNotifications([FromBody] SubstituteCategoryModel substituteEventModel)
        {
            try
            {
                substituteEventModel.SubstituteId = base.CurrentUser.Id;
                var events = _service.UpdateUserNotifications(substituteEventModel);
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityType = AuditLogs.EntityType.User,
                    ActionType = AuditLogs.ActionType.UpdatedNotifySettings,
                    PostValue = Serializer.Serialize(substituteEventModel),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

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
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityType = AuditLogs.EntityType.User,
                    ActionType = AuditLogs.ActionType.UpdatedSubPreference,
                    PostValue = Serializer.Serialize(substitutePreferenceModel),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

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
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityType = AuditLogs.EntityType.User,
                    ActionType = AuditLogs.ActionType.UpdatedSchoolSettings,
                    PostValue = Serializer.Serialize(preferredSchoolModel),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

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
                var Position= _service.InsertPositions(position);
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = Position.Id.ToString(),
                    EntityType = AuditLogs.EntityType.Substitute,
                    ActionType = AuditLogs.ActionType.CreatedSubPosition,
                    PostValue = Serializer.Serialize(position),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

                return Position;
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
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = position.Id.ToString(),
                    EntityType = AuditLogs.EntityType.Substitute,
                    ActionType = AuditLogs.ActionType.UpdatedSubPosition,
                    PostValue = Serializer.Serialize(position),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

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
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = id.ToString(),
                    EntityType = AuditLogs.EntityType.Substitute,
                    ActionType = AuditLogs.ActionType.DeletedSubPosition,
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

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

        [Route("verifySubstitutesData/{DistrictId}")]
        [HttpPost]
        public IActionResult VerifyData(int DistrictId)
        {
            var file = Request.Form.Files[0];
            var stream = file.OpenReadStream();
            IExcelDataReader reader = null;

            if (file.FileName.EndsWith(".xls"))
            {
                reader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else if (file.FileName.EndsWith(".xlsx"))
            {
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            else
            {
                return Ok(1);
            }

            DataSet excelRecords = reader.AsDataSet();
            reader.Close();

            var finalRecords = excelRecords.Tables[0];
            if (finalRecords.Rows.Count <= 1)
            {
                return Ok(2);
            }

            var deletesubstitutes = _service.DeleteTemporarySubstitutes(DistrictId);
            for (int i = 1; i < finalRecords.Rows.Count; i++)
            {
                var Status = "";
                SubzzV2.Core.Entities.User model = new SubzzV2.Core.Entities.User();
                if (string.IsNullOrEmpty(finalRecords.Rows[i][0].ToString()))
                {
                    Status = "Please Fill First Name in column 1. ";
                    model.FirstName = null;
                }
                else
                {
                    model.FirstName = finalRecords.Rows[i][0].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][1].ToString()))
                {
                    Status = Status + "Please Fill Last Name in column 2. ";
                    model.LastName = null;
                }
                else
                {
                    model.LastName = finalRecords.Rows[i][1].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][2].ToString()))
                {
                    Status = Status + "Please Fill User Type in column 3. ";
                    model.UserTypeId = 0;
                }
                else
                {
                    model.UserTypeDescription = finalRecords.Rows[i][2].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][3].ToString()))
                {
                    Status = Status + "Please Fill Email in column 4. ";
                    model.Email = null;
                }
                else
                {
                    model.Email = finalRecords.Rows[i][3].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][4].ToString()))
                {
                    Status = Status + "Please Fill Phone Number in column 5. ";
                    model.PhoneNumber = null;
                }
                else
                {
                    model.PhoneNumber = "+" + finalRecords.Rows[i][4].ToString();
                }
                model.DistrictId = DistrictId;
                var record = _service.InsertTemporarySubstitutes(model, Status); 
                
                //var userModel = _service.InsertUser(model);
            }

            return Ok(3);
        }

        [Route("importSubstitutes/{DistrictId}")]
        [HttpPost]
        public IActionResult Upload(int DistrictId)
        {
            var file = Request.Form.Files[0];
            var stream = file.OpenReadStream();
            IExcelDataReader reader = null;

            if (file.FileName.EndsWith(".xls"))
            {
                reader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else if (file.FileName.EndsWith(".xlsx"))
            {
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            else
            {
                return Ok(1);
            }

            DataSet excelRecords = reader.AsDataSet();
            reader.Close();

            var finalRecords = excelRecords.Tables[0];
            if (finalRecords.Rows.Count <= 1)
            {
                return Ok(2);
            }

            var deletesubstitutes = _service.DeleteTemporarySubstitutes(DistrictId);
            for (int i = 1; i < finalRecords.Rows.Count; i++)
            {
                var Status = "";
                SubzzV2.Core.Entities.User model = new SubzzV2.Core.Entities.User();
                if (string.IsNullOrEmpty(finalRecords.Rows[i][0].ToString()))
                {
                    Status = "Please Fill First Name in column 1 at row " + i;
                    return Json(Status);
                }
                else
                {
                    model.FirstName = finalRecords.Rows[i][0].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][1].ToString()))
                {
                    Status = Status + "Please Fill Last Name in column 2 at row " + i;
                    return Json(Status);
                }
                else
                {
                    model.LastName = finalRecords.Rows[i][1].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][2].ToString()))
                {
                    Status = Status + "Please Fill User Type in column 3 at row " + i;
                    return Json(Status);
                }
                else
                {
                    model.UserTypeDescription = finalRecords.Rows[i][2].ToString();
                    var positions = _service.GetPositions(DistrictId);
                    var pos = positions.Where(x => x.Title == model.UserTypeDescription).FirstOrDefault();
                    if (pos == null)
                        return Json("Position type does not exists in selected district. Please enter another position type in column 3 at row " + i);
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][3].ToString()))
                {
                    Status = Status + "Please Fill Email in column 4 at row " + i;
                    return Json(Status);
                }
                else
                {
                    model.Email = finalRecords.Rows[i][3].ToString();
                    var Emails = _service.VerifyUser(model);
                    if (Emails)
                        return Json("Email already belongs to another user. Please enter another email address in column 4 at row " + i);
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][4].ToString()))
                {
                    Status = Status + "Please Fill Phone Number in column 5 at row " + i;
                    return Json(Status);
                }
                else
                {
                    model.PhoneNumber = "+" + finalRecords.Rows[i][4].ToString();
                }

                
                    
                }
            for (int i = 1; i < finalRecords.Rows.Count; i++)
            {
                SubzzV2.Core.Entities.User model = new SubzzV2.Core.Entities.User();
                model.FirstName = finalRecords.Rows[i][0].ToString();
                model.LastName = finalRecords.Rows[i][1].ToString();
                model.UserTypeDescription = finalRecords.Rows[i][2].ToString();
                var positions = _service.GetPositions(DistrictId);
                var pos = positions.Where(x => x.Title == model.UserTypeDescription).FirstOrDefault();
                model.UserTypeId = pos.Id;
                model.TeachingLevel = 0;
                model.SpecialityTypeId = 0;
                model.RoleId = 4;
                model.Gender = "M";
                model.DistrictId = DistrictId;
                model.Email = finalRecords.Rows[i][3].ToString();
                model.PhoneNumber = "+" + finalRecords.Rows[i][4].ToString();
                model.IsSubscribedEmail = true;
                model.IsSubscribedSMS = true;
                model.ProfilePicture = "noimage.png";
                model.PayRate = 20;
                model.HourLimit = 20;
                model.Password = "Password1";
                var userModel = _service.InsertUser(model);
            }

            return Ok(3);
        }


        [Route("verifyStaffData/{DistrictId}")]
        [HttpPost]
        public IActionResult VerifyStaffData(int DistrictId)
        {
            var file = Request.Form.Files[0];
            var stream = file.OpenReadStream();
            IExcelDataReader reader = null;

            if (file.FileName.EndsWith(".xls"))
            {
                reader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else if (file.FileName.EndsWith(".xlsx"))
            {
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            else
            {
                return Ok(1);
            }

            DataSet excelRecords = reader.AsDataSet();
            reader.Close();

            var finalRecords = excelRecords.Tables[0];
            if (finalRecords.Rows.Count <= 1)
            {
                return Ok(2);
            }

            var deleteStaff = _service.DeleteTemporaryStaff(DistrictId);
            for (int i = 1; i < finalRecords.Rows.Count; i++)
            {
                var Status = "";
                SubzzV2.Core.Entities.User model = new SubzzV2.Core.Entities.User();
                if(string.IsNullOrEmpty(finalRecords.Rows[i][0].ToString()))
                {
                    Status = "Please Fill First Name in column 1. ";
                }
                else
                {
                    model.FirstName = finalRecords.Rows[i][0].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][1].ToString()))
                {
                    Status = Status + "Please Fill Last Name in column 2. ";
                }
                else
                {
                    model.LastName = finalRecords.Rows[i][1].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][2].ToString()))
                {
                    Status = Status + "Please Fill User Role in column 3. ";
                }
                else
                {
                    model.UserRoleDesciption = finalRecords.Rows[i][2].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][3].ToString()))
                {
                    Status = Status + "Please Fill Work Location in column 4. ";
                }
                else
                {
                    model.OrganizationName = finalRecords.Rows[i][3].ToString();

                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][4].ToString()))
                {
                    Status = Status + "Please Fill Grade in column 5. ";
                }
                else
                {
                    model.UserGrade = finalRecords.Rows[i][4].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][5].ToString()))
                {
                    Status = Status + "Please Fill Subject in column 6.  ";
                }
                else
                {
                    model.UserSubject = finalRecords.Rows[i][5].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][6].ToString()))
                {
                    Status = Status + "Please Fill Employe Email in column 7. ";
                }
                else
                {
                    model.Email = finalRecords.Rows[i][6].ToString();

                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][7].ToString()))
                {
                    Status = Status + "Please Fill Phone Number in column 8. ";
                }
                else
                {
                    model.PhoneNumber = "+" + finalRecords.Rows[i][7].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][8].ToString()))
                {
                    Status = Status + "Please Fill User Level in column 9. ";
                }
                else
                {
                    model.UserLevelDescription = finalRecords.Rows[i][8].ToString();
                }
                model.DistrictId = DistrictId;
                var record = _service.InsertTemporaryStaff(model, Status);
            }

            return Ok(3);
        }

        [Route("importStaff/{DistrictId}")]
        [HttpPost]
        public IActionResult ImportStaff(int DistrictId)
        {
            var file = Request.Form.Files[0];
            var stream = file.OpenReadStream();
            IExcelDataReader reader = null;

            if (file.FileName.EndsWith(".xls"))
            {
                reader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else if (file.FileName.EndsWith(".xlsx"))
            {
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            else
            {
                return Ok(1);
            }

            DataSet excelRecords = reader.AsDataSet();
            reader.Close();

            var finalRecords = excelRecords.Tables[0];
            if (finalRecords.Rows.Count <= 1)
            {
                return Ok(2);
            }

            var deletesubstitutes = _service.DeleteTemporarySubstitutes(DistrictId);
            for (int i = 1; i < finalRecords.Rows.Count; i++)
            {
                var Status = "";
                SubzzV2.Core.Entities.User model = new SubzzV2.Core.Entities.User();
                if (string.IsNullOrEmpty(finalRecords.Rows[i][0].ToString()))
                {
                    Status = "Please Fill First Name in column 1 at row " + i;
                    return Json(Status);
                }
                else
                {
                    model.FirstName = finalRecords.Rows[i][0].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][1].ToString()))
                {
                    Status = Status + "Please Fill Last Name in column 2 at row " + i;
                    return Json(Status);
                }
                else
                {
                    model.LastName = finalRecords.Rows[i][1].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][2].ToString()))
                {
                    Status = Status + "Please Fill User Role in column 3 at row " + i;
                    return Json(Status);
                }
                else
                {
                    model.UserRoleDesciption = finalRecords.Rows[i][2].ToString();
                    var Roles = _service.GetAllUserRoles();
                    var role = Roles.Where(x => x.RoleName == model.UserRoleDesciption).FirstOrDefault();
                    if (role == null)
                        return Json("Role doesnot exists in database. Please enter another role in column 3 at row " + i);

                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][3].ToString()))
                {
                    Status = Status + "Please Fill Work Location in column 4 at row " + i;
                    return Json(Status);
                }
                else
                {
                        
                        if (string.IsNullOrEmpty(finalRecords.Rows[i][8].ToString()))
                        {
                            Status = Status + "Please Fill User Level in column 9 at row " + i;
                            return Json(Status);
                        }
                        else
                        {
                            if (finalRecords.Rows[i][8].ToString() == "Campus" || finalRecords.Rows[i][8].ToString() == "District")
                            {
                                model.OrganizationName = finalRecords.Rows[i][3].ToString();
                                var Schools = _service.GetSchools();
                                var school = Schools.Where(x => x.SchoolName == model.OrganizationName).FirstOrDefault();
                                var Districts = _service.GetDistricts();
                                var district = Schools.Where(x => x.DistrictName == model.OrganizationName).FirstOrDefault();
                                if (school == null && district == null)
                                    return Json("Work Location doesnot exists in database. Please enter another location in column 4 at row " + i);
                            }
                            else
                            {
                            return Json("User Level should be Campus or District in column 9.");
                            }
                        }
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][4].ToString()))
                {
                    Status = Status + "Please Fill Grade in column 5 at row " + i;
                    return Json(Status);
                }
                else
                {
                    model.UserGrade = finalRecords.Rows[i][4].ToString();
                    var Grades = _service.GetTeachingLevels();
                    var grade = Grades.Where(x => x.Title == model.UserGrade).FirstOrDefault();
                    if (grade == null)
                        return Json("Grade doesnot exists in database. Please enter another grade in column 5 at row " + i);
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][5].ToString()))
                {
                    Status = Status + "Please Fill Subject in column 6 at row " + i;
                    return Json(Status);
                }
                else
                {
                    model.UserSubject = finalRecords.Rows[i][5].ToString();
                    var Subjects = _service.GetTeachingSubjects();
                    var subject = Subjects.Where(x => x.Title == model.UserSubject).FirstOrDefault();
                    if (subject == null)
                        return Json("Subject doesnot exists in database. Please enter another Subject in column 6 at row " + i);
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][6].ToString()))
                {
                    Status = Status + "Please Fill Employe Email in column 7 at row " + i;
                    return Json(Status);
                }
                else
                {
                    model.Email = finalRecords.Rows[i][6].ToString();
                    var Emails = _service.VerifyUser(model);
                    if (Emails)
                        return Json("Email already belongs to another user. Please enter another email address in column 7 at row " + i);

                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][7].ToString()))
                {
                    Status = Status + "Please Fill Phone Number in column 8 at row " + i;
                    return Json(Status);
                }
                else
                {
                    model.PhoneNumber = "+" + finalRecords.Rows[i][7].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][8].ToString()))
                {
                    Status = Status + "Please Fill User Level in column 9 at row " + i;
                    return Json(Status);
                }
                else
                {
                    //model.UserLevel = "+" + finalRecords.Rows[i][8].ToString();
                }

            }
            for (int i = 1; i < finalRecords.Rows.Count; i++)
            {
                SubzzV2.Core.Entities.User model = new SubzzV2.Core.Entities.User();
                model.FirstName = finalRecords.Rows[i][0].ToString();
                model.LastName = finalRecords.Rows[i][1].ToString();
                model.UserRoleDesciption = finalRecords.Rows[i][2].ToString();
                var Roles = _service.GetAllUserRoles();
                var role = Roles.Where(x => x.RoleName == model.UserRoleDesciption).FirstOrDefault();
                model.RoleId = role.RoleId;
                //var positions = _service.GetPositions(DistrictId);
                //var pos = positions.Where(x => x.Title == model.UserTypeDescription).FirstOrDefault();
                model.UserTypeId = 1;

                if (model.RoleId == 3)
                {
                    model.UserGrade = finalRecords.Rows[i][4].ToString();
                    var Grades = _service.GetTeachingLevels();
                    var grade = Grades.Where(x => x.Title == model.UserGrade).FirstOrDefault();
                    model.TeachingLevel = grade.Id;
                    model.UserSubject = finalRecords.Rows[i][5].ToString();
                    var Subjects = _service.GetTeachingSubjects();
                    var subject = Subjects.Where(x => x.Title == model.UserSubject).FirstOrDefault();
                    model.SpecialityTypeId = subject.Id;
                }
                if (finalRecords.Rows[i][8].ToString() == "Campus")
                {
                    model.OrganizationName = finalRecords.Rows[i][3].ToString();
                    var Schools = _service.GetSchools();
                    var school = Schools.Where(x => x.SchoolName == model.OrganizationName).FirstOrDefault();
                    model.OrganizationId = school.SchoolId;
                }
                model.Gender = "M";
                model.DistrictId = DistrictId;
                model.Email = finalRecords.Rows[i][6].ToString();
                model.PhoneNumber = "+" + finalRecords.Rows[i][7].ToString();
                model.IsSubscribedEmail = true;
                model.IsSubscribedSMS = true;
                model.ProfilePicture = "noimage.png";
                model.PayRate = 0;
                model.HourLimit = 0;
                model.Password = "Password1";
                var userModel = _service.InsertUser(model);
            }

            return Ok(3);
        }
        #region PayRateSetting
        [Route("payRate")]
        [HttpPost]
        public PayRateSettings InsertPayRate([FromBody] PayRateSettings payRateSettings)
        {
            try
            { 
                var Settings = _service.InsertPayRate(payRateSettings);
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = Settings.Id.ToString(),
                    EntityType = AuditLogs.EntityType.PayRate,
                    ActionType = AuditLogs.ActionType.CreatedPayRate,
                    PostValue = Serializer.Serialize(payRateSettings),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

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
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = payRateSettings.Id.ToString(),
                    EntityType = AuditLogs.EntityType.PayRate,
                    ActionType = AuditLogs.ActionType.UpdatedPayRate,
                    PostValue = Serializer.Serialize(payRateSettings),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

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
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = id.ToString(),
                    EntityType = AuditLogs.EntityType.PayRate,
                    ActionType = AuditLogs.ActionType.DeletedPayRate,
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

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
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = rule.Id.ToString(),
                    EntityType = AuditLogs.EntityType.PayRateRule,
                    ActionType = AuditLogs.ActionType.CreatedPayRateRule,
                    PostValue = Serializer.Serialize(payRateRule),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

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
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = payRateRule.Id.ToString(),
                    EntityType = AuditLogs.EntityType.PayRateRule,
                    ActionType = AuditLogs.ActionType.UpdatedPayRateRule,
                    PostValue = Serializer.Serialize(payRateRule),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

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
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = id.ToString(),
                    EntityType = AuditLogs.EntityType.PayRateRule,
                    ActionType = AuditLogs.ActionType.DeletedPayRateRule,
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

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
        [HttpPost]
        public IEnumerable<SchoolSubList> GetSchoolSubList([FromBody] SchoolSubList model)
        {
            try
            { 
                var userId = base.CurrentUser.Id;
                var schoolSubs = _service.GetSchoolSubList(userId, model.DistrictId);
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
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityType = AuditLogs.EntityType.User,
                    ActionType = AuditLogs.ActionType.UpdatedSubList,
                    PostValue = Serializer.Serialize(schoolSubList),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

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
        [HttpPost]
        public IEnumerable<SchoolSubList> GetBlockedSchoolSubList([FromBody]SchoolSubList model)
        {
            try
            { 
                var userId = base.CurrentUser.Id;
                var schoolSubs = _service.GetBlockedSchoolSubList(userId, model.DistrictId);
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
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityType = AuditLogs.EntityType.User,
                    ActionType = AuditLogs.ActionType.UpdatedBlockedList,
                    PostValue = Serializer.Serialize(schoolSubList),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

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

        #region SubstituteList
        //Update Substitute Category and Substitute list
        [Route("updateSubstituteList")]
        [HttpPost]
        public IActionResult InsertSubstituteList([FromBody]SubstituteCategory substituteCategory)
        {
            if (substituteCategory.CategoryId == 0)
            {
                var category = _service.InsertSubstituteCategory(substituteCategory);
                substituteCategory.CategoryId = category.CategoryId;
            }
            if(substituteCategory.SubstituteList.Count > 0)
                 _service.UpdateSubstituteCategory(substituteCategory);
            return Ok(substituteCategory);
        }
        //Get Both Categories against district and substitute list against all categories
        [Route("getSubstituteCategoryList/{districtId}")]
        [HttpGet]
        public IActionResult GetSubstituteCategoryList(int districtId)
        {
            var substituteCategories = new List<SubstituteCategory>();
            substituteCategories = _service.GetSubstituteCategoryList(districtId);
            foreach (var substituteCategory in substituteCategories)
            {
                substituteCategory.SubstituteList = _service.GetSubstituteByCategoryId(substituteCategory.CategoryId, districtId);
            }
            return Ok(substituteCategories);
        }

        [Route("getSubstituteListForNewList")]
        [HttpGet]
        public IActionResult GetSubstituteListForNewList()
        {
            var districtId = CurrentUser.DistrictId;
            var SubList = _service.GetSubstituteByCategoryId(0, districtId);
            return Ok(SubList);
        }
        //Get Only Categories against district
        [Route("getSubstituteCategory/{districtId}")]
        [HttpGet]
        public IActionResult GetSubstituteCategory(int districtId)
        {
            var substituteCategories = new List<SubstituteCategory>();
            substituteCategories = _service.GetSubstituteCategoryList(districtId);
            return Ok(substituteCategories);
        }

        [Route("deleteSubstituteCategory/{id}")]
        [HttpDelete]
        public IActionResult DeleteSubstituteCategory(int id)
        {
            _service.DeleteSubstituteCategory(id);
            return Ok();
        }
        //Update Substitute Catgory
        [Route("updateSubstituteCategoryById")]
        [HttpPost]
        public IActionResult UpdateSubstituteCategoryById([FromBody]SubstituteCategory substituteCategory)
        {
            var Cat =_service.UpdateSubstituteCategoryById(substituteCategory);
            return Ok(Cat);
        }
        #endregion
    }
}