using Dapper;
using SubzzV2.Core.Models;
using Subzz.DataAccess.Repositories.Base;
using Subzz.DataAccess.Repositories.Users.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Subzz.DataAccess.Repositories.Users
{
    public class AnnouncementRepository : IAnnouncementRepository
    {
        public AnnouncementRepository()
        {
        }
        public AnnouncementRepository(IConfiguration configuration)
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
        
        public string InsertAnnouncement(Announcements model)
        {
            var sql = "[Users].[CreateAnnouncement]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@AnnouncementId", model.AnnouncementId);
            queryParams.Add("@Recipients", model.Recipients);
            queryParams.Add("@DistrictId", model.DistrictId);
            queryParams.Add("@OrganizationId", model.OrganizationId);
            queryParams.Add("@Title", model.Title);
            queryParams.Add("@Message", model.Message);
            queryParams.Add("@ScheduleAnnouncement", model.ScheduleAnnouncement);
            queryParams.Add("@ShowOn", model.ShowOn);
            queryParams.Add("@HideOn", model.HideOn);
            queryParams.Add("@ShowOnDate", model.ShowOnDate);
            queryParams.Add("@HideOnDate", model.HideOnDate);
            queryParams.Add("@ShowOnTime", model.ShowOnTime);
            queryParams.Add("@HideOnTime", model.HideOnTime);
            var result = Db.ExecuteScalar<string>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            return result;
        }

        public List<Announcements> GetAnnouncements(Announcements model)
        {
            var sql = "[Users].[GetAnnouncement]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", model.DistrictId);
            queryParams.Add("@OrganizationId", model.OrganizationId);
            queryParams.Add("@LoginUserId", model.UserId);
            var result = Db.Query<Announcements>(sql, queryParams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }
    }
}
