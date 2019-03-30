using SubzzLookup.Business.Lookups.Interface;
using SubzzLookup.DataAccess.Repositories.Lookups.Interface;
using SubzzV2.Core.Entities;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzLookup.Business.Lookups
{
    public class LookupService : ILookupService
    {
        private readonly ILookupRepository _repo;
        public LookupService(ILookupRepository repo)
        {
            _repo = repo;
        }
        public IEnumerable<User> GetAllUserRoles()
        {
            return _repo.GetAllUserRoles();
        }

        public IEnumerable<LookupModel> GetTeachingLevels()
        {
            return _repo.GetTeachingLevels();
        }

        public IEnumerable<LookupModel> GetAvailabilityStatuses()
        {
            return _repo.GetAvailabilityStatuses();
        }
    }
}
