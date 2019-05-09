using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class TimeclockFilter    {        public string StartDate { get; set; }        public string EndDate { get; set; }        public int IsDaysSelected { get; set; }        public int DistrictId { get; set; }        public string OrganizationId { get; set; }        public string LocationId { get; set; }        public string UserId { get; set; }    }
}
