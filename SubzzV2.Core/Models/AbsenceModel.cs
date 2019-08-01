using SubzzV2.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class AbsenceModel
    {
        public AbsenceModel()
        {
            Users = new List<User>();
        }
        public int AbsenceId { get; set; }
        public string EmployeeId { get; set; }
        public string AbsenceCreatedByEmployeeId { get; set; }
        public string CreatedByUser { get; set; }
        public string EmployeeName { get; set; }
        public string SubstituteName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int AbsenceReasonId { get; set; }
        public string AbsenceStatusDescription { get; set; }
        public string AbsenceLocation { get; set; }
        public int DurationType { get; set; }
        public int PositionId { get; set; }
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationAddress { get; set; }
        public int Status { get; set; }
        public int DistrictId { get; set; }
        public string DistrictAddress { get; set; }
        public string SubstituteId { get; set; }
        public bool SubstituteRequired { get; set; }
        public int AbsenceScope { get; set; }
        public string PayrollNotes { get; set; }
        public string SubstituteNotes { get; set; }
        public string AttachedFileName { get; set; }
        public string FileContentType { get; set; }
        public string FileExtention { get; set; }
        public bool AnyAttachment { get; set; }
        public DateTime ApprovedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime AcceptedDate { get; set; }
        public string AbsenceReasonDescription { get; set; }
        public string PositionDescription { get; set; }
        public string SubjectDescription { get; set; }
        public string Grade { get; set; }
        public List<User> Users { get; set; }
        public int Interval { get; set; }
        public int TotalInterval { get; set; }
        public int anyConflict { get; set; }
        public string ProfilePicture { get; set; }
        public bool IsApprovalRequired { get; set; }
        public string UpdatedById { get; set; }
        public bool Requested { get; set; }
        public int SpecialityTypeId { get; set; }
        public string OriginalFileName { get; set; }
        public string EmployeeProfilePicUrl { get; set; }
        public int AbsenceType { get; set; }
        public string ConfirmationNumber { get; set; }
        public bool OnlyCertified { get; set; }
        public bool OnlySubjectSpecialist { get; set; }
        public int AbsenceResendCounter { get; set; }
        public string DistrictPhoneNumber { get; set; }
        public string OrganizationPhoneNumber { get; set; }
    }

}
