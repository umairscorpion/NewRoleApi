using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubzzV2.Core.Models;

namespace SubzzAbsence.DataAccess.Repositories.Leaves.Interface
{
    public interface ILeaveRepository
    {
        LeaveRequestModel InsertLeaveRequest(LeaveRequestModel model);
        LeaveRequestModel UpdateLeaveRequestStatus(LeaveRequestModel model);
        LeaveTypeModel InsertLeaveType(LeaveTypeModel model);
        IEnumerable<LeaveRequestModel> GetLeaveRequests(int IsApproved, int IsDenied);
        IEnumerable<LeaveTypeModel> GetLeaveTypes();
    }
}
