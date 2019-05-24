using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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
        public AuthenticationController(IUserService userService, IUserAuthenticationService service, IAuditingService audit)
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
            var UserInfo = _service.GetUserByCredentials(user.UserName, user.Password);
            if (user == null)
            {
                return BadRequest("Invalid client request");
            }

            if (UserInfo != null)
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
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost, Route("insertExternalUser")]
        public IActionResult InsertExternalUser([FromBody]ExternalUser externalUser)
        {
            var userModel = _userService.InsertExternalUser(externalUser);
            return Ok(new { Token = "sdsd"});
        }

        [HttpPost, Route("forgotPassword")]
        public IActionResult ForgotPassword([FromBody]SubzzV2.Core.Entities.User user)
        {
            var exist = _userService.CheckEmailExistance(user.Email);
            if (Convert.ToBoolean(exist))
            {
                var resetPassKey = System.Guid.NewGuid().ToString() + user.Email;
                var updated = _userService.UpdatePasswordResetKey(user);
                user.ActivationCode = resetPassKey;
                Subzz.Integration.Core.Domain.Message message = new Integration.Core.Domain.Message();
                message.ActivationCode = resetPassKey;
                message.Email = user.Email;
                message.TemplateId = 4;
                CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
            }
            return Ok(false);
        }
    }
}