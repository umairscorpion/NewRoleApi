﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NETCore.Encrypt;
using Subzz.Api.Controllers.Base;
using Subzz.Business.Services.Users.Interface;
using Subzz.Integration.Core.Container;
using SubzzV2.Core.Entities;
using SubzzV2.Core.Enum;
using SubzzV2.Core.Models;

namespace Subzz.Api.Controllers.Authentication
{
    [Route("api/auth")]
    public class AuthenticationController : Controller
    {
        private readonly IUserAuthenticationService _service;
        private readonly IUserService _userService;
        private readonly IAuditingService _audit;
        public AuthenticationController(IUserService userService, IUserAuthenticationService service,
            IAuditingService audit)
        {
            _service = service;
            _userService = userService;
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

        [HttpPost, Route("login")]
        public IActionResult Login([FromBody]UserLogin user)
        {
            try
            {
                var UserInfo = _service.GetUserByCredentials(user.UserName, user.Password);
                if (user == null)
                {
                    return BadRequest("Invalid client request");
                }

                if (UserInfo != null)
                {
                    if(!UserInfo.IsCertified)
                    {
                        return BadRequest("NotCertified");
                    }
                    else if (!UserInfo.IsActive)
                    {
                        return BadRequest("notactive");
                    }
                    else
                    {
                        var userDetail = _userService.GetUserDetail(UserInfo.UserId);
                        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("P@$sw0rd123Ki@Keysec"));
                        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                        var tokeOptions = new JwtSecurityToken(
                            //issuer: "http://localhost:56412",
                            //audience: "http://localhost:56412",
                            claims: new List<Claim> { new Claim("UserId", UserInfo.UserId.ToString()),
                        new Claim("districtId", userDetail.DistrictId.ToString()),
                        new Claim("organizationId", !string.IsNullOrEmpty(userDetail.OrganizationId)  ? userDetail.OrganizationId.ToString(): "-1")},
                            expires: DateTime.Now.AddMinutes(20),
                            signingCredentials: credentials
                        );
                        // Audit Log
                        var audit = new AuditLog
                        {
                            UserId = UserInfo.UserId,
                            EntityType = AuditLogs.EntityType.User,
                            ActionType = AuditLogs.ActionType.LoggedIn,
                            DistrictId = userDetail.DistrictId,
                            OrganizationId = !string.IsNullOrEmpty(userDetail.OrganizationId) ? userDetail.OrganizationId.ToString() : "null"
                        };
                        _audit.InsertAuditLog(audit);

                        var token = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                        return Ok(new { Token = token });
                    }
                    
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [HttpPost, Route("insertExternalUser")]
        public IActionResult InsertExternalUser([FromBody]ExternalUser externalUser)
        {
            try
            {
                var userModel = _userService.InsertExternalUser(externalUser);
                return Ok(new { Token = "sdsd" });
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [HttpPost, Route("forgotPassword")]
        public IActionResult ForgotPassword([FromBody]SubzzV2.Core.Entities.User user)
        {
            try
            {
                var exist = _userService.CheckEmailExistance(user.Email);
            if (Convert.ToBoolean(exist))
            {
                var resetPassKey = System.Guid.NewGuid().ToString() + user.Email;
                user.ActivationCode = resetPassKey;
                var updated = _userService.UpdatePasswordResetKey(user);
                Subzz.Integration.Core.Domain.Message message = new Integration.Core.Domain.Message();
                message.ActivationCode = resetPassKey;
                message.SendTo = user.Email;
                message.TemplateId = 9;
                CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
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

        [HttpPost, Route("updatePasswordByActivationCode")]
        public IActionResult UpdatePasswordByActivationCode([FromBody]SubzzV2.Core.Entities.User user)
        {
            try
            {
                var userModel = _userService.UpdatePasswordUsingActivationLink(user);
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

        [Route("updateSubscription")]
        [HttpPost]
        public IActionResult UpdateSubscription([FromBody]SubzzV2.Core.Entities.User user)
        {
            try
            {
                var result = _userService.UpdateSubscription(user);
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

        [Route("updateUserStatus")]
        [HttpPost]
        public SubzzV2.Core.Entities.User UpdateUserStatus([FromBody]SubzzV2.Core.Entities.User model)
        {
            try
            {
                var Model = _userService.UpdateUserStatus(model);
                //if (model.RoleId == 4)
                //{
                //    var audit = new AuditLog
                //    {
                //        UserId = CurrentUser.Id,
                //        EntityId = model.UserId.ToString(),
                //        EntityType = AuditLogs.EntityType.Substitute,
                //        ActionType = AuditLogs.ActionType.SubstituteVerification,
                //        DistrictId = CurrentUser.DistrictId,
                //        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                //    };
                //    _audit.InsertAuditLog(audit);
                //}
                //else
                //{
                //    var audit = new AuditLog
                //    {
                //        UserId = CurrentUser.Id,
                //        EntityId = model.UserId.ToString(),
                //        EntityType = AuditLogs.EntityType.Staff,
                //        ActionType = AuditLogs.ActionType.EmployeeVerification,
                //        DistrictId = CurrentUser.DistrictId,
                //        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                //    };
                //    _audit.InsertAuditLog(audit);
                //}
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

        [Route("unprotectdata")]
        [HttpPost]
        public IActionResult Unprotectdata([FromBody]Protected model)
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);
            var root = configurationBuilder.Build();
            var desKey = root.GetSection("KEY").GetSection("SECkey").Value;
            try
            {
                model.Password = EncryptProvider.DESDecrypt(model.Password, desKey);
                var email = model.Email;
                string output = email.Replace(" ", "+");
                model.Email = EncryptProvider.DESDecrypt(output, desKey.ToString());
                if (model.Action == 10) //reset Password from email
                {
                    model.Email = EncryptProvider.DESDecrypt(output, desKey);
                    return Ok(model);
                }
                
                //model.Email = EncryptProvider.DESDecrypt(model.Email, desKey);
                //model.Password = EncryptProvider.DESDecrypt(model.Password, desKey);
                if (model.Action > 0 && model.Action != 5)
                    model.AbsenceId = EncryptProvider.DESDecrypt(model.AbsenceId, desKey); //when action = 5 then there is no JobId 
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid");
            }
            finally
            {
            }
        }
        private static byte[] ConvertFromBase64String(string input)
        {
            string working = input.Replace('-', '+').Replace('_', '/');
            while (working.Length % 3 != 0)
            {
                working += '=';
            }
            try
            {
                return Convert.FromBase64String(working);
            }
            catch (Exception)
            {
                // .Net core 2.1 bug
                // https://github.com/dotnet/corefx/issues/30793
                try
                {
                    return Convert.FromBase64String(input.Replace('-', '+').Replace('_', '/'));
                }
                catch (Exception) { }
                try
                {
                    return Convert.FromBase64String(input.Replace('-', '+').Replace('_', '/') + "=");
                }
                catch (Exception) { }
                try
                {
                    return Convert.FromBase64String(input.Replace('-', '+').Replace('_', '/') + "==");
                }
                catch (Exception) { }

                return null;
            }
        }
    }
}