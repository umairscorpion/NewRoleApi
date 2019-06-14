using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SubzzManage.Business.Manage.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Subzz.Api.Controllers.Base;
using SubzzV2.Core.Models;

namespace Subzz.Api.Controllers.Manage
{
    [Produces("application/json")]
    [Route("api/School")]
    public class SchoolController : BaseApiController
    {
        private readonly ISchoolService _service;
        public SchoolController(ISchoolService service)
        {
            _service = service;
        }
        [Route("insertSchool")]
        [HttpPost]
        public OrganizationModel InsertSchool([FromBody]OrganizationModel model)
        {
            try
            {
                var school = _service.InsertSchool(model);
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
                return _service.UpdateSchool(model);
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
        public bool Delete(string id)
        {
                return _service.DeleteSchool(id);
        }

        [Route("getSchoolById/{id}")]
        [HttpGet]
        public IEnumerable<OrganizationModel> GetSchoolById(string id)
        {
            try
            {
                return _service.GetSchool(id);
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
    }
}