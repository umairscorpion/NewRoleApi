using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Entities
{
    public class UserLogin
    {
        public int Id
        {
            get; set;
        }
        public string UserId { get; set; }
        public int UserStatusId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string Pin { get; set; }
        public bool IsActive { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public int UserTypeId { get; set; }
        public string Description { get; set; }
        public string ActivationCode { get; set; }
        public UserProfile Profile { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UserIP { get; set; }
        public int IsSubscribedSms { get; set; }
        public int IsSubscribedEmail { get; set; }
        public string UserBrowser { get; set; }
        public string BrowserVersion { get; set; }
        public string OperatingSystem { get; set; }
        public string OSVersion { get; set; }
        public string Device { get; set; }
        public string Password { get; set; }
        public int DistrictId { get; set; }
        public string OrganizationId { get; set; }
        public bool IsCertified { get; set; }
    }
}
