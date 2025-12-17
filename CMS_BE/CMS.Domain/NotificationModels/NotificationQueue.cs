using System.ComponentModel.DataAnnotations;
using CMS.Domain.NotificationModels.Enums;

namespace CMS.Domain.NotificationModels
{
    public class NotificationQueue
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public Guid NotificationId { get; set; }
        
        [Required]
        public NotificationPriority Priority { get; set; }
        
        [Required]
        public DateTime ScheduledFor { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ProcessedAt { get; set; }
        
        public int AttemptCount { get; set; } = 0;
        
        public int MaxAttempts { get; set; } = 3;
        
        public string? ErrorMessage { get; set; }
        
        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
        
        public string? ProcessingNode { get; set; }
        
        public Notification? Notification { get; set; }
    }
}
