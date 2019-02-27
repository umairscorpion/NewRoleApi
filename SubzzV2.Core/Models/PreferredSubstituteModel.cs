using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class PreferredSubstituteModel
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string SubstituteId { get; set; }
        public int AbsenceId { get; set; }
        public int DistrictId { get; set; }
        public int Interval { get; set; }
        public int IntervalToSendAll { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SubstitutePhoneNumber { get; set; }
        public bool IsSendAll { get; set; }
    }
}
