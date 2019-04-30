using System;
using System.Collections.Generic;

namespace SubzzV2.Core.Models
{
    public class PermissionMaster
    {
        public List<PermissionsCategory> Permissions { get; set; }
        public int RoleId { get; set; }
        public string CreateUpdateBy { get; set; }
        public DateTime CreateUpdateOn { get; set; }
    }
}
