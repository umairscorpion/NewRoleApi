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
        public IEnumerable<LeaveRequestModel> GetLeaveRequests(int districtId, string organizationId)
        {
            return _repo.GetLeaveRequests(districtId, organizationId);
        }
        public LeaveTypeModel InsertLeaveType(LeaveTypeModel model)
        {
            return _repo.InsertLeaveType(model);
        }
        public IEnumerable<LeaveTypeModel> GetLeaveTypes()
        {
            return _repo.GetLeaveTypes();
        }

        public IEnumerable<LeaveTypeModel> GetLeaveTypes(int districtId, string organizationId)
        {
            return _repo.GetLeaveTypes(districtId, organizationId);
        }

        public int DeleteLeaveType(int leaveTypeId)
        {
            return _repo.DeleteLeaveType(leaveTypeId);
        }

        public LeaveTypeModel GetleaveTypeById(int leaveTypeId)
        {
            return _repo.GetleaveTypeById(leaveTypeId);
        }

        public IEnumerable<LeaveBalance> GetEmployeeLeaveBalance(LeaveBalance leaveBalance)
        {
            return _repo.GetEmployeeLeaveBalance(leaveBalance);
        }
    }
}
