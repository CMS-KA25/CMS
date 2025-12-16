using System.ComponentModel.DataAnnotations;
using CMS.Domain.Notifications.Enums;

namespace CMS.Domain.Notifications.Entities
{
    public class NotificationInstance
    {
        public Guid NotificationID { get; set; }
        [Required]
        public Guid TemplateID { get; set; }
        [Required]
        public RecipientType RecipientType { get; set; }
        [Required]
        public Guid[] RecipientIDs { get; set; }
        [Required]
        public string? Subject { get; set; }
        [Required]
        public string? Message { get; set; }
        public string[] RecipientContact { get; set; }
        [Required]
        public NotificationStatus NotificationStatus { get; set; }
        public DateTime ScheduledAt { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime DeliveredAt { get; set; }
        public int RetryCount { get; set; }
        public string? ErrorMessage { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}