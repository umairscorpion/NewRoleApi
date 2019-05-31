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
            var school = _service.InsertSchool(model);
            return school;
        }

        [Route("updateSchool")]
        [HttpPatch]
        public OrganizationModel UpdateSchool([FromBody]OrganizationModel model)
        {
            return _service.UpdateSchool(model);
        }

        [Route("getSchools")]
        [HttpGet]
        public IEnumerable<OrganizationModel> GetSchools()
        {
            var schools = _service.GetSchools();
            return schools;
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
            return _service.GetSchool(id);
        }

        [Route("getOrganizationsByDistrictId/{id}")]
        [HttpGet]
        public IEnumerable<OrganizationModel> GetOrganizationsByDistrictId(int id)
        {
            return _service.GetOrganizationsByDistrictId(id);
        }

        [Route("getOrganizationTimeByOrganizationId/{OrganizationId}")]
        [HttpGet]
        public LocationTime GetOrganizationTimeByOrganizationId(string OrganizationId)
        {
            return _service.GetOrganizationTimeByOrganizationId(OrganizationId);
        }

        [Route("getAbsenceScopes")]
        [HttpPost]
        public IActionResult GetAbsenceScopes([FromBody]OrganizationModel organizationModel)
        {
            var scopes = _service.GetAbsenceScopes(organizationModel);
            return Ok(scopes);
        }

        [Route("updateAbsenceScopes")]
        [HttpPost]
        public IActionResult UpdateAbsenceScopes([FromBody]List<AbsenceScope> absenceScopes)
        {
            foreach (AbsenceScope ab in absenceScopes)
            {
                _service.UpdateAbsenceScope(ab);
            }
            return Ok();
        }
    }
}