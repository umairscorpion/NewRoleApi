using System;

namespace SubzzV2.Core.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public DateTime OccurredOn { get; set; }
        public string UserId { get; set; }
        public string EntityId { get; set; }
        public string EntityType { get; set; }
        public string ActionType { get; set; }
        public string PreValue { get; set; }
        public string PostValue { get; set; }
        public int DistrictId { get; set; }
        public string OrganizationId { get; set; }
    }

    public class AuditLogView
    {
        public int Id { get; set; }
        public DateTime OccurredOn { get; set; }
        public string User { get; set; }
        public string Event { get; set; }
    }

    public class AuditLogAbsenceView
    {
        public string EntityId { get; set; }
        public string Created { get; set; }
        public string Approved { get; set; }
        public string Accepted { get; set; }
        public string Released { get; set; }
        public string Declined { get; set; }
        public string Cancelled { get; set; }
        public string Assigned { get; set; }
        public string Updated { get; set; }
        public string SubstituteName { get; set; }
        public string EntityType { get; set; }
    }
}
