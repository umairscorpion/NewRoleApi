using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class UserRefree
    {
        public int Id { get; set; }
        public string UserReferralCode { get; set; }
        public long RefereeUserId { get; set; }
        public int ReferralStatusId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DateUsed { get; set; }

        public string Email { get; set; }
        public decimal RefereeAmount { get; set; }
    }
}
