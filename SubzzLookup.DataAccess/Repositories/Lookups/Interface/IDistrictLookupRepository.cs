using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzLookup.DataAccess.Repositories.Interface
{
    public interface IDistrictLookupRepository
    {
        IEnumerable<CountryModel> GetCountries();
        IEnumerable<StateModel> GetStateByCountryId(int counrtyId);
        IEnumerable<LookupModel> GetTimeZone();
    }
}
