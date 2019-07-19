using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Subzz.Api.Controllers.Base;
using SubzzV2.Core.Models;
using SubzzAbsence.Business.Absence.Interface;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Subzz.Api.Custom;
using System.Data;
using Subzz.Integration.Core.Container;
using SubzzV2.Core.Enum;
using Subzz.Business.Services.Users.Interface;
using Subzz.Integration.Core.Helper;
using Subzz.Integration.Core.Domain;

namespace Subzz.Api.Controllers.Absence
{
    [Route("api/Absence")]
    public class AbsenceController : BaseApiController
    {
        private readonly IAbsenceService _service;
        private readonly IUserService _userService;
        private IHostingEnvironment _hostingEnvironment;
        private readonly IAuditingService _audit;
        public AbsenceController(IAbsenceService service, IHostingEnvironment hostingEnvironment, IUserService userService, IAuditingService audit)
        {
            _service = service;
            _userService = userService;
            _audit = audit;
            _hostingEnvironment = hostingEnvironment;
        }

        private CommunicationContainer _communicationContainer;
        public virtual CommunicationContainer CommunicationContainer
        {
            get
            {
                return _communicationContainer ?? (_communicationContainer = new CommunicationContainer());
            }
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult Get(int id)
        {
            try
            {
                var result = _service.GetAbsenceDetailByAbsenceId(id);
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

        [Route("uploadFile")]
        [HttpPost]
        public IActionResult UploadFile()
        {
            try
            {
                var file = Request.Form.Files[0];
                string folderName = "Attachment";
                string webRootPath = _hostingEnvironment.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRootPath))
                {
                    webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }
                string filePath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                string fileName = "";
                if (file.Length > 0)
                {
                    fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fileExtention = new System.IO.FileInfo(fileName).Extension;
                    fileName = Guid.NewGuid().ToString() + fileExtention;
                    string fullPath = Path.Combine(filePath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                }
                return Json(fileName);
            }
            catch (System.Exception ex)
            {
                return Json("Upload Failed: " + ex.Message);
            }
        }

        [Route("createAbsence")]
        [HttpPost]
        public async Task<IActionResult> CreateAbsence([FromBody]AbsenceModel model)
        {
            try
            {
                DateTime updatedOn = DateTime.Now;

                var absenceCreation = _service.CreateAbsence(model);
                if (absenceCreation.AbsenceId > 0)
                {
                    // Audit Log
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = absenceCreation.ConfirmationNumber.ToString(),
                        EntityType = AuditLogs.EntityType.Absence,
                        ActionType = AuditLogs.ActionType.Create,
                        PostValue = Serializer.Serialize(model),
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);

                    model.AbsenceId = absenceCreation.AbsenceId;
                    model.ConfirmationNumber = absenceCreation.ConfirmationNumber;
                    DataTable SingleDayAbsences = CustomClass.InsertAbsenceBasicDetailAsSingleDay(absenceCreation.AbsenceId,
                        model.StartDate, model.EndDate, model.StartTime, model.EndTime, model.SubstituteId.Length > 10 ? "-1" : model.SubstituteId, model.Status);
                    Task taskForStoreAbsenceAsSingleDay = _service.SaveAsSingleDayAbsence(SingleDayAbsences);
                    if (model.AbsenceScope == 3)
                    {
                        IEnumerable<SubzzV2.Core.Entities.User> FavSubstitutes =
                            _userService.GetFavoriteSubstitutes(model.EmployeeId);
                        await _service.CreatePreferredAbsenceHistory(FavSubstitutes, model); 
                        if (model.IsApprovalRequired)
                        {
                            Task.Run(() => SendEmailToEmployeeOnJobCreation(model.AbsenceId));
                        }
                        else
                        {
                            //this means required approval from admins
                            Task.Run(() => SendJobPostEmails(model));
                        }
                        
                        return Json(absenceCreation.ConfirmationNumber.ToString());
                    }
                    else
                    {
                        Task.Run(() => SendJobPostEmails(model));
                        return Json(absenceCreation.ConfirmationNumber.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(ex.InnerException);
            }

            return Json("error");
        }

        async Task SendJobPostEmails(AbsenceModel absenceModel)
        {
            Subzz.Integration.Core.Domain.Message message = new Integration.Core.Domain.Message();
            //SubstituteId Contains All Substitute Ids in case Of Request specific Substitute.
            var DataForEmails = _userService.GetUsersForSendingAbsenceNotificationOnEntireSub(absenceModel.DistrictId, absenceModel.OrganizationId, absenceModel.AbsenceId, absenceModel.SubstituteId);
            message.ConfirmationNumber = absenceModel.ConfirmationNumber;
            message.AbsenceId = absenceModel.AbsenceId;
            message.StartTime = DateTime.ParseExact(Convert.ToString(absenceModel.StartTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture).ToSubzzTime();
            message.EndTime = DateTime.ParseExact(Convert.ToString(absenceModel.EndTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture).ToSubzzTime();
            message.StartDate = Convert.ToDateTime(absenceModel.StartDate).ToString("D");
            message.EndDate = Convert.ToDateTime(absenceModel.EndDate).ToString("D");
            message.StartTimeSMS = DateTime.ParseExact(Convert.ToString(absenceModel.StartTime), "HH:mm:ss",
                                CultureInfo.InvariantCulture).ToSubzzTimeForSms();
            message.EndTimeSMS = DateTime.ParseExact(Convert.ToString(absenceModel.EndTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture).ToSubzzTimeForSms();
            if (message.StartDate == message.EndDate)
            {
                message.DateToDisplayInSMS = Convert.ToDateTime(absenceModel.StartDate).ToSubzzDateForSMS();
            }
            else
            {
                message.DateToDisplayInSMS = Convert.ToDateTime(absenceModel.StartDate).ToSubzzShortDateForSMS() + "-" + Convert.ToDateTime(absenceModel.EndDate).ToSubzzDateForSMS();
            }
            message.EmployeeName = DataForEmails.EmployeeName;
            message.Position = DataForEmails.PositionDescription;
            message.Subject = DataForEmails.SubjectDescription;
            message.Grade = DataForEmails.Grade;
            message.Location = DataForEmails.AbsenceLocation;
            message.School = DataForEmails.OrganizationName;
            message.Notes = DataForEmails.SubstituteNotes;
            message.Reason = DataForEmails.AbsenceReasonDescription;
            message.Photo = DataForEmails.EmployeeProfilePicUrl;
            message.AttachedFileName = DataForEmails.AttachedFileName;
            message.FileContentType = DataForEmails.FileContentType;
            message.Duration = DataForEmails.DurationType == 1 ? "Full Day" : DataForEmails.DurationType == 2 ? "First Half" : DataForEmails.DurationType == 3 ? "Second Half" : "Custom";
            //Entire Sustitute Pool or Request Specifc Sub
            if (absenceModel.IsApprovalRequired)
            {
                if (absenceModel.AbsenceScope == 4 || absenceModel.AbsenceScope == 1)
                {
                    foreach (var user in DataForEmails.Users)
                    {
                        try
                        {
                            message.Password = user.Password;
                            message.UserName = user.FirstName;
                            message.SendTo = user.Email;
                            //For Substitutes
                            if (user.RoleId == 4)
                            {
                                var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                                var jobPostedEvent = events.Where(x => x.EventId == 5).First();
                                message.TemplateId = 1;
                                if (user.IsSubscribedEmail)
                                {
                                    if (jobPostedEvent.EmailAlert)
                                    {
                                        var sub = _userService.GetSubjectsForNotifications(user.UserId);
                                        var subjects = sub.Where(x => x.TeacherSpecialityId == absenceModel.SpecialityTypeId).FirstOrDefault();
                                        if (absenceModel.OnlySubjectSpecialist && subjects != null ? subjects.SubjectNotification : true &&
                                            absenceModel.OnlyCertified ? user.IsCertified == 1 : true)
                                            await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                                    }
                                }

                                if (user.IsSubscribedSMS)
                                {
                                    message.PhoneNumber = user.PhoneNumber;
                                    if (jobPostedEvent.TextAlert)
                                    {
                                        message.PhoneNumber = user.PhoneNumber;
                                        var sub = _userService.GetSubjectsForNotifications(user.UserId);
                                        var subjects = sub.Where(x => x.TeacherSpecialityId == absenceModel.SpecialityTypeId).FirstOrDefault();
                                        if (absenceModel.OnlySubjectSpecialist && subjects != null ? subjects.SubjectNotification : true &&
                                            absenceModel.OnlyCertified ? user.IsCertified == 1 : true)
                                            CommunicationContainer.SMSProcessor.Process(message, (MailTemplateEnums)message.TemplateId);
                                    }
                                }
                            }

                            else if (user.RoleId == 3)
                            {
                                message.TemplateId = 10;
                                if (user.IsSubscribedEmail)
                                {
                                    var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                                    var jobPostedEvent = events.Where(x => x.EventId == 2).First();
                                    if (jobPostedEvent.EmailAlert)
                                    {
                                            await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                                    }
                                        
                                }
                            }

                            //For Admins
                            else
                            {
                                message.TemplateId = 2;
                                if (user.IsSubscribedEmail)
                                {
                                    var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                                    var jobPostedEvent = events.Where(x => x.EventId == 2).First();
                                    if (jobPostedEvent.EmailAlert)
                                        await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                //Direct Assign
                else if (absenceModel.AbsenceScope == 2)
                {
                    foreach (var user in DataForEmails.Users)
                    {
                        try
                        {
                            message.UserName = user.FirstName;
                            message.SendTo = user.Email;
                            //For Substitutes
                            if (user.RoleId == 4)
                            {
                                var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                                var jobPostedEvent = events.Where(x => x.EventId == 5).First();
                                message.TemplateId = 7;
                                if (user.IsSubscribedEmail)
                                {
                                    if (jobPostedEvent.EmailAlert)
                                    {
                                        var sub = _userService.GetSubjectsForNotifications(user.UserId);
                                        var subjects = sub.Where(x => x.TeacherSpecialityId == absenceModel.SpecialityTypeId).FirstOrDefault();
                                        if (absenceModel.OnlySubjectSpecialist && subjects != null ? subjects.SubjectNotification : true &&
                                            absenceModel.OnlyCertified ? user.IsCertified == 1 : true)
                                            await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                                    }
                                }
                                if (user.IsSubscribedSMS)
                                {
                                    if (jobPostedEvent.TextAlert)
                                    {
                                        message.PhoneNumber = user.PhoneNumber;
                                        var sub = _userService.GetSubjectsForNotifications(user.UserId);
                                        var subjects = sub.Where(x => x.TeacherSpecialityId == absenceModel.SpecialityTypeId).FirstOrDefault();
                                        if (absenceModel.OnlySubjectSpecialist && subjects != null ? subjects.SubjectNotification : true &&
                                            absenceModel.OnlyCertified ? user.IsCertified == 1 : true)
                                            CommunicationContainer.SMSProcessor.Process(message, (MailTemplateEnums)message.TemplateId);
                                    }
                                }

                            }
                            //For Admins And Employee
                            else
                            {
                                message.TemplateId = 8;
                                message.SubstituteName = DataForEmails.SubstituteName;
                                if (user.IsSubscribedEmail)
                                {
                                    var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                                    var jobPostedEvent = events.Where(x => x.EventId == 5).First();
                                    if (jobPostedEvent.EmailAlert)
                                        await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                else if (absenceModel.AbsenceScope == 5)
                {
                    foreach (var user in DataForEmails.Users)
                    {
                        try
                        {
                            message.Password = user.Password;
                            message.UserName = user.FirstName;
                            message.SendTo = user.Email;
                            //For Substitutes
                            if (user.RoleId == 4)
                            {

                            }

                            else if (user.RoleId == 3)
                            {
                                message.TemplateId = 10;
                                if (user.IsSubscribedEmail)      
                                {
                                    var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                                    var jobPostedEvent = events.Where(x => x.EventId == 2).First();
                                    if (jobPostedEvent.EmailAlert)
                                        await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                                }
                            }

                            //For Admins
                            else
                            {
                                message.TemplateId = 2;
                                if (user.IsSubscribedEmail)
                                {
                                    var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                                    var jobPostedEvent = events.Where(x => x.EventId == 2).First();
                                    if (jobPostedEvent.EmailAlert)
                                        await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            else
            {
                if (absenceModel.AbsenceScope == 4 || absenceModel.AbsenceScope == 1)
                {
                    foreach (var user in DataForEmails.Users)
                    {
                        try
                        {
                            message.Password = user.Password;
                            message.UserName = user.FirstName;
                            message.SendTo = user.Email;

                            if (user.RoleId == 3)
                            {
                                message.TemplateId = 13;
                                if (user.IsSubscribedEmail)
                                {
                                    await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                                }
                            }

                            //For Admins
                            else if (user.RoleId == 1 || user.RoleId == 2)
                            {
                                message.TemplateId = 14;
                                if (user.IsSubscribedEmail)
                                {
                                    await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                //Direct Assign
                else if (absenceModel.AbsenceScope == 2 || absenceModel.AbsenceScope == 3)
                {
                    foreach (var user in DataForEmails.Users)
                    {
                        try
                        {
                            message.UserName = user.FirstName;
                            message.SendTo = user.Email;
                            if (user.RoleId == 3)
                            {
                                message.TemplateId = 13;
                                if (user.IsSubscribedEmail)
                                {
                                    //var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                                    //var jobPostedEvent = events.Where(x => x.EventId == 1).First();
                                    //if (jobPostedEvent.EmailAlert)
                                    await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                                }
                            }

                            //For Admins
                            else if (user.RoleId == 1 || user.RoleId == 2)
                            {
                                message.TemplateId = 14;
                                if (user.IsSubscribedEmail)
                                {
                                    //var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                                    //var jobPostedEvent = events.Where(x => x.EventId == 1).First();
                                    //if (jobPostedEvent.EmailAlert)
                                    await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                //No Sub
                else if (absenceModel.AbsenceScope == 5)
                {
                    foreach (var user in DataForEmails.Users)
                    {
                        try
                        {
                            message.Password = user.Password;
                            message.UserName = user.FirstName;
                            message.SendTo = user.Email;

                            if (user.RoleId == 3)
                            {
                                message.TemplateId = 13;
                                if (user.IsSubscribedEmail)
                                {
                                    await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                                }
                            }

                            //For Admins
                            else if (user.RoleId == 1 || user.RoleId == 2)
                            {
                                message.TemplateId = 14;
                                if (user.IsSubscribedEmail)
                                {
                                    await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
        }

        [Route("getAbsences/{StartDate}/{EndDate}/{UserId}")]
        [HttpGet]
        public IEnumerable<AbsenceModel> GetAbsences(DateTime StartDate, DateTime EndDate, string UserId)
        {
            try
            {
                return _service.GetAbsences(StartDate, EndDate, UserId, null);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getAbsencesScheduleEmployee/{StartDate}/{EndDate}/{UserId}")]
        [HttpGet]
        public IEnumerable<EmployeeSchedule> GetAbsencesScheduleEmployee(DateTime StartDate, DateTime EndDate, string UserId)
        {
            try
            {
                return _service.GetAbsencesScheduleEmployee(StartDate, EndDate, UserId);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("updateAbseceStatus/{ConfirmationNumber}/{AbsenceId}/{StatusId}/{UpdateStatusDate}/{UserId}")]
        [HttpGet]
        public ActionResult UpdateAbsenceStatus(string ConfirmationNumber, int AbsenceId, int statusId, string UpdateStatusDate, string UserId)
        {
            try
            {
                int RowsEffected = _service.UpdateAbsenceStatus(AbsenceId, statusId, Convert.ToDateTime(UpdateStatusDate), UserId);
                if (statusId == 1)
                {
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = ConfirmationNumber.ToString(),
                        EntityType = AuditLogs.EntityType.Absence,
                        ActionType = AuditLogs.ActionType.Released,
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
                else
                {
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = ConfirmationNumber.ToString(),
                        EntityType = AuditLogs.EntityType.Absence,
                        ActionType = AuditLogs.ActionType.Cancelled,
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                }
                if (statusId == 4)
                    Task.Run(() => SendNotificationsOnJobCancelled(AbsenceId));
                if (statusId == 1)
                    Task.Run(() => SendNotificationsOnJobReleased(AbsenceId));
                return Json("success");
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getfile")]
        [HttpPost]
        public IActionResult GetFile([FromBody]AbsenceModel model)
        {
            try
            {
                string folderName = "Attachment";
                string webRootPath = _hostingEnvironment.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRootPath))
                {
                    webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }
                string filePath = Path.Combine(webRootPath, folderName);
                byte[] bytes = System.IO.File.ReadAllBytes(Path.Combine(filePath, model.AttachedFileName));
                return File(bytes, model.FileContentType);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("updateAbsence")]
        [HttpPatch]
        public ActionResult UpdateAbsence([FromBody]AbsenceModel model)
        {
            try
            {
                model.UpdatedById = base.CurrentUser.Id;
                string RowsEffected = _service.UpdateAbsence(model);
                if (RowsEffected == "success")
                {
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = model.ConfirmationNumber.ToString(),
                        EntityType = AuditLogs.EntityType.Absence,
                        ActionType = AuditLogs.ActionType.Update,
                        PreValue = Serializer.Serialize(_service.GetAbsenceDetailByAbsenceId(model.AbsenceId)),
                        PostValue = Serializer.Serialize(model),
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                    Task.Run(() => SendNotificationsOnJobUpdated(model.AbsenceId));
                    DataTable SingleDayAbsences = CustomClass.InsertAbsenceBasicDetailAsSingleDay(model.AbsenceId, model.StartDate, model.EndDate, model.StartTime, model.EndTime, model.SubstituteId.Length > 10 ? "-1" : model.SubstituteId, model.Status);
                    Task taskForStoreAbsenceAsSingleDay = _service.SaveAsSingleDayAbsence(SingleDayAbsences);
                    return Json(RowsEffected);
                }
                else
                {
                    return Json(RowsEffected);
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

        [Route("updateAbseceStatusAndSub/{ConfirmationNumber}/{AbsenceId}/{StatusId}/{UpdateStatusDate}/{UserId}/{SubstituteId}/{SubstituteRequired}")]
        [HttpGet]
        public ActionResult UpdateAbseceStatusAndSub(string ConfirmationNumber, int AbsenceId, int statusId, string UpdateStatusDate, string UserId, string SubstituteId, bool SubstituteRequired)
        {
            try
            {
                int RowsEffected = _service.UpdateAbsenceStatusAndSub(AbsenceId, statusId, Convert.ToDateTime(UpdateStatusDate), UserId, SubstituteId, SubstituteRequired);
                if (RowsEffected > 0)
                {
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = ConfirmationNumber.ToString(),
                        EntityType = AuditLogs.EntityType.Absence,
                        ActionType = AuditLogs.ActionType.Assigned,
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);
                    Task.Run(() => SendNotificationsOnJobAssignedFromReports(AbsenceId));

                    return Json("success");
                }
                return Json("error");
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        //For Dashboard Chart
        [Route("summary")]
        [HttpGet]
        public IActionResult GetSummary()
        {
            try
            {
                var year = DateTime.Now.Year;
                var userId = base.CurrentUser.Id;
                var Summary = _service.GetAbsenceSummary(userId, year);
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

        [Route("getTopTenTeachers")]
        [HttpGet]
        public IActionResult GetTopTenTeachers()
        {
            try
            {
                var userId = base.CurrentUser.Id;
                var Summary = _service.GetTopTenTeachers(userId);
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

        [Route("views/calendar")]
        [HttpPost]
        public IActionResult CalendarView([FromBody]AbsenceModel model)
        {
            try
            {
                var result = _service.GetAbsencesForSharedCalendar(model);
                var events = _service.GetEvents(model.StartDate, model.EndDate, model.EmployeeId);
                var absencesCalendarView = CalendarEvents(result);
                var eventsCalendarView = CalendarEvents(events);
                absencesCalendarView.AddRange(eventsCalendarView);
                return Ok(absencesCalendarView);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        private List<CalendarEvent> CalendarEvents(IEnumerable<AbsenceModel> absences)
        {
            try
            {
                var events = absences.Select(a => new CalendarEvent
                {
                    id = a.AbsenceId,
                    title = DateTime.Today.Add(a.StartTime).ToString("h:mm tt") + "-" + DateTime.Today.Add(a.EndTime).ToString("h:mm tt") + " " + a.EmployeeName,
                    description = a.SubstituteName + " for " + a.EmployeeName,
                    start = DateTime.Parse(Convert.ToDateTime(a.StartDate).ToShortDateString() + " " + a.StartTime).ToString("s"),
                    end = a.StartTime.ToString() == a.EndTime.ToString() || DateTime.Parse(a.EndTime.ToString()) < DateTime.Parse("9:00:00") ?
                    DateTime.Parse(Convert.ToDateTime(a.EndDate).AddDays(1).ToShortDateString() + " " + a.EndTime).ToString("s") :
                    DateTime.Parse(Convert.ToDateTime(a.EndDate).ToShortDateString() + " " + a.EndTime).ToString("s"),
                    organizationName = a.OrganizationId == "-1" ? a.AbsenceLocation : a.OrganizationName,
                    backgroundColor = "#15A315",
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

        private List<CalendarEvent> CalendarEvents(IEnumerable<Event> events)
        {
            try
            {
                var cEvents = events.Select(a => new CalendarEvent
                {
                    id = a.EventId,
                    title = a.Title,
                    description = a.Title,
                    start = DateTime.Parse(Convert.ToDateTime(a.StartDate).ToShortDateString() + " " + a.StartTime).ToString("s"),
                    end = DateTime.Parse(Convert.ToDateTime(a.EndDate).ToShortDateString() + " " + a.EndTime).ToString("s"),
                    forEvents = "Events",
                }).ToList();
                return cEvents;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        async Task SendNotificationsOnJobCancelled(int AbsenceId)
        {
            AbsenceModel absenceDetail = _service.GetAbsenceDetailByAbsenceId(AbsenceId);
            IEnumerable<SubzzV2.Core.Entities.User> users = _userService.GetAdminListByAbsenceId(AbsenceId);
            Message message = new Message();
            message.ConfirmationNumber = absenceDetail.ConfirmationNumber;
            message.AbsenceId = absenceDetail.AbsenceId;
            message.StartTime = DateTime.ParseExact(Convert.ToString(absenceDetail.StartTime), "HH:mm:ss",
                                CultureInfo.InvariantCulture).ToSubzzTime();
            message.EndTime = DateTime.ParseExact(Convert.ToString(absenceDetail.EndTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture).ToSubzzTime();
            message.StartDate = Convert.ToDateTime(absenceDetail.StartDate).ToString("D");
            message.EndDate = Convert.ToDateTime(absenceDetail.EndDate).ToString("D");
            message.StartTimeSMS = DateTime.ParseExact(Convert.ToString(absenceDetail.StartTime), "HH:mm:ss",
                                CultureInfo.InvariantCulture).ToSubzzTimeForSms();
            message.EndTimeSMS = DateTime.ParseExact(Convert.ToString(absenceDetail.EndTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture).ToSubzzTimeForSms();
            if (message.StartDate == message.EndDate)
            {
                message.DateToDisplayInSMS = Convert.ToDateTime(absenceDetail.StartDate).ToSubzzDateForSMS();
            }
            else
            {
                message.DateToDisplayInSMS = Convert.ToDateTime(absenceDetail.StartDate).ToSubzzShortDateForSMS() + "-" + Convert.ToDateTime(absenceDetail.EndDate).ToSubzzDateForSMS();
            }
            message.EmployeeName = absenceDetail.EmployeeName;
            message.Position = absenceDetail.PositionDescription;
            message.Subject = absenceDetail.SubjectDescription;
            message.Grade = absenceDetail.Grade;
            message.Location = absenceDetail.AbsenceLocation;
            message.School = absenceDetail.OrganizationName;
            message.Notes = absenceDetail.SubstituteNotes;
            message.SubstituteName = absenceDetail.SubstituteName;
            message.AttachedFileName = absenceDetail.AttachedFileName;
            message.FileContentType = absenceDetail.FileContentType;
            message.Duration = absenceDetail.DurationType == 1 ? "Full Day" : absenceDetail.DurationType == 2 ? "First Half" : absenceDetail.DurationType == 3 ? "Second Half" : "Custom";
            message.TemplateId = 15;
            message.Photo = absenceDetail.EmployeeProfilePicUrl;
            foreach (var user in users)
            {
                try
                {
                    message.Password = user.Password;
                    message.UserName = user.FirstName;
                    message.SendTo = user.Email;
                    if (user.IsSubscribedSMS && user.RoleId == 4 && absenceDetail.SubstituteRequired)
                    {
                        var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                        var jobPostedEvent = events.Where(x => x.EventId == 5).First();
                        message.PhoneNumber = user.PhoneNumber;
                        if (jobPostedEvent.EmailAlert)
                        {
                            var sub = _userService.GetSubjectsForNotifications(user.UserId);
                            var subjects = sub.Where(x => x.TeacherSpecialityId == absenceDetail.SpecialityTypeId).FirstOrDefault();
                            if (absenceDetail.OnlySubjectSpecialist && subjects != null ? subjects.SubjectNotification : true &&
                                absenceDetail.OnlyCertified ? user.IsCertified == 1 : true)
                                await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                        }
                        if (jobPostedEvent.TextAlert)
                        {
                            var sub = _userService.GetSubjectsForNotifications(user.UserId);
                            var subjects = sub.Where(x => x.TeacherSpecialityId == absenceDetail.SpecialityTypeId).FirstOrDefault();
                            if (absenceDetail.OnlySubjectSpecialist && subjects != null ? subjects.SubjectNotification : true &&
                                absenceDetail.OnlyCertified ? user.IsCertified == 1 : true)
                                CommunicationContainer.SMSProcessor.Process(message, (MailTemplateEnums)message.TemplateId);
                        }
                            
                    }
                    else if (user.IsSubscribedEmail && user.RoleId != 4)
                    {
                        var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                        var jobPostedEvent = events.Where(x => x.EventId == 7).First();
                        if (jobPostedEvent.EmailAlert)
                        {
                            await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        async Task SendNotificationsOnJobAssignedFromReports(int AbsenceId)
        {
            AbsenceModel absenceDetail = _service.GetAbsenceDetailByAbsenceId(AbsenceId);
            IEnumerable<SubzzV2.Core.Entities.User> users = _userService.GetAdminListByAbsenceId(AbsenceId);
            Message message = new Message();
            message.ConfirmationNumber = absenceDetail.ConfirmationNumber;
            message.AbsenceId = absenceDetail.AbsenceId;
            message.StartTime = DateTime.ParseExact(Convert.ToString(absenceDetail.StartTime), "HH:mm:ss",
                                CultureInfo.InvariantCulture).ToSubzzTime();
            message.EndTime = DateTime.ParseExact(Convert.ToString(absenceDetail.EndTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture).ToSubzzTime();
            message.StartDate = Convert.ToDateTime(absenceDetail.StartDate).ToString("D");
            message.EndDate = Convert.ToDateTime(absenceDetail.EndDate).ToString("D");
            message.StartTimeSMS = DateTime.ParseExact(Convert.ToString(absenceDetail.StartTime), "HH:mm:ss",
                                CultureInfo.InvariantCulture).ToSubzzTimeForSms();
            message.EndTimeSMS = DateTime.ParseExact(Convert.ToString(absenceDetail.EndTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture).ToSubzzTimeForSms();
            if (message.StartDate == message.EndDate)
            {
                message.DateToDisplayInSMS = Convert.ToDateTime(absenceDetail.StartDate).ToSubzzDateForSMS();
            }
            else
            {
                message.DateToDisplayInSMS = Convert.ToDateTime(absenceDetail.StartDate).ToSubzzShortDateForSMS() + "-" + Convert.ToDateTime(absenceDetail.EndDate).ToSubzzDateForSMS();
            }
            message.EmployeeName = absenceDetail.EmployeeName;
            message.Position = absenceDetail.PositionDescription;
            message.Subject = absenceDetail.SubjectDescription;
            message.Grade = absenceDetail.Grade;
            message.Location = absenceDetail.AbsenceLocation;
            message.School = absenceDetail.OrganizationName;
            message.Notes = absenceDetail.SubstituteNotes;
            message.SubstituteName = absenceDetail.SubstituteName;
            message.Reason = absenceDetail.AbsenceReasonDescription;
            message.AttachedFileName = absenceDetail.AttachedFileName;
            message.FileContentType = absenceDetail.FileContentType;
            message.Duration = absenceDetail.DurationType == 1 ? "Full Day" : absenceDetail.DurationType == 2 ? "First Half" : absenceDetail.DurationType == 3 ? "Second Half" : "Custom";
            message.TemplateId = 15;
            foreach (var user in users)
            {
                try
                {
                    message.UserName = user.FirstName;
                    message.SendTo = user.Email;
                    //For Substitutes
                    if (user.RoleId == 4)
                    {
                        message.TemplateId = 7;
                        if (user.IsSubscribedEmail)
                        {
                            var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            var jobPostedEvent = events.Where(x => x.EventId == 5).First();
                            if (jobPostedEvent.EmailAlert)
                            {
                                var sub = _userService.GetSubjectsForNotifications(user.UserId);
                                var subjects = sub.Where(x => x.TeacherSpecialityId == absenceDetail.SpecialityTypeId).FirstOrDefault();
                                if (absenceDetail.OnlySubjectSpecialist && subjects != null ? subjects.SubjectNotification : true &&
                                    absenceDetail.OnlyCertified ? user.IsCertified == 1 : true)
                                    await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                            }    
                        }
                        if (user.IsSubscribedSMS)
                        {
                            var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            var jobPostedEvent = events.Where(x => x.EventId == 5).First();
                            if (jobPostedEvent.TextAlert)
                            {
                                var sub = _userService.GetSubjectsForNotifications(user.UserId);
                                var subjects = sub.Where(x => x.TeacherSpecialityId == absenceDetail.SpecialityTypeId).FirstOrDefault();
                                if (absenceDetail.OnlySubjectSpecialist && subjects != null ? subjects.SubjectNotification : true &&
                                    absenceDetail.OnlyCertified ? user.IsCertified == 1 : true)
                                    CommunicationContainer.SMSProcessor.Process(message, (MailTemplateEnums)message.TemplateId);
                            }
                        }

                    }
                    //For Admins
                    else
                    {
                        message.TemplateId = 8;
                        message.SubstituteName = absenceDetail.SubstituteName;
                        if (user.IsSubscribedEmail)
                        {
                            var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            var jobPostedEvent = events.Where(x => x.EventId == 5).First();
                            if (jobPostedEvent.EmailAlert)
                                await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        async Task SendNotificationsOnJobUpdated(int AbsenceId)
        {
            AbsenceModel absenceDetail = _service.GetAbsenceDetailByAbsenceId(AbsenceId);
            IEnumerable<SubzzV2.Core.Entities.User> users = _userService.GetAdminListByAbsenceId(AbsenceId);
            Message message = new Message();
            message.ConfirmationNumber = absenceDetail.ConfirmationNumber;
            message.AbsenceId = absenceDetail.AbsenceId;
            message.StartTime = DateTime.ParseExact(Convert.ToString(absenceDetail.StartTime), "HH:mm:ss",
                                CultureInfo.InvariantCulture).ToSubzzTime();
            message.EndTime = DateTime.ParseExact(Convert.ToString(absenceDetail.EndTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture).ToSubzzTime();
            message.StartDate = Convert.ToDateTime(absenceDetail.StartDate).ToString("D");
            message.EndDate = Convert.ToDateTime(absenceDetail.EndDate).ToString("D");
            message.StartTimeSMS = DateTime.ParseExact(Convert.ToString(absenceDetail.StartTime), "HH:mm:ss",
                                CultureInfo.InvariantCulture).ToSubzzTimeForSms();
            message.EndTimeSMS = DateTime.ParseExact(Convert.ToString(absenceDetail.EndTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture).ToSubzzTimeForSms();
            if (message.StartDate == message.EndDate)
            {
                message.DateToDisplayInSMS = Convert.ToDateTime(absenceDetail.StartDate).ToSubzzDateForSMS();
            }
            else
            {
                message.DateToDisplayInSMS = Convert.ToDateTime(absenceDetail.StartDate).ToSubzzShortDateForSMS() + "-" + Convert.ToDateTime(absenceDetail.EndDate).ToSubzzDateForSMS();
            }
            message.EmployeeName = absenceDetail.EmployeeName;
            message.Position = absenceDetail.PositionDescription;
            message.Subject = absenceDetail.SubjectDescription;
            message.Grade = absenceDetail.Grade;
            message.Location = absenceDetail.AbsenceLocation;
            message.School = absenceDetail.OrganizationName;
            message.Notes = absenceDetail.SubstituteNotes;
            message.SubstituteName = absenceDetail.SubstituteName;
            message.Photo = absenceDetail.EmployeeProfilePicUrl;
            message.AttachedFileName = absenceDetail.AttachedFileName;
            message.FileContentType = absenceDetail.FileContentType;
            message.Duration = absenceDetail.DurationType == 1 ? "Full Day" : absenceDetail.DurationType == 2 ? "First Half" : absenceDetail.DurationType == 3 ? "Second Half" : "Custom";

            foreach (var user in users)
            {
                try
                {
                    message.UserName = user.FirstName;
                    message.SendTo = user.Email;
                    //For Substitutes
                    if (user.RoleId == 4 && absenceDetail.SubstituteRequired)
                    {
                        message.TemplateId = 17;
                        if (user.IsSubscribedEmail)
                        {
                            var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            var jobPostedEvent = events.Where(x => x.EventId == 4).First();
                            if (jobPostedEvent.EmailAlert)
                            {
                                var sub = _userService.GetSubjectsForNotifications(user.UserId);
                                var subjects = sub.Where(x => x.TeacherSpecialityId == absenceDetail.SpecialityTypeId).FirstOrDefault();
                                if (absenceDetail.OnlySubjectSpecialist && subjects != null ? subjects.SubjectNotification : true &&
                                    absenceDetail.OnlyCertified ? user.IsCertified == 1 : true)
                                    await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                            }
                        }
                        if (user.IsSubscribedSMS)
                        {
                            var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            var jobPostedEvent = events.Where(x => x.EventId == 5).First();
                            if (jobPostedEvent.TextAlert)
                            {
                                message.PhoneNumber = user.PhoneNumber;
                                var sub = _userService.GetSubjectsForNotifications(user.UserId);
                                var subjects = sub.Where(x => x.TeacherSpecialityId == absenceDetail.SpecialityTypeId).FirstOrDefault();
                                if (absenceDetail.OnlySubjectSpecialist && subjects != null ? subjects.SubjectNotification : true &&
                                    absenceDetail.OnlyCertified ? user.IsCertified == 1 : true)
                                    CommunicationContainer.SMSProcessor.Process(message, (MailTemplateEnums)message.TemplateId);
                            }
                                
                        }

                    }
                    //For Admins And Employee
                    else if (user.RoleId != 4)
                    {
                        message.TemplateId = 18;
                        message.SubstituteName = absenceDetail.SubstituteName;
                        if (user.IsSubscribedEmail)
                        {
                            var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            var jobPostedEvent = events.Where(x => x.EventId == 4).First();
                            if (jobPostedEvent.EmailAlert)
                                await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        async Task SendNotificationsOnJobReleased(int AbsenceId)
        {
            AbsenceModel absenceDetail = _service.GetAbsenceDetailByAbsenceId(AbsenceId);
            IEnumerable<SubzzV2.Core.Entities.User> users = _userService.GetAdminListByAbsenceId(AbsenceId);
            Message message = new Message();
            message.ConfirmationNumber = absenceDetail.ConfirmationNumber;
            message.AbsenceId = absenceDetail.AbsenceId;
            message.StartTime = DateTime.ParseExact(Convert.ToString(absenceDetail.StartTime), "HH:mm:ss",
                                CultureInfo.InvariantCulture).ToSubzzTime();
            message.EndTime = DateTime.ParseExact(Convert.ToString(absenceDetail.EndTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture).ToSubzzTime();
            message.StartDate = Convert.ToDateTime(absenceDetail.StartDate).ToString("D");
            message.EndDate = Convert.ToDateTime(absenceDetail.EndDate).ToString("D");

            message.StartTimeSMS = DateTime.ParseExact(Convert.ToString(absenceDetail.StartTime), "HH:mm:ss",
                                CultureInfo.InvariantCulture).ToSubzzTimeForSms();
            message.EndTimeSMS = DateTime.ParseExact(Convert.ToString(absenceDetail.EndTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture).ToSubzzTimeForSms();
            if (message.StartDate == message.EndDate)
            {
                message.DateToDisplayInSMS = Convert.ToDateTime(absenceDetail.StartDate).ToSubzzDateForSMS();
            }
            else
            {
                message.DateToDisplayInSMS = Convert.ToDateTime(absenceDetail.StartDate).ToSubzzShortDateForSMS() + "-" + Convert.ToDateTime(absenceDetail.EndDate).ToSubzzDateForSMS();
            }

            message.EmployeeName = absenceDetail.EmployeeName;
            message.Position = absenceDetail.PositionDescription;
            message.Subject = absenceDetail.SubjectDescription;
            message.Grade = absenceDetail.Grade;
            message.Location = absenceDetail.AbsenceLocation;
            message.School = absenceDetail.OrganizationName;
            message.Notes = absenceDetail.SubstituteNotes;
            message.SubstituteName = absenceDetail.SubstituteName;
            message.Reason = absenceDetail.AbsenceReasonDescription;
            message.Photo = absenceDetail.EmployeeProfilePicUrl;
            message.AttachedFileName = absenceDetail.AttachedFileName;
            message.FileContentType = absenceDetail.FileContentType;
            message.Duration = absenceDetail.DurationType == 1 ? "Full Day" : absenceDetail.DurationType == 2 ? "First Half" : absenceDetail.DurationType == 3 ? "Second Half" : "Custom";
            message.SubstituteName = absenceDetail.SubstituteName;
            foreach (var user in users)
            {
                try
                {
                    message.UserName = user.FirstName;
                    message.SendTo = user.Email;
                    //For Substitutes
                    if (user.RoleId == 4)
                    {
                        message.TemplateId = 21;
                        if (user.IsSubscribedEmail)
                        {
                            var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            var jobPostedEvent = events.Where(x => x.EventId == 6).First();
                            if (jobPostedEvent.EmailAlert)
                            {
                                var sub = _userService.GetSubjectsForNotifications(user.UserId);
                                var subjects = sub.Where(x => x.TeacherSpecialityId == absenceDetail.SpecialityTypeId).FirstOrDefault();
                                if (absenceDetail.OnlySubjectSpecialist && subjects != null ? subjects.SubjectNotification : true &&
                                    absenceDetail.OnlyCertified ? user.IsCertified == 1 : true)
                                    await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                            }
                        }
                        if (user.IsSubscribedSMS)
                        {
                            var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            var jobPostedEvent = events.Where(x => x.EventId == 5).First();
                            if (jobPostedEvent.TextAlert)
                            {
                                message.PhoneNumber = user.PhoneNumber;
                                var sub = _userService.GetSubjectsForNotifications(user.UserId);
                                var subjects = sub.Where(x => x.TeacherSpecialityId == absenceDetail.SpecialityTypeId).FirstOrDefault();
                                if (absenceDetail.OnlySubjectSpecialist && subjects != null ? subjects.SubjectNotification : true &&
                                    absenceDetail.OnlyCertified ? user.IsCertified == 1 : true)
                                    CommunicationContainer.SMSProcessor.Process(message, (MailTemplateEnums)message.TemplateId);
                            }
                        }

                    }
                    //For Employee
                    else if (user.RoleId == 3)
                    {
                        message.TemplateId = 20;
                        if (user.IsSubscribedEmail)
                        {
                            var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            var jobPostedEvent = events.Where(x => x.EventId == 6).First();
                            if (jobPostedEvent.EmailAlert)
                                await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                        }

                    }
                    //For Admins
                    else
                    {
                        message.TemplateId = 22;
                        if (user.IsSubscribedEmail)
                        {
                            var events = _userService.GetSubstituteNotificationEvents(user.UserId);
                            var jobPostedEvent = events.Where(x => x.EventId == 6).First();
                            if (jobPostedEvent.EmailAlert)
                                await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        void SendEmailToEmployeeOnJobCreation(int AbsenceId)
        {
            AbsenceModel absenceDetail = _service.GetAbsenceDetailByAbsenceId(AbsenceId);
            IEnumerable<SubzzV2.Core.Entities.User> users = _userService.GetAdminListByAbsenceId(AbsenceId);
            Message message = new Message();
            message.ConfirmationNumber = absenceDetail.ConfirmationNumber;
            message.AbsenceId = absenceDetail.AbsenceId;
            message.StartTime = DateTime.ParseExact(Convert.ToString(absenceDetail.StartTime), "HH:mm:ss",
                                CultureInfo.InvariantCulture).ToSubzzTime();
            message.EndTime = DateTime.ParseExact(Convert.ToString(absenceDetail.EndTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture).ToSubzzTime();
            message.StartDate = Convert.ToDateTime(absenceDetail.StartDate).ToString("D");
            message.EndDate = Convert.ToDateTime(absenceDetail.EndDate).ToString("D");
            message.StartTimeSMS = DateTime.ParseExact(Convert.ToString(absenceDetail.StartTime), "HH:mm:ss",
                                CultureInfo.InvariantCulture).ToSubzzTimeForSms();
            message.EndTimeSMS = DateTime.ParseExact(Convert.ToString(absenceDetail.EndTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture).ToSubzzTimeForSms();
            if (message.StartDate == message.EndDate)
            {
                message.DateToDisplayInSMS = Convert.ToDateTime(absenceDetail.StartDate).ToSubzzDateForSMS();
            }
            else
            {
                message.DateToDisplayInSMS = Convert.ToDateTime(absenceDetail.StartDate).ToSubzzShortDateForSMS() + "-" + Convert.ToDateTime(absenceDetail.EndDate).ToSubzzDateForSMS();
            }
            message.EmployeeName = absenceDetail.EmployeeName;
            message.Position = absenceDetail.PositionDescription;
            message.Subject = absenceDetail.SubjectDescription;
            message.Grade = absenceDetail.Grade;
            message.Location = absenceDetail.AbsenceLocation;
            message.Notes = absenceDetail.SubstituteNotes;
            message.SubstituteName = absenceDetail.SubstituteName;
            message.School = absenceDetail.OrganizationName;
            message.Photo = absenceDetail.EmployeeProfilePicUrl;
            message.AttachedFileName = absenceDetail.AttachedFileName;
            message.FileContentType = absenceDetail.FileContentType;
            message.Duration = absenceDetail.DurationType == 1 ? "Full Day" : absenceDetail.DurationType == 2 ? "First Half" : absenceDetail.DurationType == 3 ? "Second Half" : "Custom";
            var employeeDetail = _userService.GetUserDetail(absenceDetail.EmployeeId);
            message.TemplateId = 10;
            message.Photo = absenceDetail.EmployeeProfilePicUrl;
            try
            {
                message.Password = employeeDetail.Password;
                message.UserName = employeeDetail.FirstName;
                message.SendTo = employeeDetail.Email;
                if (employeeDetail.IsSubscribedEmail)
                {
                    var events = _userService.GetSubstituteNotificationEvents(employeeDetail.UserId);
                    var jobPostedEvent = events.Where(x => x.EventId == 2).First();
                    if (jobPostedEvent.EmailAlert)
                         CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                }

            }
            catch (Exception ex)
            {
            }
        }
    }
}