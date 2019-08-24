using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class SubstituteCategory
    {
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string OrganizationId { get; set; }
        public int DistrictId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string TimeToSendNotification { get; set; }
        public List<SubstituteList> SubstituteList { get; set; }
    }
}
