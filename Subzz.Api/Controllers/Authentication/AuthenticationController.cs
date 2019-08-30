using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
            var data = DataProtectionProvider.Create("Subzz");
            var protector = data.CreateProtector("secretAdmin@0192837465");
            try
            {
                model.Email = protector.Unprotect(model.Email);
                model.Password = protector.Unprotect(model.Password);
                if (model.Action > 0 && model.Action != 5)
                    model.AbsenceId = protector.Unprotect(model.AbsenceId); //when action = 5 then there is no JobId 
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid");
            }
            finally
            {
                protector = null;
            }
        }
    }
}