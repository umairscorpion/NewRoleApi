using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Microsoft.Extensions.Configuration;
using SubzzManage.DataAccess.Repositries.Base;
using SubzzManage.DataAccess.Repositries.Error_Log.Interface;
using SubzzV2.Core.Entities;
using SubzzV2.Core.Models;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SubzzManage.DataAccess.Repositries.Error_Log
{
    public class ErrorLogRepository : EntityRepository, IErrorLogRepository
    {
        public int InsertError(string userId, DateTime creationDate, int statusCode, string message, string messageDetail)
        {
            using (var connection = base.GetConnection)
            {
                try
                {
                    var sql = "[Subzz_Logs].[Logs].[InsertError]";
                    var queryParams = new DynamicParameters();
                    queryParams.Add("@userId", userId);
                    queryParams.Add("@creationDate", creationDate);
                    queryParams.Add("@statusCode", statusCode);
                    queryParams.Add("@error", message);
                    queryParams.Add("@errorDetail", messageDetail);
                    connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                }
                finally
                {
                }
            }
            return 1;
        }

        public int InsertEmailLog(string emailTo, string message, string subject, string exception, DateTime updatedOn, string absenceId, string statusCode)
        {
            using (var connection = base.GetConnection)
            {
                try
                {
                    var sql = "[Subzz_Logs].[Logs].[InsertEmailLog]";
                    var queryParams = new DynamicParameters();
                    queryParams.Add("@emailTo", emailTo);
                    queryParams.Add("@message", message);
                    queryParams.Add("@subject", subject);
                    queryParams.Add("@exception", exception);
                    queryParams.Add("@updatedOn", updatedOn);
                    queryParams.Add("@absenceId", absenceId);
                    queryParams.Add("@statusCode", statusCode);
                    connection.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                }
                finally
                {
                }
            }
            return 1;
        }
    }
}
