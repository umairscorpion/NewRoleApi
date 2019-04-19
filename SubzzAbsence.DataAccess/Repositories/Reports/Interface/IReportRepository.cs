﻿using System;
using System.Collections.Generic;
using System.Text;
using SubzzV2.Core.Models;

namespace SubzzAbsence.DataAccess.Repositories.Reports.Interface
{
    public interface IReportRepository
    {
        List<ReportSummary> GetReportSummary(ReportFilter reportFilter);
        List<ReportDetail> GetReportDetail(ReportFilter model);
        int DeleteAbsences(string model);
    }
}
