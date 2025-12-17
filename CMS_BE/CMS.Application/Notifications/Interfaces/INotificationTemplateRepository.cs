using CMS.Domain.NotificationModels;
using CMS.Domain.NotificationModels.Enums;

namespace CMS.Application.Notifications.Interfaces
{
    public interface INotificationTemplateRepository
    {
        Task<NotificationTemplate?> GetByIdAsync(Guid id);
        Task<NotificationTemplate?> GetTemplateByTypeAndChannelAsync(NotificationType type, NotificationChannelType channelType);
        Task<List<NotificationTemplate>> GetTemplatesByCategoryAsync(NotificationCategory category);
        Task<NotificationTemplate?> GetTemplateByNameAndTypeAsync(string name, NotificationType type);
        Task<List<NotificationTemplate>> GetActiveTemplatesAsync();
        Task<List<NotificationTemplate>> GetAllAsync();
        Task AddAsync(NotificationTemplate template);
        Task UpdateAsync(NotificationTemplate template);
        Task DeleteAsync(Guid id);
        Task<int> SaveChangesAsync();
    }
}