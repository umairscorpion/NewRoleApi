﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
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
                if(model.RoleId == 4)
                {
                    // Audit Log
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = model.UserId.ToString(),
                        EntityType = AuditLogs.EntityType.Substitute,
                        ActionType = AuditLogs.ActionType.CreatedSubstitute,
                        PostValue = Serializer.Serialize(model),
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
                else
                {
                    // Audit Log
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = model.UserId.ToString(),
                        EntityType = AuditLogs.EntityType.Staff,
                        ActionType = AuditLogs.ActionType.CreatedEmployee,
                        PostValue = Serializer.Serialize(model),
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
                
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
                if (model.RoleId == 4)
                {
                    // Audit Log
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = model.UserId.ToString(),
                        EntityType = AuditLogs.EntityType.Substitute,
                        ActionType = AuditLogs.ActionType.CreatedSubstitute,
                        PostValue = Serializer.Serialize(model),
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
                else
                {
                    // Audit Log
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = model.UserId.ToString(),
                        EntityType = AuditLogs.EntityType.Staff,
                        ActionType = AuditLogs.ActionType.CreatedEmployee,
                        PostValue = Serializer.Serialize(model),
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }

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

        [Route("resendWelcomeLetter")]
        [HttpPost]
        public void ResendWelcomeLetter([FromBody]SubzzV2.Core.Entities.User user)
        {
            if (user.IsActive == true)
            {
                SendWelcomeLetter(user);
            }
        }

        [Route("sendWellcomeLetterToAll/{DistrictId}/{UserRole}")]
        [HttpGet]
        public IActionResult SendWelcomeLetterToAll(int districtId, int userRole)
        {
            
            try
            {
                
                 IEnumerable<SubzzV2.Core.Entities.User> users = _service.GetUsersByDistrictId(districtId, userRole);
                 Task.Run(() => SendWellcomeEmailToAll(users));
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

        private void SendWelcomeLetter(SubzzV2.Core.Entities.User user)
        {
            Message message = new Message();
            message.Password = user.Password;
            message.UserName = user.FirstName;
            message.SendTo = user.Email;
            message.Photo = user.ProfilePicture;
            message.TemplateId = 25;
            CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
        }

        [Route("updateUser")]
        [HttpPatch]
        public SubzzV2.Core.Entities.User UpdateUser([FromBody]SubzzV2.Core.Entities.User model)
        {
            try
            {
                var UserModel = _service.UpdateUser(model);
                if (model.RoleId == 4)
                {
                    // Audit Log
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = model.UserId.ToString(),
                        EntityType = AuditLogs.EntityType.Substitute,
                        ActionType = AuditLogs.ActionType.UpdatedSubstitute,
                        PostValue = Serializer.Serialize(model),
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
                else
                {
                    // Audit Log
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = model.UserId.ToString(),
                        EntityType = AuditLogs.EntityType.Staff,
                        ActionType = AuditLogs.ActionType.UpdatedEmployee,
                        PostValue = Serializer.Serialize(model),
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
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

        [Route("updateUserStatus")]
        [HttpPatch]
        public SubzzV2.Core.Entities.User UpdateUserStatus([FromBody]SubzzV2.Core.Entities.User model)
        {
            try
            { 
                 var Model = _service.UpdateUserStatus(model);

                if (model.RoleId == 4)
                {
                    if(model.IsActive == true)
                    {
                        var audit = new AuditLog
                        {
                            UserId = CurrentUser.Id,
                            EntityId = model.UserId.ToString(),
                            EntityType = AuditLogs.EntityType.Substitute,
                            ActionType = AuditLogs.ActionType.SubstituteActive,
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
                            EntityType = AuditLogs.EntityType.Substitute,
                            ActionType = AuditLogs.ActionType.SubstituteInactive,
                            DistrictId = CurrentUser.DistrictId,
                            OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                        };
                        _audit.InsertAuditLog(audit);
                    }                   
                }
                else
                {
                    if (model.IsActive == true)
                    {
                        var audit = new AuditLog
                        {
                            UserId = CurrentUser.Id,
                            EntityId = model.UserId.ToString(),
                            EntityType = AuditLogs.EntityType.Staff,
                            ActionType = AuditLogs.ActionType.EmployeeActive,
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
                            EntityType = AuditLogs.EntityType.Staff,
                            ActionType = AuditLogs.ActionType.EmployeeInactive,
                            DistrictId = CurrentUser.DistrictId,
                            OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                        };
                        _audit.InsertAuditLog(audit);
                    }
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
                if (userModel.RoleId == 4)
                {
                    // Audit Log
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = userModel.UserId.ToString(),
                        EntityType = AuditLogs.EntityType.Substitute,
                        ActionType = AuditLogs.ActionType.ViewedSubstitute,
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
                else
                {
                    // Audit Log
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = userModel.UserId.ToString(),
                        EntityType = AuditLogs.EntityType.Staff,
                        ActionType = AuditLogs.ActionType.ViewedEmployee,
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
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