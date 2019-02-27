using Dapper;
using SubzzV2.Core.Models;
using Subzz.DataAccess.Repositories.Base;
using Subzz.DataAccess.Repositories.Users.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Subzz.DataAccess.Repositories.Users
{
    public class AuditingRepository : IAuditingRepository
    {
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
    }
}
