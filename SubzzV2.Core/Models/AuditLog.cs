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
        public string   PreValue { get; set; }
        public string PostValue { get; set; }   
    }
}
