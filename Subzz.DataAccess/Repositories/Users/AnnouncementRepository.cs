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

        public string InsertAnnouncement(OrganizationModel model)
        {
            var sql = "[User].[InsertAnnouncement]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@SchoolId", model.SchoolId);
            queryParams.Add("@SchoolName", model.SchoolName);
            queryParams.Add("@SchoolEmail", model.SchoolEmail);
            queryParams.Add("@SchoolDistrictId", model.SchoolDistrictId);
            queryParams.Add("@SchoolAddress", model.SchoolAddress);
            queryParams.Add("@SchoolEmployees", model.SchoolEmployees);
            queryParams.Add("@SchoolTimeZone", model.SchoolTimeZone);
            queryParams.Add("@SchoolStartTime", model.SchoolStartTime);
            queryParams.Add("@SchoolEndTime", model.SchoolEndTime);
            queryParams.Add("@School1stHalfEnd", model.School1stHalfEnd);
            queryParams.Add("@School2ndHalfStart", model.School2ndHalfStart);
            queryParams.Add("@SchoolCity", model.SchoolCity);
            queryParams.Add("@SchoolPhone", model.SchoolPhone);
            queryParams.Add("@SchoolZipCode", model.SchoolZipCode);
            var result = Db.ExecuteScalar<string>(sql, queryParams, commandType: System.Data.CommandType.StoredProcedure);
            return result;
        }
    }
}
