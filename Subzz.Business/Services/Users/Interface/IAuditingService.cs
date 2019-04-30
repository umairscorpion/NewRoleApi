using SubzzV2.Core.Models;

namespace Subzz.Business.Services.Users.Interface
{
    public interface IAuditingService
    {
        void InsertErrorlog(ErrorlogModel model);
        void InsertAuditLog(AuditLog model);
    }
}
