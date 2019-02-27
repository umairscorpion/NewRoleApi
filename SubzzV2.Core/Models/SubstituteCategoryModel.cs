using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class SubstituteCategoryModel
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string Title { get; set; }
        public string UserId { get; set; }
        public bool IsNotificationSend { get; set; }
        public string Date { get; set; }
    }
}
