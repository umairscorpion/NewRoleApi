using Dapper;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Subzz.DataAccess.Repositories.Base
{
    public class DapperRepository<T> : IRepository<T>
    {
        private const string ConnString = "MembershipContext";
        private IDbConnection _connection;
        //private Database _db;
        protected IDbConnection Db
        {
            get
            {
                //var factory = new DatabaseProviderFactory();
                //_db = factory.Create(ConnString);
                //_connection = _db.CreateConnection();
                return _connection;
                //get db connection based on my sql database
                //var connString = ConfigurationManager.ConnectionStrings["MasterContext"].ConnectionString;
                //MySql.Data.MySqlClient.MySqlConnection mySqlConnection = new
                //MySql.Data.MySqlClient.MySqlConnection();
                //mySqlConnection.ConnectionString = connString;
                //mySqlConnection.Open();
                //return mySqlConnection;
            }
        }
        public IEnumerable<T> GetList(string sql, DynamicParameters param, CommandType commandType)
        {
            var list = Db.Query<T>(sql, param: param, commandType: commandType);
            return list;
        }
        public T GetByID(string sql, DynamicParameters param, CommandType commandType)
        {
            return Db.Query<T>(sql, param: param, commandType: commandType).SingleOrDefault();
        }
        public int Insert(string sql, DynamicParameters param, CommandType commandType)
        {   
            return Db.ExecuteScalar<int>(sql, param: param, commandType: commandType);
        }
        public bool InsertTransaction(string sql, DynamicParameters param, CommandType commandType)
        {
            using (var transactionScope = new TransactionScope())
            {
                try
                {
                    Db.ExecuteScalar<int>(sql, param: param, commandType: commandType);
                    transactionScope.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public void Update(string sql, DynamicParameters param, CommandType commandType)
        {
            Db.Execute(sql, param: param, commandType: commandType);
        }
        public bool Delete(string sql, DynamicParameters param, CommandType commandType)
        {
            Db.Execute(sql, param: param, commandType: commandType);
            return Convert.ToBoolean(param.Get<int>("@HasSucceeded"));
        }
    }
}
