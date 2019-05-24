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

        //Notification Events for Substitute
        public int NotificationId { get; set; }
        public int EventId { get; set; }
        public bool EmailAlert { get; set; }
        public bool TextAlert { get; set; }
        public string EventName { get; set; }
        public string SubstituteId { get; set; }

    }
}
