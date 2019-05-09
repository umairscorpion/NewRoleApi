using System;

namespace SubzzV2.Core.Models
{
    public class AuditLogFilter
    {
        public string LoginUserId { get; set; }
        public string SearchByEmployeeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DistrictId { get; set; }
        public string OrganizationId { get; set; }
        public string EntityId { get; set; }
    }
}
