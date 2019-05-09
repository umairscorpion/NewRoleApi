using SubzzV2.Core.Models;
using System.Collections.Generic;

namespace Subzz.Business.Services.Users.Interface
{
    public interface IAuditingService
    {
        void InsertErrorlog(ErrorlogModel model);
        void InsertAuditLog(AuditLog model);
        List<AuditLogView> GetAuditView(AuditLogFilter model);
        List<AuditLogAbsenceView> GetAbsencesAuditView(AuditLogFilter model);
    }
}
