using SubzzLookup.DataAccess.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubzzLookup.DataAccess.Base;
using SubzzV2.Core.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SubzzLookup.DataAccess.Repositories
{
    public class DistrictLookupRepository : IDistrictLookupRepository
    {
        public DistrictLookupRepository()
        {
        }
        public DistrictLookupRepository(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected IDbConnection Db
        {
            get
            {
                return new SqlConnection(Configuration.GetConnectionString("LookupsContext"));
            }
        }
        public IConfiguration Configuration { get; }
        public IEnumerable<CountryModel> GetCountries()
        {
            var sql = "[Lookup].[GetCountries]";
            var queryParams = new DynamicParameters();
            return Db.Query<CountryModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public IEnumerable<StateModel> GetStateByCountryId(int counrtyId)
        {
            var sql = "[Lookup].[GetStatesByCounrtyId]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@counrtyId", counrtyId);
            return Db.Query<StateModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }
    }
}
