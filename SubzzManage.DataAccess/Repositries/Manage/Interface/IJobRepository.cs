using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SubzzManage.DataAccess.Repositries.Manage.Interface
{
    public interface IJobRepository
    {
        Task<IEnumerable<AbsenceModel>> GetAvailableJobs(DateTime StartDate, DateTime EndDate, string UserId, string OrganizationId, int DistrictId, int status);
        Task<string> AcceptJob(int AbsenceId, string SubstituteId, string AcceptVia);
    }
}
