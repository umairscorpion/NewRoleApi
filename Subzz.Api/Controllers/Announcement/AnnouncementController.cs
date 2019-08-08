using Microsoft.AspNetCore.Mvc;
using Subzz.Api.Controllers.Base;
using Subzz.Business.Services.Users.Interface;
using Subzz.Integration.Core.Helper;
using SubzzV2.Core.Enum;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Subzz.Api.Controllers.Announcement
{
    [Route("api/announcement")]
    public class AnnouncementController : BaseApiController
    {
        private readonly IAnnouncementService _service;
        private readonly IAuditingService _audit;
        public AnnouncementController(IAnnouncementService service, IAuditingService audit)
        {
            _service = service;
            _audit = audit;
        }

        [Route("insertAnnouncement")]
        [HttpPost]
        public IActionResult InsertAnnouncement([FromBody]Announcements model)
        {
            try
            {
                var AnnouncementId = _service.InsertAnnouncement(model);
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = model.AnnouncementId.ToString(),
                    EntityType = AuditLogs.EntityType.Announcement,
                    ActionType = AuditLogs.ActionType.CreatedAnnouncement,
                    PostValue = Serializer.Serialize(model),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);
                return Json(AnnouncementId);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getAnnouncement")]
        [HttpPost]
        public IActionResult GetAnnouncements([FromBody]Announcements model)
        {
            try
            {
                var Announcements = _service.GetAnnouncements(model);
                return Ok(Announcements);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }
    }
}
