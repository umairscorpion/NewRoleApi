using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class UserResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CSS { get; set; }
        public string StateMachine { get; set; }
        public string URL { get; set; }
        public int ResourceTypeId { get; set; }
        public int RoleId { get; set; }
        public int PrivilegeId { get; set; }
        public int UserId { get; set; }
        public bool ShowInTopNavigation { get; set; }
        public bool ShowInLeftNavigation { get; set; }
    }
}
