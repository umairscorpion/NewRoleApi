using Dapper;
using SubzzV2.Core.Models;
using Subzz.DataAccess.Repositories.Base;
using Subzz.DataAccess.Repositories.Users.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Subzz.DataAccess.Repositories.Users
{
    public class AuditingRepository : IAuditingRepository
    {
        public IConfiguration Configuration { get; }
        public AuditingRepository(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        protected IDbConnection Db => new SqlConnection(Configuration.GetConnectionString("MembershipContext"));
        public void InsertErrorlog(ErrorlogModel model)
        {
            //var sql = "dbo.InsertServerAndClientErrors";
            model.ErrorType = "Web Server";
            model.ErrorPriority = 1;
            var queryParams = new DynamicParameters();
            queryParams.Add("@ErrorMessage", model.ErrorMessage);
            queryParams.Add("@ErrorType", model.ErrorType);
            queryParams.Add("@ErrorSource", model.ErrorSource);
            queryParams.Add("@UserId", model.UserId);
            queryParams.Add("@ErrorPriority", model.ErrorPriority);
            queryParams.Add("@ErrorCode", model.ErrorCode);
            queryParams.Add("@ErrorLine", model.ErrorLine);
            queryParams.Add("@ReasonPhrase", model.ReasonPhrase);
            //Db.ExecuteScalar<int>(sql, queryParams, commandType: CommandType.StoredProcedure);
        }

        public void InsertAuditLog(AuditLog model)
        {
            var sql = "[Users].[InsertAuditLog]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", model.UserId);
            queryParams.Add("@EntityId", model.EntityId);
            queryParams.Add("@EntityType", model.EntityType);
            queryParams.Add("@ActionType", model.ActionType);
            queryParams.Add("@PreValue", model.PreValue);
            queryParams.Add("@PostValue", model.PostValue);
            var result = Db.Query<AuditLog>(sql, queryParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }
    }
}
