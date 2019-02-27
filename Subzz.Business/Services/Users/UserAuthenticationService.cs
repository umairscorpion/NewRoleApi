using Subzz.Business.Services.Users.Interface;
using SubzzV2.Core.Entities;
using Subzz.DataAccess.Repositories.Users;
using Subzz.DataAccess.Repositories.Users.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subzz.Business.Services.Users
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private IUserRepository _repo;
        public UserAuthenticationService()
        {
            _repo = new UserRepository();
        }
        public UserAuthenticationService(IUserRepository repo)
        {
            _repo = repo;
        }
        public UserLogin GetUserByCredentials(string username, string password)
        {
            return _repo.GetUserByCredentials(username, password);
        }
    }
}
