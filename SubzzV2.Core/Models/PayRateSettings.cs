using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class PayRateSettings
    {
        public int Id { get; set; }
        public int PositionId { get; set; }
        public int DistrictId { get; set; }
        public int PayRate { get; set; }
        public string Period { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public bool IsArchived { get; set; }
        public string ArchivedBy { get; set; }
    }
}
