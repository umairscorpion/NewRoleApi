﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubzzV2.Core.Models;

namespace SubzzAbsence.Business.Leaves.Interface
{
    public interface ILeaveService
    {
        LeaveRequestModel InsertLeaveRequest(LeaveRequestModel model);
        LeaveRequestModel UpdateLeaveRequestStatus(LeaveRequestModel model);  
        LeaveTypeModel InsertLeaveType(LeaveTypeModel model);
        IEnumerable<LeaveRequestModel> GetLeaveRequests(int districtId, string organizationId);
        IEnumerable<LeaveTypeModel> GetLeaveTypes();
        IEnumerable<LeaveTypeModel> GetLeaveTypes(int districtId, string organizationId);
        int DeleteLeaveType(int leaveTypeId);
        LeaveTypeModel GetleaveTypeById(int leaveTypeId);
        IEnumerable<LeaveBalance> GetEmployeeLeaveBalance(int districtId, int year, string userId);
    }
}
