using System;

namespace SubzzV2.Core.Models
{
    public class UserAvailability
    {
        public int AvailabilityId { get; set; }
        public string UserId { get; set; }
        public int AvailabilityStatusId { get; set; }
        public string AvailabilityContentBackgroundColor { get; set; }
        public string AvailabilityIconCss { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool IsAllDayOut { get; set; }
        public bool IsRepeat { get; set; }
        public string RepeatType { get; set; }
        public int? RepeatValue { get; set; }
        public string RepeatOnWeekDays { get; set; }
        public bool IsEndsNever { get; set; }
        public int? EndsOnAfterNumberOfOccurrance { get; set; }
        public string EndsOnUntilDate { get; set; }
        public string Notes { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public bool? IsArchived { get; set; }
        public DateTime? ArchivedOn { get; set; }
        public string ArchivedBy { get; set; }
        public string AvailabilityStatusName { get; set; }
        public string AvailabilityStatusTitle { get; set; }
    }
}
