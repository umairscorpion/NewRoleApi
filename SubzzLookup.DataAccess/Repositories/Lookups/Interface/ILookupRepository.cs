using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SubzzLookup.DataAccess.Repositories.Lookups.Interface
{
    public interface ILookupRepository
    {
        IEnumerable<SubzzV2.Core.Entities.User> GetAllUserRoles();
        IEnumerable<LookupModel> GetTeachingLevels();
    }
}
