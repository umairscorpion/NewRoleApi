using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subzz.DataAccess.Repositories.Users.Interface
{
    public interface IAuditingRepository
    {
        void InsertErrorlog(ErrorlogModel model);
    }
}
