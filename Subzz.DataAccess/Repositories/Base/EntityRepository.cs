using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subzz.DataAccess.Repositories.Base
{
    public class EntityRepository
    {
        public EntityRepository(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected IDbConnection Db
        {
            get
            {
                return new SqlConnection(Configuration.GetConnectionString("BookASubConnection"));
            }
        }
        public IConfiguration Configuration { get; }
        public bool Delete(string sql, DynamicParameters param, CommandType commandType)
        {
            Db.Execute(sql, param: param, commandType: commandType);
            return Convert.ToBoolean(param.Get<int>("@HasSucceeded"));
        }
    }
}
