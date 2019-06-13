using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class LookupModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        //For Time Zone
        public int StateId { get; set; }
        public string TimeZone_Title { get; set; }
        public int Timezone_Value { get; set; }
    }
}
