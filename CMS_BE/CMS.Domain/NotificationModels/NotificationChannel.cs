using System.ComponentModel.DataAnnotations;
using CMS.Domain.NotificationModels.Enums;

namespace CMS.Domain.NotificationModels
{
    public class NotificationChannel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public Guid NotificationId { get; set; }
        
        [Required]
        public NotificationChannelType ChannelType { get; set; }
        
        [Required]
        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
        
        public string? ExternalId { get; set; } // ID from external service
        
        public DateTime? SentAt { get; set; }
        
        public DateTime? DeliveredAt { get; set; }
        
        public DateTime? FailedAt { get; set; }
        
        public string? ErrorMessage { get; set; }
        
        public int RetryCount { get; set; } = 0;
        
        public string? DeliveryReceipt { get; set; }
        
        public Dictionary<string, object>? ProviderMetadata { get; set; }
        
        public Notification? Notification { get; set; }
    }
}
