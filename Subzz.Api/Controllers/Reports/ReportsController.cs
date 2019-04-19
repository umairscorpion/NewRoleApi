using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Subzz.Api.Controllers.Base;
using SubzzAbsence.Business.Reports.Interface;
using SubzzV2.Core.Models;

namespace Subzz.Api.Controllers.Reports
{
    [Route("api/reports")]
    public class ReportsController : BaseApiController
    {
        private readonly IReportService _service;
        public ReportsController(IReportService service)
        {
            _service = service;
        }

        [Route("summary")]
        [HttpPost]
        public IActionResult GetSummary([FromBody]ReportFilter model)
        {
            var reportSummary = _service.GetReportSummary(model);
            return Ok(reportSummary);
        }

        [Route("details")]
        [HttpPost]
        public IActionResult GetDetails([FromBody]ReportFilter model)
        {
            var reportDetails = _service.GetReportDetails(model);
            return Ok(reportDetails);
        }

        [Route("deleteAbsences/{data}")]
        [HttpGet]
        public IActionResult DeleteAbsences(string data)
        {
            var reportDetails = _service.DeleteAbsences(data);
            return Json("success");
        }
    }
}