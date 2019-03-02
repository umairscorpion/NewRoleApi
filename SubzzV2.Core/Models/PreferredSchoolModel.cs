using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class PreferredSchoolModel
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationId { get; set; }
        public bool IsEnabled { get; set; }
        public string UserId { get; set; }
    }
}
