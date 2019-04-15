
using System;

namespace SubzzV2.Core.Models
{
    public class SubstituteAvailability
    {
        public int AvailabilityId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string ProfilePicUrl { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Speciality { get; set; }
        public string Available { get; set; }
        public int AvailabilityStatusId { get; set; }
        public string Title { get; set; }
        public string AvailabilityStatusName { get; set; }
        public string AvailabilityStatusTitle { get; set; }
        public string AvailabilityContentBackgroundColor { get; set; }
        public string AvailabilityIconCss { get; set; }
    }
}
