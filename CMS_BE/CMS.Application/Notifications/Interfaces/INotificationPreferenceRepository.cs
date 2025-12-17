using CMS.Domain.NotificationModels;
using CMS.Domain.NotificationModels.Enums;
using CMS.Domain.Auth.Enums;

namespace CMS.Application.Notifications.Interfaces
{
    public interface INotificationPreferenceRepository
    {
        Task<NotificationPreference?> GetByIdAsync(Guid id);
        Task<NotificationPreference?> GetByUserTypeAndChannelAsync(Guid userId, NotificationType type, NotificationChannelType channelType);
        Task<List<NotificationPreference>> GetByUserIdAsync(Guid userId);
        Task<List<NotificationPreference>> GetByUserRoleAsync(RoleType userRole);
        Task<NotificationPreference> CreateAsync(NotificationPreference preference);
        Task<NotificationPreference> UpdateAsync(NotificationPreference preference);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid userId, NotificationType type, NotificationChannelType channelType);
    }
}