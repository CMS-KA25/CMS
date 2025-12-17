using System.ComponentModel.DataAnnotations;
using CMS.Domain.NotificationModels.Enums;

namespace CMS.Domain.NotificationModels
{
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Message { get; set; } = string.Empty;
        
        [Required]
        public NotificationType Type { get; set; }
        
        [Required]
        public NotificationPriority Priority { get; set; }
        
        [Required]
        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
        
        [Required]
        public Guid RecipientId { get; set; }
        
        [MaxLength(100)]
        public string? RecipientEmail { get; set; }
        
        [MaxLength(20)]
        public string? RecipientPhone { get; set; }
        
        public Guid? SenderId { get; set; }
        
        public Guid? AppointmentId { get; set; }
        
        public Guid? PatientId { get; set; }
        
        public Guid? DoctorId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ScheduledFor { get; set; }
        
        public DateTime? SentAt { get; set; }
        
        public DateTime? ReadAt { get; set; }
        
        public int RetryCount { get; set; } = 0;
        
        public int MaxRetries { get; set; } = 3;
        
        public string? ErrorMessage { get; set; }
        
        public Dictionary<string, object>? Metadata { get; set; }
        
        public List<NotificationChannel> Channels { get; set; } = new();
        
        public Guid? TemplateId { get; set; }
        public NotificationTemplate? Template { get; set; }
    }
}
