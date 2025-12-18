using System.ComponentModel.DataAnnotations;

namespace CMS.Domain.Auth.Entities
{
    public class AuditLog
    {
        public Guid AuditID { get; set; }
        public Guid UserID { get; set; }
        public string Action { get; set; } // CREATE, UPDATE, DELETE
        public string TableName { get; set; }
        public bool ActionResult { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public string? CorrelationId { get; set; }
        public string? IPAddress { get; set; }
        public DateTime ActionTimestamp { get; set; }
    }
}
