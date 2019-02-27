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
using SubzzV2.Core.Entities;
using SubzzV2.Core.Models;

namespace Subzz.Api.Controllers.Authentication
{
    [Route("api/auth")]
    public class AuthenticationController : Controller
    {
        private readonly IUserAuthenticationService _service;
        private readonly IUserService _userService;
        public AuthenticationController(IUserService userService, IUserAuthenticationService service)
        {
            _service = service;
            _userService = userService;
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
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("P@$sw0rd123Ki@Keysec"));
                var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var tokeOptions = new JwtSecurityToken(
                    //issuer: "http://localhost:56412",
                    //audience: "http://localhost:56412",
                    claims: new List<Claim> { new Claim("UserId", UserInfo.UserId.ToString()) },
                    expires: DateTime.Now.AddMinutes(20),
                    signingCredentials: credentials
                );

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
    }
}