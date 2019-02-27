using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class SubstitutePreferenceModel
    {
        public string UserId { get; set; }
        public string BlockedSubstituteList { get; set; }
        public string FavoriteSubstituteList { get; set; }
    }
}
