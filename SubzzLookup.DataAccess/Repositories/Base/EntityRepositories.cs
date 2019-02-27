using Dapper;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzLookup.DataAccess.Base
{
    public class EntityRepository
    {
        private const string ConnString = "LookupsContext";
        //private IDbConnection _connection;
        //private Database _db;
        protected IDbConnection Db
        {
            get
            {
                //var factory = new DatabaseProviderFactory();
                //_db = factory.Create(ConnString);
                //_connection = _db.CreateConnection();
                return null;
            }
        }
        public bool Delete(string sql, DynamicParameters param, CommandType commandType)
        {
            Db.Execute(sql, param: param, commandType: commandType);
            return Convert.ToBoolean(param.Get<int>("@HasSucceeded"));
        }
    }
}
