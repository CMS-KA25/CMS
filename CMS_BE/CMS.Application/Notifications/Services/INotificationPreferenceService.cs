using CMS.Domain.NotificationModels;
using CMS.Domain.NotificationModels.Enums;
using CMS.Domain.Auth.Enums;

namespace CMS.Application.Notifications.Services
{
    public interface INotificationPreferenceService
    {
        Task<NotificationPreference?> GetPreferenceAsync(Guid userId, NotificationType type, NotificationChannelType channelType);
        Task<List<NotificationPreference>> GetUserPreferencesAsync(Guid userId);
        Task<bool> UpdatePreferenceAsync(NotificationPreference preference);
        Task<bool> CreatePreferenceAsync(NotificationPreference preference);
        Task<bool> DeletePreferenceAsync(Guid preferenceId);
        Task<bool> IsNotificationAllowedAsync(Guid userId, NotificationType type, NotificationChannelType channelType);
        Task<List<NotificationChannelType>> GetAllowedChannelsAsync(Guid userId, NotificationType type);
        Task<List<NotificationPreference>> GetPreferencesByUserRoleAsync(RoleType userRole);
    }
}
