using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class Allowance
    {
        public int Id { get; set; }
        public int DistrictId { get; set; }
        public string Title { get; set; }
        public string YearlyAllowance { get; set; }
        public bool IsDeductAllowance { get; set; }
        public bool IsResidualDays { get; set; }
        public bool IsEnalbled { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
