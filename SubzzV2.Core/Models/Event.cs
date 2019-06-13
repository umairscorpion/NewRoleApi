using System;

namespace SubzzV2.Core.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Notes { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public bool? IsArchived { get; set; }
        public DateTime? ArchivedOn { get; set; }
        public string ArchivedBy { get; set; }
    }
}
