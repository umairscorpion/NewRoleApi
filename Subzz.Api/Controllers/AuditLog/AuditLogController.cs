﻿using System;
using Subzz.Business.Services.Users.Interface;
using SubzzV2.Core.Models;
        [HttpPost]
        public IActionResult GetAuditLog([FromBody]AuditLogFilter model)
        {
            model.LoginUserId = base.CurrentUser.Id;
            model.DistrictId = base.CurrentUser.DistrictId;
            model.OrganizationId = base.CurrentUser.OrganizationId;
            var reportDetails = _audit.GetAuditLog(model);
            return Ok(reportDetails);
        }

    }