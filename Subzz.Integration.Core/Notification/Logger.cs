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
        private string _connectionString;
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

        public void LogError(string exception, string messagePrefix, string appName)
        {
            using (var connection = Conn)
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
