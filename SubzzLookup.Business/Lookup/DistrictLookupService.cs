using SubzzLookup.Business.Lookups.Interface;
using SubzzLookup.DataAccess.Repositories.Interface;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzLookup.Business.Lookups
{
    public class DistrictLookupService : IDistrictLookupService
    {
        private readonly IDistrictLookupRepository _repo;
        public DistrictLookupService(IDistrictLookupRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<CountryModel> GetCountries()
        {
            return _repo.GetCountries();
        }

        public IEnumerable<StateModel> GetStateByCountryId(int counrtyId)
        {
            return _repo.GetStateByCountryId(counrtyId);
        }

        public IEnumerable<LookupModel> GetTimeZone()
        {
            return _repo.GetTimeZone();
        }
    }
}
