using System.ComponentModel.DataAnnotations;
using CMS.Domain.Notifications.Enums;

namespace CMS.Domain.Notifications.Entities
{
    public class NotificationTemplate
    {
        public Guid TemplateID { get; set; }
        [Required]
        public string TemplateName { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string MessageBody { get; set; }
        [Required]
        public NotificationType NotificationType { get; set; }
        [Required]
        public string TriggerEvent { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}