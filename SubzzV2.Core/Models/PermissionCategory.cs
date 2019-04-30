using System.Collections.Generic;

namespace SubzzV2.Core.Models
{
    public class PermissionsCategory
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<RolePermission> Permissions { get; set; }
        public bool IsChecked { get; set; }
    }
}
