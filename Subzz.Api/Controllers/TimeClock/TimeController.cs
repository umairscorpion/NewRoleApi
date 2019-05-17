
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Subzz.Api.Controllers.Base;
using SubzzV2.Core.Models;
using SubzzAbsence.Business.Time.Interface;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Subzz.Api.Custom;
using System.Data;
using Subzz.Integration.Core.Container;
using SubzzV2.Core;
using SubzzV2.Core.Enum;
using Subzz.Business.Services.Users.Interface;
using SubzzAbsence.Business.Time;
using Subzz.Integration.Core.Helper;

namespace Subzz.Api.Controllers.TimeClock
{
    [Route("api/Time")]
    public class TimeController : BaseApiController
    {

        private readonly ITimeService _service;
        private readonly IUserService _userService;
        private IHostingEnvironment _hostingEnvironment;
        public TimeController(ITimeService service, IHostingEnvironment hostingEnvironment, IUserService userService)
        {
            _service = service;
            _userService = userService;
            _hostingEnvironment = hostingEnvironment;
        }

        [Route("clockin")]
        [HttpPost]
        public IActionResult InsertClockInTime(string userId)
        {
            var model = new SubzzV2.Core.Models.TimeClock
            {
                UserId = CurrentUser.Id,
                ClockInDate = DateTime.Now,
                ClockInTime = DateTime.Now.TimeOfDay,
                Activity = TimeClockActivity.ActionType.Clockin
            };
            var Summary = _service.InsertClockInTime(model);
            return Ok(Summary);
        }

        [Route("clockout")]
        [HttpPost]
        public IActionResult InsertClockOutTime(string userId)
        {
            var model = new SubzzV2.Core.Models.TimeClock
            {
                UserId = base.CurrentUser.Id,
                ClockInDate = DateTime.Now,
                ClockOutTime = DateTime.Now.TimeOfDay,
                Activity = TimeClockActivity.ActionType.Clockout
            };
            var Summary = _service.InsertClockOutTime(model);
            return Ok(Summary);
        }

        [Route("break")]
        [HttpPost]
        public IActionResult TimeClockBreakStatus(string userId)
        {
            var model = new SubzzV2.Core.Models.TimeClock
            {
                UserId = base.CurrentUser.Id,
                Activity = TimeClockActivity.ActionType.Break
            };
            var Summary = _service.TimeClockBreakStatus(model);
            return Ok(Summary);
        }

        [Route("return")]
        [HttpPost]
        public IActionResult TimeClockReturnStatus(string userId)
        {
            var model = new SubzzV2.Core.Models.TimeClock
            {
                UserId = base.CurrentUser.Id,
                Activity = TimeClockActivity.ActionType.Return
            };
            var Summary = _service.TimeClockReturnStatus(model);
            return Ok(Summary);
        }

        [Route("timeclockdata")]
        [HttpGet]
        public async Task<IEnumerable<SubzzV2.Core.Models.TimeClock>> GetTimeClockData(string userId)
        {
            var model = new SubzzV2.Core.Models.TimeClock
            {
                UserId = base.CurrentUser.Id,
                Activity = TimeClockActivity.ActionType.Return
            };
            return await _service.GetTimeClockData(model);

        }
        [Route("timeclockstatus")]
        [HttpGet]
        public IActionResult CheckTimeClockStatus()
        {
            var model = new SubzzV2.Core.Models.TimeClock
            {
                UserId = base.CurrentUser.Id,
                Activity = TimeClockActivity.ActionType.Return
            };
            var Summary = _service.CheckTimeClockStatus(model);
            return Json(Summary);
        }

        [Route("timeclockdatawithfilter")]
        [HttpPost]
        public async Task<IEnumerable<SubzzV2.Core.Models.TimeClock>> GetTimeClockSummaryWithFilter([FromBody]TimeclockFilter model)
        {
            model.DistrictId = base.CurrentUser.DistrictId;
            model.OrganizationId = base.CurrentUser.OrganizationId;
            model.UserId = base.CurrentUser.Id;
            return await _service.GetTimeClockSummaryWithFilter(model);

        }

        [Route("gettimetrackerdata")]
        [HttpPost]
        public async Task<IEnumerable<SubzzV2.Core.Models.TimeClock>> GetTimeTrackerSummary([FromBody]TimeclockFilter model)
        {
            model.DistrictId = base.CurrentUser.DistrictId;
            model.OrganizationId = base.CurrentUser.OrganizationId;
            model.UserId = base.CurrentUser.Id;
            return await _service.GetTimeTrackerSummary(model);

        }

        [Route("timetrackerdatawithfilter")]
        [HttpPost]
        public IActionResult GetTimeTrackerDataWithFilter([FromBody]TimeclockFilter model)
        {
            model.DistrictId = base.CurrentUser.DistrictId;
            model.OrganizationId = base.CurrentUser.OrganizationId;
            model.UserId = base.CurrentUser.Id;
            var reportDetails = _service.GetTimeTrackerDataWithFilter(model);
            return Ok(reportDetails);
        }

        [Route("updateTimeClockData")]
        [HttpPatch]
        public ActionResult UpdateTimeClockData([FromBody]SubzzV2.Core.Models.TimeClock model)
        {
            int RowsEffected = _service.UpdateTimeClockData(model);
            if (RowsEffected > 0)
            {
                return Json("success");
            }
            else
            {
                return Json("error");
            }
        }
    }
}