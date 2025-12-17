using CMS.Domain.NotificationModels.Enums;

namespace CMS.Application.Notifications.Services
{
    public interface ITemplateNotificationService
    {
        Task<bool> SendNotificationAsync(Guid templateId, string recipient, string recipientName, Dictionary<string, object>? variables = null);
        Task<bool> SendNotificationByTypeAsync(NotificationType type, NotificationChannelType channelType, string recipient, string recipientName, Dictionary<string, object>? variables = null);
    }
}
