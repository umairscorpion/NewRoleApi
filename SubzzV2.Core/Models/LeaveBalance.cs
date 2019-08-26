using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class LeaveBalance
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Personal { get; set; }
        public string Sick { get; set; }
        public string Vacation { get; set; }
        public string OrganizationId { get; set; }
        public int DistrictId { get; set; }
        public int Year { get; set; }
        public int LeaveTypeId { get; set; }
        public int Balance { get; set; }
        public bool IsAllowNegativeAllowance { get; set; }
        public int BalanceId { get; set; }
        public string AllowanceTitle { get; set; }
        public decimal AllowanceBalance { get; set; }
        public decimal FirstColumn { get; set; }
        public decimal SecondColumn { get; set; }
        public decimal ThirdColumn { get; set; }
        public string AbsenceStartDate { get; set; }
        public string AbsenceEndDate { get; set; }
        public int allowanceType { get; set; }
    }
}
