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
                if (filter.JobNumber == "" && filter.EmployeeTypeId == 0 && filter.AbsenceTypeId == 0 && filter.LocationId == 0 && filter.DistrictId == 0 && filter.ReasonId == 0 && filter.EmployeeName == "")
                {
                    param.Add("@FromDate", filter.FromDate);
                    param.Add("@ToDate", filter.ToDate);
                }
                else
                {
                    param.Add("@FromDate", null);
                    param.Add("@ToDate", null);
                }
                param.Add("@JobNumber", filter.JobNumber);
                param.Add("@EmployeeTypeId", filter.EmployeeTypeId);
                param.Add("@AbsenceTypeId", filter.AbsenceTypeId);
                param.Add("@LocationId", filter.LocationId);
                param.Add("@DistrictId", filter.DistrictId);
                param.Add("@ReasonId", filter.ReasonId);
                param.Add("@EmployeeName", filter.EmployeeName);
                return connection.Query<ReportSummary>(sql, param, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
        }

        public List<ReportDetail> GetReportDetail(ReportFilter filter)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Report].[GetDetail]";
                var param = new DynamicParameters();
                if (filter.JobNumber == "" && filter.EmployeeTypeId == 0 && filter.AbsenceTypeId == 0 && filter.LocationId == 0 && filter.DistrictId == 0 && filter.ReasonId == 0 && filter.EmployeeName == "")
                {
                    param.Add("@FromDate", filter.FromDate);
                    param.Add("@ToDate", filter.ToDate);
                }
                else
                {
                    param.Add("@FromDate", null);
                    param.Add("@ToDate", null);
                }
                param.Add("@JobNumber", filter.JobNumber);
                param.Add("@EmployeeTypeId", filter.EmployeeTypeId);
                param.Add("@AbsenceTypeId", filter.AbsenceTypeId);
                param.Add("@LocationId", filter.LocationId);
                param.Add("@DistrictId", filter.DistrictId);
                param.Add("@ReasonId", filter.ReasonId);
                param.Add("@EmployeeName", filter.EmployeeName);
                return connection.Query<ReportDetail>(sql, param, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
        }

        public int DeleteAbsences(string data)
        {
            using (var connection = base.GetConnection)
            {
                var sql = "[Absence].[CancelAbsences]";
                var param = new DynamicParameters();
                param.Add("@valueList", data);
                return connection.ExecuteScalar<int>(sql, param, commandType: System.Data.CommandType.StoredProcedure);
            }
        }
    }
}
