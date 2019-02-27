using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public AbsenceController(IAbsenceService service, IHostingEnvironment hostingEnvironment, IUserService userService)
        {
            _service = service;
            _userService = userService;
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

        [Route("uploadFile")]
        [HttpPost]
        public IActionResult UploadFile()
        {
            try
            {
                var file = Request.Form.Files[0];
                string folderName = "Upload";
                string webRootPath = _hostingEnvironment.WebRootPath;
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
                    
                    model.AbsenceId = absenceCreation;
                    DataTable SingleDayAbsences = CustomClass.InsertAbsenceBasicDetailAsSingleDay(absenceCreation, Convert.ToDateTime(model.StartDate), Convert.ToDateTime(model.EndDate), model.StartTime, model.EndTime);
                    Task taskForStoreAbsenceAsSingleDay = _service.SaveAsSingleDayAbsence(SingleDayAbsences);
                    if (model.AbsenceScope == 3)
                    {
                        IEnumerable<SubzzV2.Core.Entities.User> FavSubstitutes = _userService.GetFavoriteSubstitutes(model.EmployeeId);
                        await _service.CreatePreferredAbsenceHistory(FavSubstitutes, model);
                    }
                    else { await SendNotifications(model); }
                    return Json("success");
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
            if (RowsEffected > 0)
                return Json("success");
            return Json("error");
        }

        [Route("getfile")]
        [HttpPost]
        public IActionResult GetFile([FromBody] AbsenceModel model)
        {
            string folderName = "Upload";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string filePath = Path.Combine(webRootPath, folderName);
            byte[] bytes = System.IO.File.ReadAllBytes(Path.Combine(filePath, model.AttachedFileName));
            return File(bytes, model.FileContentType);
        }

        async Task SendNotifications(AbsenceModel absenceModel)
        {
            Subzz.Integration.Core.Domain.Message message = new Integration.Core.Domain.Message();
            //SubstituteId Contains All Substitute Ids in case Of Request specific Substitute.
            var DataForEmails = _userService.GetUsersForSendingAbsenceNotificationOnEntireSub(absenceModel.DistrictId, absenceModel.OrganizationId, absenceModel.AbsenceId, absenceModel.SubstituteId);
            message.AbsenceId = absenceModel.AbsenceId;
            message.StartTime = absenceModel.StartTime.ToSubzzTime();
            message.EndTime = absenceModel.EndTime.ToSubzzTime();
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