using Dapper;
using Microsoft.Extensions.Configuration;
using SubzzLookup.DataAccess.Repositories.Lookups.Interface;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SubzzLookup.DataAccess.Repositories.Lookups
{
    public class LookupRepository : ILookupRepository
    {
        public LookupRepository()
        {
        }
        public LookupRepository(IConfiguration configuration)
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

        public IEnumerable<SubzzV2.Core.Entities.User> GetAllUserRoles()
        {
            var sql = "[Lookup].[GetRoles]";
            var queryParams = new DynamicParameters();
            return Db.Query<SubzzV2.Core.Entities.User>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public IEnumerable<LookupModel> GetTeachingLevels()
        {
            var sql = "[Lookup].[GetTeachingLevels]";
            var queryParams = new DynamicParameters();
            return Db.Query<LookupModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();

        }

        public IEnumerable<LookupModel> GetAvailabilityStatuses()
        {
            var sql = "[Users].[GetAvailabilityStatus]";
            var queryParams = new DynamicParameters();
            return Db.Query<LookupModel>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }
    }
}
