using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class UserReference
    {
        public int TotalCount { get; set; }
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int UserTypeId { get; set; }
        public int RoleId { get; set; }
        public int UserStatusId { get; set; }
        public string UserType { get; set; }
        public string UserStatus { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePicture { get; set; }
        public int DistrictId { get; set; }
        public string OrganizationId { get; set; }
        public int UserLevel { get; set; }
        public List<RolePermission> Permissions { get; set; }
        public bool IsViewedNewVersion { get; set; }
    }
}
