using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class SchoolSubList
    {
        public int Id { get; set; }
        public string AddedByUserId { get; set; }
        public string ModifyByUserId { get; set; }
        public string SubstituteId { get; set; }
        public string SubstituteName { get; set; }
        public int DistrictId { get; set; }
        public bool IsAdded { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
