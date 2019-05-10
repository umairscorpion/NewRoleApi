﻿using System;
using System.Collections.Generic;
using System.Text;
using SubzzV2.Core.Models;

namespace SubzzAbsence.Business.Reports.Interface
{
    public interface IReportService
    {
        List<ReportSummary> GetReportSummary(ReportFilter model);
        List<ReportDetail> GetReportDetails(ReportFilter model);
        List<ReportDetail> GetPayrollReportDetails(ReportFilter model);
        int DeleteAbsences(ReportFilter model);
    }
}
