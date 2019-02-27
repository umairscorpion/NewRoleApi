using Dapper;
using Microsoft.Extensions.Configuration;
using SubzzSettings.DataAccess.Settings.Interface;
using SubzzV2.Core.Models;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SubzzSettings.DataAccess.Settings
{
    public class UserSettingsRepository : IUserSettingsRepository
    {
        public UserSettingsRepository()
        {
        }
        public UserSettingsRepository(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected IDbConnection Db
        {
            get
            {
                return new SqlConnection(Configuration.GetConnectionString("SettingContext"));
            }
        }
        public IConfiguration Configuration { get; }

        public NoticationSettingsModel GetNotificationSettings(string UserId)
        {
            var sql = "[Setting].[GetUserNotificationSettings]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", UserId);
            return Db.Query<NoticationSettingsModel>(sql, queryParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }
    }
}
