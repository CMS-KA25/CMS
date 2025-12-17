using System.ComponentModel.DataAnnotations;
using CMS.Domain.NotificationModels.Enums;
using CMS.Domain.Auth.Enums;

namespace CMS.Domain.NotificationModels
{
    public class NotificationPreference
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public NotificationType Type { get; set; }
        
        [Required]
        public NotificationChannelType ChannelType { get; set; }
        
        public bool IsEnabled { get; set; } = true;
        
        public bool IsRequired { get; set; } = false;
        
        public string? CustomSettings { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public RoleType UserRole { get; set; }
    }
}
