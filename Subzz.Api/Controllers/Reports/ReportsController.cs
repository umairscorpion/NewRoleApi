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
            try
            {
                model.District = base.CurrentUser.DistrictId;
                model.OrganizationId = base.CurrentUser.OrganizationId;
                model.UserId = base.CurrentUser.Id;
                var reportSummary = _service.GetReportSummary(model);
                return Ok(reportSummary);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("details")]
        [HttpPost]
        public IActionResult GetDetails([FromBody]ReportFilter model)
        {
            try
            {
                model.District = base.CurrentUser.DistrictId;
                model.OrganizationId = base.CurrentUser.OrganizationId;
                model.UserId = base.CurrentUser.Id;
                var reportDetails = _service.GetReportDetails(model);
                return Ok(reportDetails);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("deleteAbsences")]
        [HttpPost]
        public IActionResult DeleteAbsences([FromBody]ReportFilter model)
        {
            try
            {
                model.UserId = base.CurrentUser.Id;
                int RowsEffected = _service.DeleteAbsences(model);
                if (RowsEffected > 0)
                    return Json("success");
                return Json("error");
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return null;
        }

        [Route("payrollDetail")]
        [HttpPost]
        public IActionResult GetPayrollDetail([FromBody]ReportFilter model)
        {
            try
            { 
            model.DistrictId = base.CurrentUser.DistrictId;
            model.OrganizationId = base.CurrentUser.OrganizationId;
            model.UserId = base.CurrentUser.Id;
            var reportDetails = _service.GetPayrollReportDetails(model);
            return Ok(reportDetails);
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