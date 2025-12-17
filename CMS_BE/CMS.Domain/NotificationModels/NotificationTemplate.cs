using System.ComponentModel.DataAnnotations;
using CMS.Domain.NotificationModels.Enums;

namespace CMS.Domain.NotificationModels
{
    public class NotificationTemplate
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(200)]
        public string Subject { get; set; } = string.Empty;
        
        [Required]
        public string Body { get; set; } = string.Empty;
        
        [Required]
        public NotificationType Type { get; set; }
        
        [Required]
        public NotificationChannelType ChannelType { get; set; }
        
        public string? Variables { get; set; } // JSON string of available variables
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public string? CreatedBy { get; set; }
        
        public string? UpdatedBy { get; set; }
        
        public NotificationCategory Category { get; set; }
        
        public string? Description { get; set; }
    }
}
