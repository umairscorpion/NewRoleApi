using System;

namespace SubzzV2.Core.Models
{
    public class Announcements
    {
        public int AnnouncementId { get; set; }
        public int Recipients { get; set; }
        public int DistrictId { get; set; }
        public string OrganizationId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool ScheduleAnnouncement { get; set; }
        public bool ShowOn { get; set; }
        public bool HideOn { get; set; }
        public DateTime ShowOnDate { get; set; }
        public DateTime HideOnDate { get; set; }
        public TimeSpan ShowOnTime { get; set; }
        public TimeSpan HideOnTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
