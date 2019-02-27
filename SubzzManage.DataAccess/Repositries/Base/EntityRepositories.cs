using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace SubzzManage.DataAccess.Repositries.Base
{
    public class EntityRepository 
    {
        protected SqlConnection GetConnection
        {
            get
            {
                var configurationBuilder = new ConfigurationBuilder();
                var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                configurationBuilder.AddJsonFile(path, false);
                var root = configurationBuilder.Build();
                string Conn = root.GetSection("ConnectionStrings").GetSection("LeaveContext").Value;
                SqlConnection connection = new SqlConnection(Conn);
                connection.Open();
                return connection;
            }
        }

        //public readonly string _connectionString = string.Empty;
        //public EntityRepository()
        //{

        //}
    }
}
