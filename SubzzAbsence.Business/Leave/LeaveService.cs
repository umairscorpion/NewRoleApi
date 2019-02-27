using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubzzAbsence.Business.Leaves.Interface;
using SubzzAbsence.DataAccess.Repositories.Leaves.Interface;
using SubzzV2.Core.Models;

namespace SubzzAbsence.Business.Leaves
{
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _repo;
        public LeaveService(ILeaveRepository repo)
        {
            _repo = repo;
        }
        public LeaveRequestModel InsertLeaveRequest(LeaveRequestModel model)
        {
            return _repo.InsertLeaveRequest(model);
        }
        public LeaveRequestModel UpdateLeaveRequestStatus(LeaveRequestModel model)
        {
            return _repo.UpdateLeaveRequestStatus(model);
        }
        public IEnumerable<LeaveRequestModel> GetLeaveRequests(int IsApproved, int IsDenied)
        {
            return _repo.GetLeaveRequests(IsApproved, IsDenied);
        }
        public LeaveTypeModel InsertLeaveType(LeaveTypeModel model)
        {
            return _repo.InsertLeaveType(model);
        }
        public IEnumerable<LeaveTypeModel> GetLeaveTypes()
        {
            return _repo.GetLeaveTypes();
        }
    }
}
