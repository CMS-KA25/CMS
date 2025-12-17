using CMS.Domain.NotificationModels;
using CMS.Domain.NotificationModels.Enums;

namespace CMS.Application.Notifications.Services
{
    public interface INotificationTemplateService
    {
        Task<NotificationTemplate?> GetTemplateAsync(NotificationType type, NotificationChannelType channelType);
        Task<NotificationTemplate?> GetTemplateByIdAsync(Guid id);
        Task<List<NotificationTemplate>> GetTemplatesByCategoryAsync(NotificationCategory category);
        Task<Guid> CreateTemplateAsync(NotificationTemplate template);
        Task<bool> UpdateTemplateAsync(NotificationTemplate template);
        Task<bool> DeleteTemplateAsync(Guid id);
        Task<string> ProcessTemplateAsync(NotificationTemplate template, Dictionary<string, object> variables);
        Task<List<NotificationTemplate>> GetAllTemplatesAsync();
        Task<List<NotificationTemplate>> GetActiveTemplatesAsync();
    }
}
