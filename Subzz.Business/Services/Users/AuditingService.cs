using Subzz.Business.Services.Users.Interface;
using SubzzV2.Core.Models;
using Subzz.DataAccess.Repositories.Users;
using Subzz.DataAccess.Repositories.Users.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subzz.Business.Services.Users
{
    public class AuditingService : IAuditingService
    {
        private readonly IAuditingRepository _repo;
        public AuditingService(IAuditingRepository repo)
        {
            _repo = repo;
        }
        public AuditingService()
        {
            _repo = new AuditingRepository();
        }
        public void InsertErrorlog(ErrorlogModel model)
        {
            _repo.InsertErrorlog(model);
        }
    }
}
