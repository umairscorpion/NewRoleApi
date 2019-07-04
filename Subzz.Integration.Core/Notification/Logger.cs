using Dapper;
using Microsoft.Extensions.Configuration;
using SubzzV2.Integration.Core.Notification.Interface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace Subzz.Integration.Core.Notification
{
    public class Logger : ILogger
    {
        private SqlConnection Conn
        {
            get
            {
                var configurationBuilder = new ConfigurationBuilder();
                var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                configurationBuilder.AddJsonFile(path, false);
                var root = configurationBuilder.Build();
                string Conn = root.GetSection("ConnectionStrings").GetSection("LogContext").Value;
                SqlConnection connection = new SqlConnection(Conn);
                return connection;
            }
        }
        public void LogError(System.Exception ex)
        {
            throw new NotImplementedException();
        }

        public void LogEmail(string emailTo, string message, string subject, string exception, DateTime updatedOn, string absenceId, string statusCode)
        {
            using (var connection = Conn)
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
                Conn.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public void LogSms(string phoneNumber, string message, DateTime sentAt, string absenceId, string senderNo, string exception, string status, string smsId)
        {
            try
            {
                using (var connection = Conn)
                {
                    var sql = "[Subzz_Logs].[Logs].[InsertSmsLog]";
                    var queryParams = new DynamicParameters();
                    queryParams.Add("@phoneNumber", phoneNumber);
                    queryParams.Add("@message", message);
                    queryParams.Add("@sentAt", sentAt);
                    queryParams.Add("@AbsenceId", absenceId);
                    queryParams.Add("@senderNo", senderNo);
                    queryParams.Add("@exception", exception);
                    queryParams.Add("@status", status);
                    queryParams.Add("@smsId", smsId);
                   
                    Conn.ExecuteScalar<int>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            catch(Exception ex)
            {

            }
            finally
            {
            }
        }

        public void LogError(System.Exception ex, string messagePrefix, string appName)
        {
            
        }

        public void LogMailMessage(string messageId, string ipAddress, int action, bool mailboxDatabaseExists)
        {
            throw new NotImplementedException();
        }
    }
}
