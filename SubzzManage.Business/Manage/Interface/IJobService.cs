using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SubzzManage.Business.Manage.Interface
{
    public interface IJobService
    {
        Task<IEnumerable<AbsenceModel>> GetAvailableJobs(DateTime StartDate, DateTime EndDate, string UserId, string OrganizationId, int DistrictId, int status, bool Requested);
        Task<string> AcceptJob(int AbsenceId, string SubstituteId, string AcceptVia);
    }
}
