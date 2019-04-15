using System.Collections.Generic;

namespace SubzzV2.Core.Models
{
    public class CalendarEvent
    {
        public int id { get; set; }
        public string title { get; set; }
        public bool allDay { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public string[] className { get; set; }
        public bool editable { get; set; }
        public string color { get; set; }
        public string backgroundColor { get; set; }
        public string resourceId { get; set; }
        public string resourceName { get; set; }
        public string profilePicUrl { get; set; }

        public List<CalendarResource> Resources { get; set; }
    }

    public class CalendarResource
    {
        public string id { get; set; }
        public string title { get; set; }
        public string profilePicUrl { get; set; }
    }
}
