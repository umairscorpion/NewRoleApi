using Dapper;
using SubzzV2.Core.Entities;
using Subzz.DataAccess.Repositories.Base;
using Subzz.DataAccess.Repositories.Users.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Subzz.DataAccess.Repositories.Users
{
    public class PermissionRepository: IPermissionRepository
    {
        public PermissionRepository()
        {
        }
        public PermissionRepository(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected IDbConnection Db
        {
            get
            {
                return new SqlConnection(Configuration.GetConnectionString("MembershipContext"));
            }
        }
        public IConfiguration Configuration { get; }
        public IEnumerable<User> GetResourcesByParentResourceId(int RoleId, int ParentResourceId)
        {
            var sql = "[Users].[GetResourcesByParentResourceId]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@RoleId", RoleId);
            queryParams.Add("@ParentResourceId", ParentResourceId);
            return Db.Query<User>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }
        public User InsertUserRole(string RoleName)
        {
            var sql = "[Users].[InsertUserRole]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@RoleName", RoleName);
            return Db.Query<User>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
        }
        public IEnumerable<User> GetUserRoles()
        {
            var sql = "[Users].[GetUserRoles]";
            var queryParams = new DynamicParameters();
            return Db.Query<User>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }
    }
}
