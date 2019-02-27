using SubzzV2.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subzz.Business.Services.Users.Interface
{
    public interface IUserAuthenticationService
    {
        UserLogin GetUserByCredentials(string username, string password);
    }
}
