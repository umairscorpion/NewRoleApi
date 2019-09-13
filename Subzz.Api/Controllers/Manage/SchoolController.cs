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

        [Route("getTemporarySchools")]
        [HttpGet]
        public IEnumerable<OrganizationModel> GetTemporarySchools()
        {
            try
            {
                var schools = _service.GetTemporarySchools();
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

        [Route("deleteTemporarySchools")]
        [HttpGet]
        public IActionResult DeleteTemporarySchools()
        {
            try
            {
                var DistrictId = 0;
                var DeleteTemporarySchools = _service.DeleteTemporarySchools(DistrictId);
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

        [Route("verifySchoolsData/{DistrictId}")]
        [HttpPost]
        public IActionResult VerifySchoolsData(int DistrictId)
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
            if (finalRecords.Rows.Count <= 1)
            {
                return Ok("Empty");
            }
            var DeleteTemporarySchools = _service.DeleteTemporarySchools(DistrictId);
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
                var Status = "";
                if (string.IsNullOrEmpty(finalRecords.Rows[i][0].ToString()))
                {
                    Status = "Please fill School Name in column 1. ";
                    model.SchoolName = null;
                }
                else
                {
                    model.SchoolName = finalRecords.Rows[i][0].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][1].ToString()))
                {
                    Status = Status + "Please fill School Address in column 2. ";
                    model.SchoolAddress = null;
                }
                else
                {
                    model.SchoolAddress = finalRecords.Rows[i][1].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][2].ToString()))
                {
                    Status = Status + "Please fill Country name in column 3. ";
                    model.SchoolAddress = null;
                }
                else
                {
                    model.CounrtyCode = finalRecords.Rows[i][2].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][3].ToString()))
                {
                    Status = Status + "Please fill State in column 4. ";
                    model.StateName = null;
                }
                else
                {
                    model.StateName = finalRecords.Rows[i][3].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][4].ToString()))
                {
                    Status = Status + "Please fill city in column 5. ";
                    model.SchoolCity = null;
                }
                else
                {
                    model.SchoolCity = finalRecords.Rows[i][4].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][5].ToString()))
                {
                    Status = Status + "Please fill zip code in column 6. ";
                    model.SchoolZipCode = 0;
                }
                else
                {

                    model.SchoolZipCode = int.Parse(finalRecords.Rows[i][5].ToString());
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][6].ToString()))
                {
                    Status = Status + "Please fill Start time in column 7. ";
                }
                else
                {
                    model.SchoolStartTime = DateTime.Parse(finalRecords.Rows[i][6].ToString()).TimeOfDay;
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][7].ToString()))
                {
                    Status = Status + "Please fill First half time in column 8. ";
                }
                else
                {
                    model.School1stHalfEnd = DateTime.Parse(finalRecords.Rows[i][7].ToString()).TimeOfDay;
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][8].ToString()))
                {
                    Status = Status + "Please fill second half time in column 9.";
                }
                else
                {
                    model.School2ndHalfStart = DateTime.Parse(finalRecords.Rows[i][8].ToString()).TimeOfDay;
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][9].ToString()))
                {
                    Status = Status + "Please fill end time in column 10. ";
                }
                else
                {
                    model.SchoolEndTime = DateTime.Parse(finalRecords.Rows[i][9].ToString()).TimeOfDay;
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][10].ToString()))
                {
                    Status = Status + "Please fill phone in column 11. ";
                }
                else
                {
                    model.SchoolPhone = "+" + finalRecords.Rows[i][10].ToString();
                }
                model.SchoolDistrictId = DistrictId;
                var tempSchools = _service.InsertSchoolTemporary(model, DistrictId, Status);
                //var school = _service.InsertSchool(model);
            }

            return Ok("Imported");
        }

        [Route("importSchools/{DistrictId}")]
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
            var DeleteTemporarySchools = _service.DeleteTemporarySchools(DistrictId);
            for (int i = 1; i < finalRecords.Rows.Count; i++)
            {
                OrganizationModel model = new OrganizationModel();
                var Status = "";
                if(string.IsNullOrEmpty(finalRecords.Rows[i][0].ToString()))
                {
                    Status = "Please fill School Name in column 1 at row " + i;
                    return Ok(Status);
                }
                else
                {
                    model.SchoolName = finalRecords.Rows[i][0].ToString();
                    var schools = _service.GetSchools();
                    var schoolName = schools.Where(x => x.SchoolName == model.SchoolName).FirstOrDefault();
                    if (schoolName != null)
                        return Ok("School Name already exists in database. Please enter another school name in column 1 at row " + i);
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][1].ToString()))
                {
                    Status = Status + "Please fill School Address in column 2 at row " + i;
                    return Ok(Status);
                }
                else
                {
                    model.SchoolAddress = finalRecords.Rows[i][1].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][2].ToString()))
                {
                    Status = Status + "Please fill Country name in column 3at row " + i;
                    return Ok(Status);
                }
                else
                {
                    model.CounrtyCode = finalRecords.Rows[i][2].ToString();
                    var countries = _service.GetCountries();
                    var cId = countries.Where(x => x.CountryName == model.CounrtyCode).FirstOrDefault();
                    if (cId == null)
                        return Ok("Country Name Does not exists in database. Please enter another country in column 3 at row " + i);
                    model.CountryId = cId.CountryId;
                    
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][3].ToString()))
                {
                    Status = Status + "Please fill State in column 4 at row " + i;
                    return Ok(Status);
                }
                else
                {
                    model.StateName = finalRecords.Rows[i][3].ToString();
                    var states = _service.GetStateByCountryId(model.CountryId);
                    var sId = states.Where(x => x.StateName == model.StateName).FirstOrDefault();
                    if (sId == null)
                        return Ok("State Name Does not exists in this country. Please enter another state in column 4 at row " + i);
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][4].ToString()))
                {
                    Status = Status + "Please fill city in column 5 at row " + i;
                    return Ok(Status);
                }
                else
                {
                    model.SchoolCity = finalRecords.Rows[i][4].ToString();
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][5] .ToString()))
                {
                    Status = Status + "Please fill zip code in column 6 at row " + i;
                    return Ok(Status);
                }
                else
                {
                    
                    model.SchoolZipCode = int.Parse(finalRecords.Rows[i][5].ToString());
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][6].ToString()))
                {
                    Status = Status + "Please fill Start time in column 7 at row " + i;
                    return Ok(Status);
                }
                else
                {
                    model.SchoolStartTime = DateTime.Parse(finalRecords.Rows[i][6].ToString()).TimeOfDay;
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][7].ToString()))
                {
                    Status = Status + "Please fill First half time in column 8 at row " + i;
                    return Ok(Status);
                }
                else
                {
                    model.School1stHalfEnd = DateTime.Parse(finalRecords.Rows[i][7].ToString()).TimeOfDay;
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][8].ToString()))
                {
                    Status = Status + "Please fill second half time in column 9 at row " + i;
                    return Ok(Status);
                }
                else
                {
                    model.School2ndHalfStart = DateTime.Parse(finalRecords.Rows[i][8].ToString()).TimeOfDay;
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][9].ToString()))
                {
                    Status = Status + "Please fill end time in column 10 at row " + i;
                    return Ok(Status);
                }
                else
                {
                    model.SchoolEndTime = DateTime.Parse(finalRecords.Rows[i][9].ToString()).TimeOfDay;
                }

                if (string.IsNullOrEmpty(finalRecords.Rows[i][10].ToString()))
                {
                    Status = Status + "Please fill phone in column 11 at row " + i;
                    return Ok(Status);
                }
                else
                {
                    model.SchoolPhone = "+" + finalRecords.Rows[i][10].ToString();
                    var phone = _service.GetSchools();
                    var phoneNumber = phone.Where(x => x.SchoolPhone == model.SchoolPhone).FirstOrDefault();
                    if (phoneNumber != null)
                        return Ok("Phone Number already exists in database. Please enter another phone number in column 11 at row" + i);
                }
                model.SchoolDistrictId = DistrictId;
                //var school = _service.InsertSchool(model);
            }

            for (int i = 1; i < finalRecords.Rows.Count; i++)
            {
                OrganizationModel model = new OrganizationModel();
                model.SchoolName = finalRecords.Rows[i][0].ToString();
                model.SchoolAddress = finalRecords.Rows[i][1].ToString();
                var countryName = finalRecords.Rows[i][2].ToString();
                var stateName = finalRecords.Rows[i][3].ToString();
                var countries = _service.GetCountries();
                var cId = countries.Where(x => x.CountryName == countryName).First();
                model.CountryId = cId.CountryId;
                var states = _service.GetStateByCountryId(model.CountryId);
                var sId = states.Where(x => x.StateName == stateName).First();
                model.SchoolStateId = sId.StateId;
                model.SchoolCity = finalRecords.Rows[i][4].ToString();
                model.SchoolZipCode = int.Parse(finalRecords.Rows[i][5].ToString());
                model.SchoolStartTime = DateTime.Parse(finalRecords.Rows[i][6].ToString()).TimeOfDay;
                model.School1stHalfEnd = DateTime.Parse(finalRecords.Rows[i][7].ToString()).TimeOfDay;
                model.School2ndHalfStart = DateTime.Parse(finalRecords.Rows[i][8].ToString()).TimeOfDay;
                model.SchoolEndTime = DateTime.Parse(finalRecords.Rows[i][9].ToString()).TimeOfDay;
                model.SchoolPhone = "+" + finalRecords.Rows[i][10].ToString();
                model.SchoolDistrictId = DistrictId;
                var school = _service.InsertSchool(model);
            }

            return Ok("Imported");
        }
    }
}