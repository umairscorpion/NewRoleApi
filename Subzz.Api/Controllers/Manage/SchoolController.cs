using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SubzzManage.Business.Manage.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Subzz.Api.Controllers.Base;
using SubzzV2.Core.Models;
using Subzz.Business.Services.Users.Interface;
using SubzzV2.Core.Enum;
using Subzz.Integration.Core.Helper;
using ExcelDataReader;
using System.Data;

namespace Subzz.Api.Controllers.Manage
{
    [Produces("application/json")]
    [Route("api/School")]
    public class SchoolController : BaseApiController
    {
        private readonly ISchoolService _service;
        private readonly IAuditingService _audit;
        public SchoolController(ISchoolService service, IAuditingService audit)
        {
            _service = service;
            _audit = audit;
        }

        [Route("insertSchool")]
        [HttpPost]
        public OrganizationModel InsertSchool([FromBody]OrganizationModel model)
        {
            try
            {
                var school = _service.InsertSchool(model);
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = model.SchoolId.ToString(),
                    EntityType = AuditLogs.EntityType.School,
                    ActionType = AuditLogs.ActionType.CreatedSchool,
                    PostValue = Serializer.Serialize(model),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);
                return school;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("updateSchool")]
        [HttpPatch]
        public OrganizationModel UpdateSchool([FromBody]OrganizationModel model)
        {
            try
            {
                 var SchoolModel = _service.UpdateSchool(model);
                // Audit Log
                var audit = new AuditLog
                {
                    UserId = CurrentUser.Id,
                    EntityId = model.SchoolId.ToString(),
                    EntityType = AuditLogs.EntityType.School,
                    ActionType = AuditLogs.ActionType.UpdatedSchool,
                    PostValue = Serializer.Serialize(model),
                    DistrictId = CurrentUser.DistrictId,
                    OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
                };
                _audit.InsertAuditLog(audit);
                return SchoolModel;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getSchools")]
        [HttpGet]
        public IEnumerable<OrganizationModel> GetSchools()
        {
            try
            {
                var schools = _service.GetSchools();
                return schools;
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
        public int Delete(string id)
        {
            var DeleteSchool = _service.DeleteSchool(id);
            // Audit Log
            var audit = new AuditLog
            {
                UserId = CurrentUser.Id,
                EntityId = id.ToString(),
                EntityType = AuditLogs.EntityType.School,
                ActionType = AuditLogs.ActionType.DeletedSchool,
                DistrictId = CurrentUser.DistrictId,
                OrganizationId = CurrentUser.OrganizationId == "-1" ? null : CurrentUser.OrganizationId
            };
            _audit.InsertAuditLog(audit);
            return DeleteSchool;
        }

        [Route("getSchoolById/{id}")]
        [HttpGet]
        public IEnumerable<OrganizationModel> GetSchoolById(string id)
        {
            try
            {
                var SchoolModel = _service.GetSchool(id);
                return SchoolModel;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getOrganizationsByDistrictId/{id}")]
        [HttpGet]
        public IEnumerable<OrganizationModel> GetOrganizationsByDistrictId(int id)
        {
            try
            {
                return _service.GetOrganizationsByDistrictId(id);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getOrganizationTimeByOrganizationId/{OrganizationId}")]
        [HttpGet]
        public LocationTime GetOrganizationTimeByOrganizationId(string OrganizationId)
        {
            try
            {
                return _service.GetOrganizationTimeByOrganizationId(OrganizationId);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getAbsenceScopes")]
        [HttpPost]
        public IActionResult GetAbsenceScopes([FromBody]OrganizationModel organizationModel)
        {
            try
            {
                var scopes = _service.GetAbsenceScopes(organizationModel);
                return Ok(scopes);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("updateAbsenceScopes")]
        [HttpPost]
        public IActionResult UpdateAbsenceScopes([FromBody]List<AbsenceScope> absenceScopes)
        {
            try
            {
                foreach (AbsenceScope ab in absenceScopes)
                {
                _service.UpdateAbsenceScope(ab);
                }
                return Ok();
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("uploadExcel/{DistrictId}")]
        [HttpPost]
        public IActionResult Upload(int DistrictId)
        {
            var file = Request.Form.Files[0];
            var stream = file.OpenReadStream();
            IExcelDataReader reader = null;

            if (file.FileName.EndsWith(".xls"))
            {
                reader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else if (file.FileName.EndsWith(".xlsx"))
            {
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            else
            {
                return Ok("NotSupported");
            }

            DataSet excelRecords = reader.AsDataSet();
            reader.Close();

            var finalRecords = excelRecords.Tables[0];
            if(finalRecords.Rows.Count <= 1)
            {
                return Ok("Empty");
            }
            for (int i = 1; i < finalRecords.Rows.Count; i++)
            {
                //UserInfo objUser = new UserInfo();
                //objUser.UserName = finalRecords.Rows[i][0].ToString();
                //objUser.EmailId = finalRecords.Rows[i][1].ToString();
                //objUser.Gender = finalRecords.Rows[i][2].ToString();
                //objUser.Address = finalRecords.Rows[i][3].ToString();
                //objUser.MobileNo = finalRecords.Rows[i][4].ToString();
                //objUser.PinCode = finalRecords.Rows[i][5].ToString();

                OrganizationModel model = new OrganizationModel();
                model.SchoolName = finalRecords.Rows[i][0].ToString();
                model.SchoolAddress = finalRecords.Rows[i][1].ToString();
                model.SchoolCity = finalRecords.Rows[i][2].ToString();
                model.SchoolZipCode =  int.Parse(finalRecords.Rows[i][3].ToString());
                model.SchoolTimeZone = int.Parse(finalRecords.Rows[i][4].ToString());
                model.SchoolStartTime = DateTime.Parse(finalRecords.Rows[i][5].ToString()).TimeOfDay;
                model.School1stHalfEnd = DateTime.Parse(finalRecords.Rows[i][6].ToString()).TimeOfDay;
                model.School2ndHalfStart = DateTime.Parse(finalRecords.Rows[i][7].ToString()).TimeOfDay;
                model.SchoolEndTime = DateTime.Parse(finalRecords.Rows[i][8].ToString()).TimeOfDay;
                model.SchoolPhone = "+" + finalRecords.Rows[i][9].ToString();
                model.SchoolDistrictId = DistrictId;
                var school = _service.InsertSchool(model);
            }

            return Ok("Imported");
        }
    }
}