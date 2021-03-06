﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Subzz.Api.Controllers.Base;
using SubzzLookup.Business.Lookups.Interface;
using SubzzV2.Core.Models;

namespace Subzz.Api.Controllers.Lookup
{
    [Produces("application/json")]
    [Route("api/lookup")]
    public class LookupController : BaseApiController
    {
        private readonly ILookupService _service;
        public LookupController(ILookupService service)
        {
            _service = service;
        }

        [Route("getUserRoles")]
        [HttpGet]
        public IEnumerable<SubzzV2.Core.Entities.User> GetUserRoles()
        {
            try
            {
                var countries = _service.GetAllUserRoles();
                return countries;
            }
            catch(Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getTeachingLevels")]
        [HttpGet]
        public IEnumerable<LookupModel> GetTeachingLevels()
        {
            try
            {
                var countries = _service.GetTeachingLevels();
                return countries;
            }
            catch(Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("availability-status")]
        [HttpGet]
        public IEnumerable<LookupModel> GetAvailabilityStatuses()
        {
            try
            {
                var result = _service.GetAvailabilityStatuses();
                return result;
            }
            catch(Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("teachingSubjects")]
        [HttpGet]
        public IEnumerable<LookupModel> GetTeachingSubjects()
        {
            try
            {
                var countries = _service.GetTeachingSubjects();
                return countries;
            }
            catch(Exception ex)
            {
            }
            finally
            {
            }
            return null;        }
    }
}