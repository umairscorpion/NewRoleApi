using Dapper;
using System.Linq;
using SubzzV2.Core.Models;
using System.Collections.Generic;
using SubzzAbsence.DataAccess.Repositories.Base;
using SubzzAbsence.DataAccess.Repositories.Reports.Interface;

namespace SubzzAbsence.DataAccess.Repositories.Reports
{
    public class ReportRepository : EntityRepository, IReportRepository
    {
        public List<ReportSummary> GetReportSummary(ReportFilter filter)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Report].[GetSummary]";
                var param = new DynamicParameters();
                if (filter.ReportTitle == "D")
                {
                    param.Add("@FromDate", filter.FromDate);
                    param.Add("@ToDate", filter.ToDate);
                }
                else
                {
                    if (filter.Month != 0 || filter.Year != "")
                    {
                        param.Add("@FromDate", null);
                        param.Add("@ToDate", null);
                    }
                    else
                    {
                        param.Add("@FromDate", filter.FromDate);
                        param.Add("@ToDate", filter.ToDate);
                    }
                }
                if (filter.LocationId == null || filter.LocationId == "")
                {
                    param.Add("@LocationId", filter.OrganizationId);
                    param.Add("@DistrictId", filter.District);
                }
                else
                {
                    param.Add("@LocationId", filter.LocationId);
                    param.Add("@DistrictId", filter.District);
                }
                param.Add("@JobNumber", filter.JobNumber);
                param.Add("@AbsenceTypeId", filter.AbsenceTypeId);
                param.Add("@ReasonId", filter.ReasonId);
                param.Add("@EmployeeName", filter.EmployeeName);
                param.Add("@Month", filter.Month);
                param.Add("@Year", filter.Year);
                param.Add("@AbsencePosition", filter.AbsencePosition);
                return connection.Query<ReportSummary>(sql, param, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
        }

        public List<ReportDetail> GetReportDetail(ReportFilter filter)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Report].[GetDetail]";
                var param = new DynamicParameters();
                if (filter.ReportTitle == "D")
                {
                    param.Add("@FromDate", filter.FromDate);
                    param.Add("@ToDate", filter.ToDate);
                }
                else
                {
                    if (filter.Month != 0 || filter.Year != "")
                    {
                        param.Add("@FromDate", null);
                        param.Add("@ToDate", null);
                    }
                    else
                    {
                        param.Add("@FromDate", filter.FromDate);
                        param.Add("@ToDate", filter.ToDate);
                    }
                }
                if (filter.LocationId == null || filter.LocationId == "")
                {
                    param.Add("@LocationId", filter.OrganizationId);
                    param.Add("@DistrictId", filter.District);
                }
                else
                {
                    param.Add("@LocationId", filter.LocationId);
                    param.Add("@DistrictId", filter.District);
                }
                param.Add("@JobNumber", filter.JobNumber);
                param.Add("@AbsenceTypeId", filter.AbsenceTypeId);
                param.Add("@ReasonId", filter.ReasonId);
                param.Add("@EmployeeName", filter.EmployeeName);
                param.Add("@Month", filter.Month);
                param.Add("@Year", filter.Year);
                param.Add("@AbsencePosition", filter.AbsencePosition);
                return connection.Query<ReportDetail>(sql, param, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
        }

        public int DeleteAbsences(ReportFilter filter)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Report].[CancelAbsence]";
                var param = new DynamicParameters();
                if (filter.OrganizationId == "1")
                {
                    param.Add("@OrganizationId", null);
                }
                else
                {
                    param.Add("@OrganizationId", filter.OrganizationId);
                }
                param.Add("@FromDate", filter.FromDate);
                param.Add("@ToDate", filter.ToDate);
                param.Add("@JobNumber", filter.JobNumber);
                param.Add("@AbsenceTypeId", filter.AbsenceTypeId);
                param.Add("@ReasonId", filter.ReasonId);
                param.Add("@EmployeeName", filter.EmployeeName);
                param.Add("@DeleteAbsenceReason", filter.DeleteAbsenceReason);
                param.Add("@DistrictId", filter.District);
                param.Add("@AbsencePosition", filter.AbsencePosition);
                param.Add("@UserId", filter.UserId);
                return connection.ExecuteScalar<int>(sql, param, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public List<ReportDetail> GetPayrollReportDetails(ReportFilter filter)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Report].[GetPayrollReportDetails]";
                var param = new DynamicParameters();
                param.Add("@FromDate", filter.FromDate);
                param.Add("@ToDate", filter.ToDate);
                param.Add("@LocationId", filter.OrganizationId);
                param.Add("@DistrictId", filter.DistrictId);
                param.Add("@UserId", filter.UserId);
                return connection.Query<ReportDetail>(sql, param, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
        }
    }
}
