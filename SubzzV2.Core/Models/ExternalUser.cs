using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class ExternalUser
    {
        public string Email { get; set; }
        public string Id { get; set; }
        public string IdToken { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Providor { get; set; }
        public string Token { get; set; }
    }
}
