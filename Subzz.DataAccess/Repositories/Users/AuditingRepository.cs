﻿using Dapper;
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
    public class AuditingRepository : IAuditingRepository
    {
        public IConfiguration Configuration { get; }
        public AuditingRepository(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        protected IDbConnection Db => new SqlConnection(Configuration.GetConnectionString("MembershipContext"));
        public void InsertErrorlog(ErrorlogModel model)
        {
            //var sql = "dbo.InsertServerAndClientErrors";
            model.ErrorType = "Web Server";
            model.ErrorPriority = 1;
            var queryParams = new DynamicParameters();
            queryParams.Add("@ErrorMessage", model.ErrorMessage);
            queryParams.Add("@ErrorType", model.ErrorType);
            queryParams.Add("@ErrorSource", model.ErrorSource);
            queryParams.Add("@UserId", model.UserId);
            queryParams.Add("@ErrorPriority", model.ErrorPriority);
            queryParams.Add("@ErrorCode", model.ErrorCode);
            queryParams.Add("@ErrorLine", model.ErrorLine);
            queryParams.Add("@ReasonPhrase", model.ReasonPhrase);
            //Db.ExecuteScalar<int>(sql, queryParams, commandType: CommandType.StoredProcedure);
        }

        public void InsertAuditLog(AuditLog model)
        {
            var sql = "[Users].[InsertAuditLog]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@UserId", model.UserId);
            queryParams.Add("@EntityId", model.EntityId);
            queryParams.Add("@EntityType", model.EntityType);
            queryParams.Add("@ActionType", model.ActionType);
            queryParams.Add("@PreValue", model.PreValue);
            queryParams.Add("@PostValue", model.PostValue);
            queryParams.Add("@DistrictId", model.DistrictId);
            queryParams.Add("@OrganizationId", model.OrganizationId);
            var result = Db.Query<AuditLog>(sql, queryParams, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public List<AuditLogView> GetAuditView(AuditLogFilter model)
        {
            var sql = "[Users].[GetAuditLog]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@StartDate", model.StartDate);
            queryParams.Add("@EndDate", model.EndDate);
            queryParams.Add("@LoginUserId", model.LoginUserId);
            queryParams.Add("@SearchByEmployeeName", model.SearchByEmployeeName);
            queryParams.Add("@DistrictId", model.DistrictId);
            queryParams.Add("@OrganizationId", model.OrganizationId == "0" ? null : model.OrganizationId);
            var result = Db.Query<AuditLogView>(sql, queryParams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public List<AuditLogAbsenceView> GetAbsencesAuditView(AuditLogFilter model)
        {
            var sql = "[Users].[GetAuditLogAbsences]";
            var queryParams = new DynamicParameters();
            queryParams.Add("@DistrictId", model.DistrictId);
            queryParams.Add("@OrganizationId", model.OrganizationId);
            if (model.EntityId.Length > 0)
            {
                queryParams.Add("@StartDate", null);
                queryParams.Add("@EndDate", null);
            }
            else
            {
                queryParams.Add("@StartDate", model.StartDate);
                queryParams.Add("@EndDate", model.EndDate);
            }
            queryParams.Add("@EntityId", model.EntityId);
            queryParams.Add("@LoginUserId", model.LoginUserId);
            var result = Db.Query<AuditLogAbsenceView>(sql, queryParams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }
    }
}
