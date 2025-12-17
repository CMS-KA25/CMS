using System.ComponentModel.DataAnnotations;
using CMS.Domain.NotificationModels.Enums;

namespace CMS.Application.Notifications.DTOs
{
    public class CreateNotificationTemplateDto
    {
        [Required(ErrorMessage = "Template name is required")]
        [MaxLength(100, ErrorMessage = "Template name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject is required")]
        [MaxLength(200, ErrorMessage = "Subject cannot exceed 200 characters")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message body is required")]
        public string Body { get; set; } = string.Empty;

        [Required(ErrorMessage = "Notification type is required")]
        public NotificationType Type { get; set; }

        [Required(ErrorMessage = "Channel type is required")]
        public NotificationChannelType ChannelType { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public NotificationCategory Category { get; set; }

        public string? Variables { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateNotificationTemplateDto
    {
        [Required(ErrorMessage = "Template ID is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Template name is required")]
        [MaxLength(100, ErrorMessage = "Template name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject is required")]
        [MaxLength(200, ErrorMessage = "Subject cannot exceed 200 characters")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message body is required")]
        public string Body { get; set; } = string.Empty;

        [Required(ErrorMessage = "Notification type is required")]
        public NotificationType Type { get; set; }

        [Required(ErrorMessage = "Channel type is required")]
        public NotificationChannelType ChannelType { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public NotificationCategory Category { get; set; }

        public string? Variables { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class NotificationTemplateResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public NotificationChannelType ChannelType { get; set; }
        public NotificationCategory Category { get; set; }
        public string? Variables { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class SendNotificationDto
    {
        [Required(ErrorMessage = "Template ID is required")]
        public Guid TemplateId { get; set; }

        [Required(ErrorMessage = "Recipient email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string RecipientEmail { get; set; } = string.Empty;

        public string? RecipientPhone { get; set; }

        [Required(ErrorMessage = "Recipient name is required")]
        public string RecipientName { get; set; } = string.Empty;

        public Dictionary<string, object>? Variables { get; set; }
    }
}
