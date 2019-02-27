using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SubzzV2.Core.Entities;

namespace Subzz.Api.Controllers.Base
{
    [Authorize]
    public class BaseApiController : Controller
    {
        public SubzzV2.Core.Entities.User CurrentUser
        {
            get
            {
                SubzzV2.Core.Entities.User user = new SubzzV2.Core.Entities.User();
                var claims = User.Claims.Select(c => new { Type = c.Type, Value = c.Value });
                foreach (var claim in claims)
                {
                    if (claim.Type == "UserId")
                    {
                        user.Id = claim.Value.ToString();
                    }
                }
                return user;
            }
        }

        public bool AuthenticateUser(string id)
        {
            if (CurrentUser.UserTypeId == 1)
            {
                if (CurrentUser.Id != id)
                {
                    throw new AuthenticationException("You are not authorized to perform this action");
                    // return false;

                }
            }
            return true;
        }
    }
}