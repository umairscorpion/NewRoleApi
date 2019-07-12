using System;
using System.Collections.Generic;
using System.Linq;
using SubzzV2.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Subzz.Api.Controllers.Base;
using Subzz.Business.Services.Users.Interface;
using SubzzManage.Business.Manage.Interface;
using System.Threading.Tasks;
using SubzzAbsence.Business.Absence.Interface;

namespace Subzz.Api.Controllers.User
{
    [Route("api/availability")]
    public class AvailabilityController : BaseApiController
    {
        private readonly IUserService _service;
        private readonly IJobService _jobService;
        private readonly IAbsenceService _absenceService;
        public AvailabilityController(IUserService service, IJobService jobService, IAbsenceService absenceService)
        {
            _service = service;
            _jobService = jobService;
            _absenceService = absenceService;
        }

        [Route("events")]
        [HttpPost]
        public async Task<IActionResult> Get([FromBody]UserAvailability model)
        {
            try
            {
                model.UserId = base.CurrentUser.Id;
                var startDate = Convert.ToDateTime(model.StartDate);
                var endDate = Convert.ToDateTime(model.EndDate);
                var acceptedAbsences = await _jobService.GetAvailableJobs(Convert.ToDateTime(model.StartDate), Convert.ToDateTime(model.EndDate), model.UserId, base.CurrentUser.OrganizationId, base.CurrentUser.DistrictId, 2, false);
                //Set this to null after getting absences beacause it generates error in stored Procedure
                model.StartDate = null;
                model.EndDate = null;
                var result = _service.GetAvailabilities(model);
                var calendarEvents = CalendarEvents(result);
                //var calendarEvents = CalendarEvents(result.Where(o => o.AvailabilityStatusId != 3));
                //var recurringEvents = CalendarRecurringEvents(result.Where(o => o.AvailabilityStatusId == 3), startDate, endDate);
                var absenceEvents = AbsencesToEvents(acceptedAbsences);
                var events = _absenceService.GetEvents(Convert.ToDateTime(model.StartDate), Convert.ToDateTime(model.EndDate), model.UserId);
                var eventsCalendarView = CalendarEvents(events);
                absenceEvents.AddRange(eventsCalendarView);
                var allEvents = calendarEvents.Concat(absenceEvents);
                //var all = allEvents.Concat(recurringEvents);
                return Ok(allEvents);
            }
            catch (Exception ex)
            {

            }
            finally
            {
            }
            return null;
        }

        [Route("substitutes/summary")]
        [HttpPost]
        public IActionResult GetSubstituteAvailabilitySummary([FromBody]SubstituteAvailability model)
        {
            try
            {
                var summary = new List<SubstituteAvailabilitySummary>();
                summary = _service.GetSubstituteAvailabilitySummary(model).ToList();
                return Ok(summary);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("substitutes")]
        [HttpPost]
        public IActionResult GetSubstituteAvailability([FromBody]SubstituteAvailability model)
        {
            try
            {
                if (model == null)
                {
                    model = new SubstituteAvailability { StartDate = DateTime.Now, AvailabilityStatusId = -1, UserId = "" };
                }
                
                model.DistrictId = base.CurrentUser.DistrictId;
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
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return null;

        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult Get(int id)
        {
            try
            {
                var result = _service.GetAvailabilityById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("")]
        [HttpPost]
        public IActionResult Post([FromBody]UserAvailability model)
        {
            try
            {
                model.UserId = base.CurrentUser.Id;
                model.CreatedBy = base.CurrentUser.Id;
                var result = _service.InsertAvailability(model);
                return Json(result);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("{id}")]
        [HttpPut]
        public IActionResult Put(int id, [FromBody]UserAvailability model)
        {
            try
            {
                model.ModifiedBy = base.CurrentUser.Id;
                var result = _service.UpdateAvailability(model);
                return Json(result);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("{id}")]
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                var model = new UserAvailability();
                model.AvailabilityId = id;
                model.ArchivedBy = base.CurrentUser.Id;
                var result = _service.DeleteAvailability(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("checkSubstituteAvailability")]
        [HttpPost]
        public IActionResult CheckSubstituteAvailability([FromBody]UserAvailability model)
        {
            try
            {
                var result = _service.CheckSubAvailability(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        private List<CalendarEvent> CalendarEvents(IEnumerable<Event> events)
        {
            try
            {
                var cEvents = events.Select(a => new CalendarEvent
                {
                    id = a.EventId,
                    title = a.Title,
                    description = a.Notes,
                    start = DateTime.Parse(Convert.ToDateTime(a.StartDate).ToShortDateString() + " " + a.StartTime).ToString("s"),
                    end = DateTime.Parse(Convert.ToDateTime(a.EndDate).ToShortDateString() + " " + a.EndTime).ToString("s"),
                }).ToList();
                return cEvents;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        private List<CalendarEvent> CalendarEvents(IEnumerable<UserAvailability> availabilities)
        {
            try
            {
                var events = availabilities.Select(a => new CalendarEvent
                {
                    id = a.AvailabilityId,
                    availabilityStatusId = a.AvailabilityStatusId,
                    title = a.AvailabilityContentBackgroundColor == "#d20f0f" && a.IsAllDayOut == false ? Convert.ToDateTime(a.StartTime).ToString("h:mm tt") + "-" + Convert.ToDateTime(a.EndTime).ToString("h:mm tt") + " Unavailable" :
                    a.AvailabilityContentBackgroundColor == "#d20f0f" && a.IsAllDayOut == true ? " Unavailable" :
                    a.AvailabilityContentBackgroundColor == "#0ea8ea" && a.IsAllDayOut == false ? Convert.ToDateTime(a.StartTime).ToString("h:mm tt") + "-" + Convert.ToDateTime(a.EndTime).ToString("h:mm tt") + " Vacation" :
                    a.AvailabilityContentBackgroundColor == "#0ea8ea" && a.IsAllDayOut == true ? " Vacation" :
                    a.AvailabilityContentBackgroundColor == "#0ea8ea" && a.IsAllDayOut == false ? Convert.ToDateTime(a.StartTime).ToString("h:mm tt") + "-" + Convert.ToDateTime(a.EndTime).ToString("h:mm tt") + " Recurring" : " Recurring",
                    description = a.AvailabilityStatusTitle,
                    start = DateTime.Parse(Convert.ToDateTime(a.StartDate).ToShortDateString() + " " + a.StartTime).ToString("s"),
                    end = DateTime.Parse(a.StartTime) == DateTime.Parse(a.EndTime) || DateTime.Parse(a.EndTime) < DateTime.Parse("9:00 AM") ? 
                    DateTime.Parse(Convert.ToDateTime(a.EndDate).AddDays(1).ToShortDateString() + " " + a.EndTime).ToString("s") :
                    DateTime.Parse(Convert.ToDateTime(a.EndDate).ToShortDateString() + " " + a.EndTime).ToString("s"),
                    backgroundColor = a.AvailabilityContentBackgroundColor,
                    allDay = a.IsAllDayOut,
                    className = new string[] { a.AvailabilityIconCss },
                    isRepeat = a.IsRepeat,
                    repeatType = a.RepeatType,
                    repeatValue = a.RepeatValue,
                    repeatOnWeekDays = a.RepeatOnWeekDays,
                    isEndsNever = a.IsEndsNever,
                    endsOnAfterNumberOfOccurrance = a.EndsOnAfterNumberOfOccurrance,
                    endsOnUntilDate = a.EndsOnUntilDate
                }).ToList();
                return events;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        private List<CalendarEvent> AbsencesToEvents(IEnumerable<AbsenceModel> availabilities)
        {
            try
            {
                var events = availabilities.Select(a => new CalendarEvent
                {
                    id = -1,
                    title = DateTime.Today.Add(a.StartTime).ToString("h:mm tt") + "-" + DateTime.Today.Add(a.EndTime).ToString("h:mm tt") + " " + a.EmployeeName,
                    description = a.SubstituteName + " for " + a.EmployeeName,
                    start = DateTime.Parse(Convert.ToDateTime(a.StartDate).ToShortDateString() + " " + a.StartTime).ToString("s"),
                    end = DateTime.Parse(Convert.ToDateTime(a.EndDate).ToShortDateString() + " " + a.EndTime).ToString("s"),
                    organizationName = a.OrganizationId == "-1" ? a.AbsenceLocation : a.OrganizationName,
                    backgroundColor = "#15A315",
                    allDay = false,
                    className = new string[] { "" }
                }).ToList();
                return events;
            }

            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;

        }

        private List<CalendarEvent> CalendarRecurringEvents(IEnumerable<UserAvailability> availabilities, DateTime startDate, DateTime endDate)
        {
            try
            {
                List<CalendarEvent> list = new List<CalendarEvent>();
                CalendarEvent evt;
                foreach (var av in availabilities)
                {
                    if (!av.IsEndsNever)
                    {
                        if (Convert.ToDateTime(av.EndsOnUntilDate) > endDate)
                        {
                            List<DateTime> dateTime = GetDaydBetweenTwoDates(startDate, endDate, Convert.ToInt32(av.RepeatOnWeekDays));
                            foreach(var dates in dateTime)
                            {
                                evt = new CalendarEvent();
                                evt.id = av.AvailabilityId;
                                evt.title = av.IsAllDayOut == false ? Convert.ToDateTime(av.StartTime).ToString("h:mm tt") + "-" + Convert.ToDateTime(av.EndTime).ToString("h:mm tt") + " Recurring" : " Recurring";
                                evt.description = av.AvailabilityStatusTitle;
                                evt.start = DateTime.Parse(Convert.ToDateTime(dates).ToShortDateString() + " " + av.StartTime).ToString("s");
                                evt.end = DateTime.Parse(Convert.ToDateTime(dates).ToShortDateString() + " " + av.StartTime).ToString("s");
                                list.Add(evt);
                            }
                        }
                        else
                        {
                            List<DateTime> dateTime = GetDaydBetweenTwoDates(startDate, Convert.ToDateTime(av.EndsOnUntilDate), Convert.ToInt32(av.RepeatOnWeekDays));
                            foreach (var dates in dateTime)
                            {
                                evt = new CalendarEvent();
                                evt.id = av.AvailabilityId;
                                evt.title = av.IsAllDayOut == false ? Convert.ToDateTime(av.StartTime).ToString("h:mm tt") + "-" + Convert.ToDateTime(av.EndTime).ToString("h:mm tt") + " Recurring" : " Recurring";
                                evt.description = av.AvailabilityStatusTitle;
                                evt.start = DateTime.Parse(Convert.ToDateTime(dates).ToShortDateString() + " " + av.StartTime).ToString("s");
                                evt.end = DateTime.Parse(Convert.ToDateTime(dates).ToShortDateString() + " " + av.StartTime).ToString("s");
                                list.Add(evt);
                            }
                        }
                    }
                    else
                    {
                        List<DateTime> dateTime = GetDaydBetweenTwoDates(startDate, endDate, Convert.ToInt32(av.RepeatOnWeekDays));
                        foreach (var dates in dateTime)
                        {
                            evt = new CalendarEvent();
                            evt.id = av.AvailabilityId;
                            evt.title = av.IsAllDayOut == false ? Convert.ToDateTime(av.StartTime).ToString("h:mm tt") + "-" + Convert.ToDateTime(av.EndTime).ToString("h:mm tt") + " Recurring" : " Recurring";
                            evt.description = av.AvailabilityStatusTitle;
                            evt.start = DateTime.Parse(Convert.ToDateTime(dates).ToShortDateString() + " " + av.StartTime).ToString("s");
                            evt.end = DateTime.Parse(Convert.ToDateTime(dates).ToShortDateString() + " " + av.StartTime).ToString("s");
                            list.Add(evt);
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public List<DateTime> GetDaydBetweenTwoDates(DateTime startDate, DateTime endDate, int dayOfWeek)
        {
            List<DateTime> dateTimes = new List<DateTime>();

            for(int counter= 0, arrayCounter = 0; arrayCounter < (endDate - startDate).TotalDays; counter++)
            {
                DateTime currentDate = startDate;
                int Day = Convert.ToInt32(startDate.DayOfWeek);
                if (Day == dayOfWeek)
                {
                    dateTimes.Add(currentDate);
                    arrayCounter++;
                }
                counter++;
                startDate = startDate.AddDays(1);
            }
            return dateTimes;
        }
    }
}