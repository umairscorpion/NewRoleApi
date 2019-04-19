﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SubzzManage.Business.District.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Subzz.Api.Controllers.Base;
using SubzzV2.Core.Models;
using FluentValidation.Results;
using Subzz.Api.Validators;

namespace Subzz.Api.Controllers.Manage
{
    [Produces("application/json")]
    [Route("api/District")]
    public class DistrictController : BaseApiController
    {
        private readonly IDistrictService _service;
        public DistrictController(IDistrictService service)
        {
            _service = service;
        }

        [Route("insertDistrict")]
        [HttpPost]
        public IActionResult InsertDistrict([FromBody]DistrictModel model)
        {
            DistrictValidator validator = new DistrictValidator();
            ValidationResult result = validator.Validate(model);
            if(result.IsValid)
            { var userModel = _service.InsertDistrict(model); }
            else
            {
               return BadRequest("Fill All fields");
            }
            
            return Json("successfull");
        }

        [Route("updateDistrict")]
        [HttpPatch]
        public IActionResult UpdateDistrict([FromBody]DistrictModel model)
        {
            DistrictValidator validator = new DistrictValidator();
            ValidationResult result = validator.Validate(model);
            if (result.IsValid)
            { var userModel = _service.UpdateDistrict(model); }
            else
            {
                return BadRequest("Fill All fields");
            }
            return Json("successfull");
        }

        [Route("getDistricts")]
        [HttpGet]
        public IEnumerable<DistrictModel> GetDistricts()
        {
            var districts = _service.GetDistricts();
            return districts;
        }

        [Route("{id}")]
        [HttpDelete]
        public bool Delete(int id)
        {
            return _service.DeleteDistrict(id);
        }

        [Route("getDistrictById/{Id}")]
        [HttpGet]
        public IEnumerable<DistrictModel> GetDistrict(int id)
        {
            var district = _service.GetDistrict(id);
            return district;
        }

        [Route("updateSettings")]
        [HttpPost]
        public IActionResult UpdateSettings([FromBody] DistrictModel districtModel)
        {
            var district = _service.UpdateSettings(districtModel);
            return Ok(district);
        }

        [Route("allowances")]
        [HttpPost]
        public IActionResult AddAllowances([FromBody]Allowance model)
        {
            var allowance = _service.AddAllowance(model);
            return Ok(allowance);
        }

        [Route("allowances")]
        [HttpPatch]
        public IActionResult UpdateAllowances([FromBody]Allowance model)
        {
            var allowance = _service.AddAllowance(model);
            return Ok(allowance);
        }

        [Route("getAllowances/{districtId}")]
        [HttpGet]
        public IActionResult GetAllowances(string districtId)
        {
            var allowance = _service.GetAllowances(districtId);
            return Ok(allowance);
        }

        [Route("deleteAllowance/{id}")]
        [HttpDelete]
        public IActionResult DeleteAllowance(int id)
        {
            var allowance = _service.DeleteAllowance(id);
            return Ok(allowance);
        }

    }
}