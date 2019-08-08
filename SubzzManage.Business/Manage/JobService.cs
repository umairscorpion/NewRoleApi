using SubzzManage.Business.Manage.Interface;
using SubzzManage.DataAccess.Repositries.Manage.Interface;
using SubzzV2.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SubzzManage.Business.Manage
{
    public class JobService: IJobService
    {
        private readonly IJobRepository _jobRepository;

        public JobService(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<IEnumerable<AbsenceModel>> GetAvailableJobs(DateTime StartDate, DateTime EndDate, string UserId, string OrganizationId, int DistrictId, int status, bool Requested)
        {
            return await _jobRepository.GetAvailableJobs(StartDate, EndDate, UserId, OrganizationId, DistrictId, status, Requested);
        }

        public async Task<string> AcceptJob(int AbsenceId, string SubstituteId, string AcceptVia)
        {
            return await _jobRepository.AcceptJob(AbsenceId, SubstituteId, AcceptVia);
        }

        public IEnumerable<AbsenceModel> GetRunningLate()
        {
            return _jobRepository.GetRunningLate();
        }
    }
}
