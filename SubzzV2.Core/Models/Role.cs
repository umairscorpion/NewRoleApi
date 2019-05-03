using System.Collections.Generic;

namespace SubzzV2.Core.Models
{
    public class Role
    {
        public Role()
        {
            PermissionsCategories = new List<PermissionsCategory>();
        }

        public int Role_Id { get; set; }
        public int? DistrictId { get; set; }
        public string Name { get; set; }
        public int? UsersCount { get; set; }
        public List<PermissionsCategory> PermissionsCategories { get; set; }
    }
}
