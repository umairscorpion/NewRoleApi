using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class AbsenceScope
    {
        public string Title { get; set; }
        public int Id { get; set; }
        public bool Visibility { get; set; }
        public string OrganizatonId { get; set; }
        public int AbsenceType { get; set; }
        public int DistrictId { get; set; }
    }
}
