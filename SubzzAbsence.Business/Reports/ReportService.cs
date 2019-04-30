using System;
using System.Collections.Generic;
using System.Text;
using SubzzAbsence.Business.Reports.Interface;
using SubzzAbsence.DataAccess.Repositories.Reports.Interface;
using SubzzV2.Core.Models;

namespace SubzzAbsence.Business.Reports
{
    public class ReportService:IReportService
    {
        private readonly IReportRepository _repo;
        public ReportService(IReportRepository repo)
        {
            _repo = repo;
        }

        public List<ReportSummary> GetReportSummary(ReportFilter model)
        {
            return _repo.GetReportSummary(model);
        }

        public List<ReportDetail> GetReportDetails(ReportFilter model)
        {
            return _repo.GetReportDetail(model);
        }

        public IEnumerable<LeaveRequestModel> GetActivityReportDetail(ReportFilter model)
        {
            return _repo.GetActivityReportDetail(model);
        }

        public List<ReportDetail> GetPayrollReportDetails(ReportFilter model)
        {
            return _repo.GetPayrollReportDetails(model);
        }

        public int DeleteAbsences(ReportFilter model)
        {
            return _repo.DeleteAbsences(model);
        }
    }
}
