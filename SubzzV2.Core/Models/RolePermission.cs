
namespace SubzzV2.Core.Models
{
    public class RolePermission
    {
        public int? Id { get; set; }
        public int? RoleId { get; set; }
        public string RoleName { get; set; }
        public int PermissionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PermissionCategoryId { get; set; }
        public string DisplayName { get; set; }
        public bool IsChecked { get; set; }
        public bool IsHided { get; set; }
    }
}
