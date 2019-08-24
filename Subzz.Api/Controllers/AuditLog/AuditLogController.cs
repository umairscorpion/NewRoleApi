using System;using System.Collections.Generic;using System.IdentityModel.Tokens.Jwt;using System.Linq;using System.Security.Claims;using System.Text;using System.Threading.Tasks;using Microsoft.AspNetCore.Http;using Microsoft.AspNetCore.Mvc;using Microsoft.IdentityModel.Tokens;using Subzz.Api.Controllers.Base;
using Subzz.Business.Services.Users.Interface;using SubzzV2.Core.Entities;using SubzzV2.Core.Enum;
using SubzzV2.Core.Models;namespace Subzz.Api.Controllers.Authentication{    [Route("api/audit")]    public class AuditLogController : BaseApiController    {        private readonly IAuditingService _audit;        public AuditLogController(IAuditingService audit)        {            _audit = audit;        }        [Route("view")]
        [HttpPost]
        public IActionResult GetAuditView([FromBody]AuditLogFilter model)
        {
            try
            {
                model.LoginUserId = base.CurrentUser.Id;
                var reportDetails = _audit.GetAuditView(model);
                return Ok(reportDetails);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("view/absence")]
        [HttpPost]
        public IActionResult GetAbsencesAuditView([FromBody]AuditLogFilter model)
        {
            try
            {
                model.LoginUserId = base.CurrentUser.Id;
                model.DistrictId = base.CurrentUser.DistrictId;
                model.OrganizationId = base.CurrentUser.OrganizationId == "-1" ? null : base.CurrentUser.OrganizationId;
                var reportDetails = _audit.GetAbsencesAuditView(model);
                return Ok(reportDetails);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("view/insertAbsenceAuditLog")]
        [HttpPost]
        public IActionResult InsertAbsencesLogView([FromBody]AuditLogFilter model)
        {
            try
            {
                model.LoginUserId = base.CurrentUser.Id;
                model.DistrictId = base.CurrentUser.DistrictId;
                model.OrganizationId = base.CurrentUser.OrganizationId == "-1" ? null : base.CurrentUser.OrganizationId;
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = model.EntityId.ToString(),
                    EntityType = AuditLogs.EntityType.Absence,
                    ActionType = AuditLogs.ActionType.Viewed,
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);
                return Ok();
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("view/insertAuditLogout")]
        [HttpPost]
        public IActionResult InsertAuditLogout([FromBody]AuditLogFilter model)
        {
            try
            {
                model.LoginUserId = base.CurrentUser.Id;
                model.DistrictId = base.CurrentUser.DistrictId;
                model.OrganizationId = base.CurrentUser.OrganizationId == "-1" ? null : base.CurrentUser.OrganizationId;
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityType = AuditLogs.EntityType.User,
                    ActionType = AuditLogs.ActionType.LoggedOut,
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);
                return Ok();
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }
    }}