using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Entities
{
    public class UserBasicInfo
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        //public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PartnerReferralId { get; set; }
    }
}
