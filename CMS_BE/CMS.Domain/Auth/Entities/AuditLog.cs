using System.ComponentModel.DataAnnotations;

namespace CMS.Domain.Auth.Entities
{
    public class AuditLog
    {
        [Required]
        public Guid AuditID { get; set; }
        [Required]
        public Guid UserID { get; set; }
        [Required]
        public string Action { get; set; }
        [Required]
        public string TableName { get; set; }
        [Required]
        public Guid RecordID { get; set; }
        [Required]
        public string ActionDescription { get; set; }
        [Required]
        public string ActionResult { get; set; }
        public string? ErrorMessage { get; set; }
        public string? CorrelationId { get; set; }
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }
        [Required]
        public DateTime ActionTimestamp { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
