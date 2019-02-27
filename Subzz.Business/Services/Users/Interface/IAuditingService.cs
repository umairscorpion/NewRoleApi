using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subzz.Business.Services.Users.Interface
{
    public interface IAuditingService
    {
        void InsertErrorlog(ErrorlogModel model);
    }
}
