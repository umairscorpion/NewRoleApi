using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class SubstituteList
    {
        public int SubstituteListId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CategoryId { get; set; }
        public bool IsAdded { get; set; }
    }
}
