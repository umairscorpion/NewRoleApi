using System;
using System.Collections.Generic;
using System.Linq;
using SubzzV2.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Subzz.Api.Controllers.Base;
using Subzz.Business.Services.Users.Interface;

namespace Subzz.Api.Controllers.User
{
    [Route("api/availability")]
    public class AvailabilityController : BaseApiController
    {
        private readonly IUserService _service;

        public AvailabilityController(IUserService service)
        {
            _service = service;
        }

        [Route("")]
        [HttpGet]
        public IActionResult Get()
        {
            var model = new UserAvailability { UserId = base.CurrentUser.Id };
            var result = _service.GetAvailabilities(model);
            var calendarEvents = CalendarEvents(result);
            return Ok(calendarEvents);
        }

        [Route("substitutes/summary")]
        [HttpPost]
        public IActionResult GetSubstituteAvailabilitySummary([FromBody]SubstituteAvailability model)
        {
            var summary = new List<SubstituteAvailabilitySummary>();
            summary = _service.GetSubstituteAvailabilitySummary(model).ToList();
            return Ok(summary);
        }

        [Route("substitutes")]
        [HttpPost]
        public IActionResult GetSubstituteAvailability([FromBody]SubstituteAvailability model)
        {
            if (model == null)
            {
                model = new SubstituteAvailability {StartDate = DateTime.Now, AvailabilityStatusId = -1, UserId = ""};
            }

            var result = _service.GetSubstituteAvailability(model).ToList();
            var resources = result.Select(a => new CalendarResource
            {
                id = a.UserId,
                title = a.FirstName + " " + a.LastName,
                profilePicUrl = a.ProfilePicUrl
            }).Distinct().ToList();
            var events = result.Select(a => new CalendarEvent
            {
                id = a.AvailabilityId,
                title = a.AvailabilityStatusTitle,
                description = "",
                start = DateTime.Parse(Convert.ToDateTime(a.StartDate).ToShortDateString() + " " + Convert.ToDateTime(a.StartTime).ToLongTimeString()).ToString("s"),
                end = DateTime.Parse(Convert.ToDateTime(a.StartDate).ToShortDateString() + " " + Convert.ToDateTime(a.StartTime).ToLongTimeString()).ToString("s"),
                resourceId = a.UserId,
                resourceName = a.FirstName + " " + a.LastName,
                profilePicUrl = a.ProfilePicUrl,
                Resources = resources
            }).ToList();
            return Ok(events);
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult Get(int id)
        {
            var result = _service.GetAvailabilityById(id);
            return Ok(result);
        }

        [Route("")]
        [HttpPost]
        public IActionResult Post([FromBody]UserAvailability model)
        {
            model.UserId = base.CurrentUser.Id;
            model.CreatedBy = base.CurrentUser.Id;
            var result = _service.InsertAvailability(model);
            return Ok(result);
        }

        [Route("{id}")]
        [HttpPut]
        public IActionResult Put(int id, [FromBody]UserAvailability model)
        {
            model.ModifiedBy = base.CurrentUser.Id;
            var result = _service.UpdateAvailability(model);
            return Ok(result);
        }

        [Route("{id}")]
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var model = new UserAvailability();
            model.AvailabilityId = id;
            model.ArchivedBy = base.CurrentUser.Id;
            var result = _service.DeleteAvailability(model);
            return Ok(result);
        }

        private List<CalendarEvent> CalendarEvents(IEnumerable<UserAvailability> availabilities)
        {
            var events = availabilities.Select(a => new CalendarEvent
            {
                id = a.AvailabilityId,
                title = a.Title,
                description = a.Notes,
                start = DateTime.Parse(Convert.ToDateTime(a.StartDate).ToShortDateString() + " " + a.StartTime).ToString("s"),
                end = DateTime.Parse(Convert.ToDateTime(a.EndDate).ToShortDateString() + " " + a.EndTime).ToString("s"),
                backgroundColor = a.AvailabilityContentBackgroundColor,
                allDay = a.IsAllDayOut,
                className = new string[] { a.AvailabilityIconCss }
            }).ToList();
            return events;
        }
    }
}