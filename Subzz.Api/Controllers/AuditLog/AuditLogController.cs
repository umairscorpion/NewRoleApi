using System;using System.Collections.Generic;using System.IdentityModel.Tokens.Jwt;using System.Linq;using System.Security.Claims;using System.Text;using System.Threading.Tasks;using Microsoft.AspNetCore.Http;using Microsoft.AspNetCore.Mvc;using Microsoft.IdentityModel.Tokens;using Subzz.Api.Controllers.Base;
using Subzz.Business.Services.Users.Interface;using SubzzV2.Core.Entities;using SubzzV2.Core.Enum;
using SubzzV2.Core.Models;namespace Subzz.Api.Controllers.Authentication{    [Route("api/audit")]    public class AuditLogController : BaseApiController    {        private readonly IAuditingService _audit;        public AuditLogController(IAuditingService audit)        {            _audit = audit;        }        [Route("getAuditLog")]
        [HttpPost]
        public IActionResult GetAuditLog([FromBody]AuditLogFilter model)
        {
            model.LoginUserId = base.CurrentUser.Id;
            model.DistrictId = base.CurrentUser.DistrictId;
            model.OrganizationId = base.CurrentUser.OrganizationId;
            var reportDetails = _audit.GetAuditLog(model);
            return Ok(reportDetails);
        }

    }}