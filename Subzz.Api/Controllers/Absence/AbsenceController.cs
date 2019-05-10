﻿using System;
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
            var result = _service.GetAbsenceDetailByAbsenceId(id);
            return Ok(result);
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
                var absenceCreation = _service.CreateAbsence(model);
                if (absenceCreation > 0)
                {
                    // Audit Log
                    var audit = new AuditLog
                    {
                        UserId = CurrentUser.Id,
                        EntityId = absenceCreation.ToString(),
                        EntityType = AuditLogs.EntityType.Absence,
                        ActionType = AuditLogs.ActionType.Create,
                        PostValue = Serializer.Serialize(model),
                        DistrictId = CurrentUser.DistrictId,
                        OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                    };
                    _audit.InsertAuditLog(audit);

                    model.AbsenceId = absenceCreation;
                    DataTable SingleDayAbsences = CustomClass.InsertAbsenceBasicDetailAsSingleDay(absenceCreation,
                        model.StartDate, model.EndDate, model.StartTime, model.EndTime, model.SubstituteId.Length > 10 ? "-1" : model.SubstituteId, model.Status);
                    Task taskForStoreAbsenceAsSingleDay = _service.SaveAsSingleDayAbsence(SingleDayAbsences);
                    if (model.AbsenceScope == 3)
                    {
                        IEnumerable<SubzzV2.Core.Entities.User> FavSubstitutes =
                            _userService.GetFavoriteSubstitutes(model.EmployeeId);
                        await _service.CreatePreferredAbsenceHistory(FavSubstitutes, model);
                    }
                    else
                    {
                        if (model.IsApprovalRequired)
                        {
                            //Task.Run(() => SendNotifications(model));
                        }

                        return Json("success");
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(ex.InnerException);
            }

            return Json("error");
        }

        [Route("getAbsences/{StartDate}/{EndDate}/{UserId}")]
        [HttpGet]
        public IEnumerable<AbsenceModel> GetAbsences(DateTime StartDate, DateTime EndDate, string UserId)
        {
           return _service.GetAbsences(StartDate, EndDate, UserId);
        }

        [Route("getAbsencesScheduleEmployee/{StartDate}/{EndDate}/{UserId}")]
        [HttpGet]
        public IEnumerable<EmployeeSchedule> GetAbsencesScheduleEmployee(DateTime StartDate, DateTime EndDate, string UserId)
        {
            return _service.GetAbsencesScheduleEmployee(StartDate, EndDate, UserId);
        }

        [Route("updateAbseceStatus/{AbsenceId}/{StatusId}/{UpdateStatusDate}/{UserId}")]
        [HttpGet]
        public ActionResult UpdateAbsenceStatus(int AbsenceId, int statusId, string UpdateStatusDate, string UserId)
        {
            int RowsEffected = _service.UpdateAbsenceStatus(AbsenceId, statusId, Convert.ToDateTime(UpdateStatusDate), UserId);
            if (statusId == 1)
            {
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = AbsenceId.ToString(),
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
                    EntityId = AbsenceId.ToString(),
                    EntityType = AuditLogs.EntityType.Absence,
                    ActionType = AuditLogs.ActionType.Cancelled,
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);
            }
            return Json("success");
        }

        [Route("getfile")]
        [HttpPost]
        public IActionResult GetFile([FromBody]AbsenceModel model)
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

        [Route("updateAbsence")]
        [HttpPatch]
        public ActionResult UpdateAbsence([FromBody]AbsenceModel model)
        {
            model.UpdatedById = base.CurrentUser.Id;
            int RowsEffected = _service.UpdateAbsence(model);
            if (RowsEffected > 0)
            {
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = model.AbsenceId.ToString(),
                    EntityType = AuditLogs.EntityType.Absence,
                    ActionType = AuditLogs.ActionType.Update,
                    PreValue = Serializer.Serialize(_service.GetAbsenceDetailByAbsenceId(model.AbsenceId)),
                    PostValue = Serializer.Serialize(model),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

                DataTable SingleDayAbsences = CustomClass.InsertAbsenceBasicDetailAsSingleDay(model.AbsenceId, model.StartDate, model.EndDate, model.StartTime, model.EndTime, model.SubstituteId.Length > 10 ? "-1" : model.SubstituteId, model.Status);
                Task taskForStoreAbsenceAsSingleDay = _service.SaveAsSingleDayAbsence(SingleDayAbsences);
                return Json("success");
            }
            else
            {
                return Json("error");
            }
        }

        [Route("updateAbseceStatusAndSub/{AbsenceId}/{StatusId}/{UpdateStatusDate}/{UserId}/{SubstituteId}/{SubstituteRequired}")]
        [HttpGet]
        public ActionResult UpdateAbseceStatusAndSub(int AbsenceId, int statusId, string UpdateStatusDate, string UserId, string SubstituteId, bool SubstituteRequired)
        {
            int RowsEffected = _service.UpdateAbsenceStatusAndSub(AbsenceId, statusId, Convert.ToDateTime(UpdateStatusDate), UserId, SubstituteId, SubstituteRequired);
            if (RowsEffected > 0)
            {
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = AbsenceId.ToString(),
                    EntityType = AuditLogs.EntityType.Absence,
                    ActionType = AuditLogs.ActionType.Assigned,
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);

                return Json("success");
            }
            return Json("error");
        }

        //For Dashboard Chart
        [Route("summary")]
        [HttpGet]
        public IActionResult GetSummary()
        {
            var year = DateTime.Now.Year;
            var userId = base.CurrentUser.Id;
            var Summary = _service.GetAbsenceSummary(userId, year);
            return Ok(Summary);
        }

        async Task SendNotifications(AbsenceModel absenceModel)
        {
            Subzz.Integration.Core.Domain.Message message = new Integration.Core.Domain.Message();
            //SubstituteId Contains All Substitute Ids in case Of Request specific Substitute.
            var DataForEmails = _userService.GetUsersForSendingAbsenceNotificationOnEntireSub(absenceModel.DistrictId, absenceModel.OrganizationId, absenceModel.AbsenceId, absenceModel.SubstituteId);
            message.AbsenceId = absenceModel.AbsenceId;
            message.StartTime = DateTime.ParseExact(Convert.ToString(absenceModel.StartTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture).ToSubzzTime();
            message.EndTime = DateTime.ParseExact(Convert.ToString(absenceModel.EndTime), "HH:mm:ss",
                                        CultureInfo.InvariantCulture).ToSubzzTime();
            message.StartDate = Convert.ToDateTime(absenceModel.StartDate).ToString("D");
            message.EndDate = Convert.ToDateTime(absenceModel.EndDate).ToString("D");
            message.EmployeeName = DataForEmails.EmployeeName;
            message.Position = DataForEmails.PositionDescription;
            message.Subject = DataForEmails.SubjectDescription;
            message.Grade = DataForEmails.Grade;
            message.Location = DataForEmails.AbsenceLocation;
            message.Notes = DataForEmails.SubstituteNotes;
            message.Duration = DataForEmails.DurationType == 1 ? "Full Day" : DataForEmails.DurationType == 2 ? "First Half" : DataForEmails.DurationType == 3 ? "Second Half" : "Custom";
            //Entire Sustitute Pool or Request Specifc Sub
            if (absenceModel.AbsenceScope == 4 || absenceModel.AbsenceScope == 1)
            {
                foreach (var User in DataForEmails.Users)
                {
                    try
                    {
                        message.Password = User.Password;
                        message.UserName = User.FirstName;
                        message.SendTo = User.Email;
                        //For Substitutes
                        if (User.RoleId == 4)
                        {
                            message.TemplateId = 1;
                        }
                        //For Admins
                        else
                        {
                            message.TemplateId = 2;
                        }
                        await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            //Direct Assign
            else if (absenceModel.AbsenceScope == 2)
            {
                foreach (var User in DataForEmails.Users)
                {
                    try
                    {
                        message.UserName = User.FirstName;
                        message.SendTo = User.Email;
                        //For Substitutes
                        if (User.RoleId == 4)
                        {
                            message.TemplateId = 7;
                            
                        }
                        //For Admins
                        else
                        {
                            message.TemplateId = 8;
                            message.SubstituteName = DataForEmails.SubstituteName;
                        }
                        await CommunicationContainer.EmailProcessor.ProcessAsync(message, (MailTemplateEnums)message.TemplateId);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            
        }

    }
}