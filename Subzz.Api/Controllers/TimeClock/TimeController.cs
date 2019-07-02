
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
using System.Text;

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
        public IActionResult InsertClockInTimeNew(string userId)
        {
            try
            {
                var model = new SubzzV2.Core.Models.TimeClock
                {
                    UserIdd = CurrentUser.Id,
                    UpdatedOn = DateTime.Now,
                    ActivityTime = DateTime.Now,
                    Activity = TimeClockActivity.ActionType.Clockin
                };
                var Summary = _service.InsertClockInTimeNew(model);
                return Ok(Summary);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

     

        [Route("clockout")]
        [HttpPost]
        public IActionResult InsertClockOutTimeNew(string userId)
        {
            try
            {
                var model = new SubzzV2.Core.Models.TimeClock
                {
                    UserIdd = base.CurrentUser.Id,
                    UpdatedOn = DateTime.Now,
                    ActivityTime = DateTime.Now,
                    Activity = TimeClockActivity.ActionType.Clockout
                };
                var Summary = _service.InsertClockOutTimeNew(model);
                return Ok(Summary);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

   

        [Route("break")]
        [HttpPost]
        public IActionResult TimeClockBreakStatusNew(string userId)
        {
            try
            {
                var model = new SubzzV2.Core.Models.TimeClock
                {
                    UserIdd = base.CurrentUser.Id,
                    UpdatedOn = DateTime.Now,
                    ActivityTime = DateTime.Now,
                    BreakTime = DateTime.Now.TimeOfDay,
                    Activity = TimeClockActivity.ActionType.Break,
                    TakeBreakTime = DateTime.Now.TimeOfDay
                };
                var Summary = _service.TimeClockBreakStatusNew(model);
                return Ok(Summary);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

       

        [Route("return")]
        [HttpPost]
        public IActionResult TimeClockReturnStatusNew(string userId)
        {
            try
            {
                var model = new SubzzV2.Core.Models.TimeClock
                {
                    UserIdd = base.CurrentUser.Id,
                    UpdatedOn = DateTime.Now,
                    ActivityTime = DateTime.Now,
                    ReturnFromBreakTime = DateTime.Now.TimeOfDay,
                    Activity = TimeClockActivity.ActionType.Return,
                    TakeBreakTime = DateTime.Now.TimeOfDay
                };
                var Summary = _service.TimeClockReturnStatusNew(model);
                return Ok(Summary);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("timeclockdata")]
        [HttpGet]
        public async Task<IEnumerable<SubzzV2.Core.Models.TimeClock>> GetTimeClockData(string userId)
        {
            try
            { 
            var model = new SubzzV2.Core.Models.TimeClock
            {
                UserId = base.CurrentUser.Id,
                Activity = TimeClockActivity.ActionType.Return
            };
            return await _service.GetTimeClockData(model);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;

        }
        [Route("timeclockstatus")]
        [HttpGet]
        public IActionResult CheckTimeClockStatus()
        {
            try
            { 
            var model = new SubzzV2.Core.Models.TimeClock
            {
                UserId = base.CurrentUser.Id,
                Activity = TimeClockActivity.ActionType.Return
            };
            var Summary = _service.CheckTimeClockStatus(model);
            return Json(Summary);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("timeclockdatawithfilter")]
        [HttpPost]
        public async Task<IEnumerable<SubzzV2.Core.Models.TimeClock>> GetTimeClockSummaryWithFilter([FromBody]TimeclockFilter model)
        {
            try
            { 
            model.DistrictId = base.CurrentUser.DistrictId;
            model.OrganizationId = base.CurrentUser.OrganizationId;
            model.UserId = base.CurrentUser.Id;
            return await _service.GetTimeClockSummaryWithFilter(model);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;

        }

        [Route("gettimetrackerdata")]
        [HttpPost]
        public async Task<IEnumerable<SubzzV2.Core.Models.TimeClock>> GetTimeTrackerSummary([FromBody]TimeclockFilter model)
        {
            try
            { 
            model.DistrictId = base.CurrentUser.DistrictId;
            model.OrganizationId = base.CurrentUser.OrganizationId;
            model.UserId = base.CurrentUser.Id;
            return await _service.GetTimeTrackerSummary(model);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;

        }

        [Route("timetrackerdatawithfilter")]
        [HttpPost]
        public IActionResult GetTimeTrackerDataWithFilter([FromBody]TimeclockFilter model)
        {
            try
            { 
            model.DistrictId = base.CurrentUser.DistrictId;
            model.OrganizationId = base.CurrentUser.OrganizationId;
            model.UserId = base.CurrentUser.Id;
            var reportDetails = _service.GetTimeTrackerDataWithFilter(model);
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

        [Route("updateTimeClockData")]
        [HttpPatch]
        public ActionResult UpdateTimeClockData([FromBody]SubzzV2.Core.Models.TimeClock model)
        {
            try
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