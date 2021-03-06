﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Subzz.Api.Controllers.Base;
using SubzzV2.Core.Models;
using SubzzLookup.Business.Lookups.Interface;

namespace Subzz.Api.Controllers.Lookup
{
    [Produces("application/json")]
    [Route("api/districtLookup")]
    public class DistrictLookupController : BaseApiController
    {
        private readonly IDistrictLookupService _service;
        public DistrictLookupController(IDistrictLookupService service)
        {
            _service = service;
        }

        [Route("getCountries")]
        [HttpGet]
        public IEnumerable<CountryModel> GetCountries()
        {
            try
            {
                var countries = _service.GetCountries();
                return countries;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getStateByCountryId/{counrtyId}")]
        [HttpGet]
        public IEnumerable<StateModel> GetStateByCountryId(int counrtyId)
        {
            try
            {
                var states = _service.GetStateByCountryId(counrtyId);
                return states;
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("getTimeZone")]
        [HttpGet]
        public IEnumerable<LookupModel> GetTimeZone()
        {
            try
            {
                var List_Of_TimeZones = _service.GetTimeZone();
                return List_Of_TimeZones;
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