using System;

namespace SubzzV2.Core.Models
{
    public class ReportDetail
    {
        public int AbsenceId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeTypeTitle { get; set; }
        public int AbsencePosition { get; set; }
        public string Grade { get; set; }
        public string Subject { get; set; }
        public string Reason { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string DistrictName { get; set; }
        public DateTime PostedOn { get; set; }
        public string PostedById { get; set; }
        public string PostedByName { get; set; }
        public int StatusId { get; set; }
        public string StatusTitle { get; set; }
        public DateTime StatusDate { get; set; }
        public string SubstituteId { get; set; }
        public string SubstituteName { get; set; }
        public string SubstituteProfilePicUrl { get; set; }
        public string Notes { get; set; }
        public string AttachedFileName { get; set; }
        public string FileContentType { get; set; }
        public bool AnyAttachment { get; set; }
        public bool SubstituteRequired { get; set; }
        public string DurationType { get; set; }
        public int PayRate { get; set; }
        public int DailyHours { get; set; }
        public string SchoolName { get; set; }
        public string ReasonId { get; set; }
        public int AbsenceScope { get; set; }
        public string EmployeeId { get; set; }
        public bool IsApproved { get; set; }
        public string OriginalFileName { get; set; }
        public string EmployeeProfilePicUrl { get; set; }
        public int AbsenceType { get; set; }
    }
}
