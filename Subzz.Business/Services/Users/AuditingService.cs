﻿using Subzz.Business.Services.Users.Interface;
using SubzzV2.Core.Models;
using Subzz.DataAccess.Repositories.Users.Interface;
using System.Collections.Generic;

namespace Subzz.Business.Services.Users
{
    public class AuditingService : IAuditingService
    {
        private readonly IAuditingRepository _repo;
        public AuditingService(IAuditingRepository repo)
        {
            _repo = repo;
        }
        
        public void InsertErrorlog(ErrorlogModel model)
        {
            _repo.InsertErrorlog(model);
        }

        public void InsertAuditLog(AuditLog model)
        {
            _repo.InsertAuditLog(model);
        }

        public List<AuditLog> GetAuditLog(AuditLogFilter model)
        {
            return _repo.GetAuditLog(model);
        }
    }
}
