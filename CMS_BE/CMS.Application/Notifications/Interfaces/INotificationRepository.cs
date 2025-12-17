using CMS.Domain.NotificationModels;
using CMS.Domain.NotificationModels.Enums;

namespace CMS.Application.Notifications.Interfaces
{
    public interface INotificationRepository
    {
        Task<Notification?> GetByIdAsync(Guid id);
        Task<List<Notification>> GetByUserIdAsync(Guid userId, int page = 1, int pageSize = 20);
        Task<List<Notification>> GetUnreadByUserIdAsync(Guid userId);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task<List<Notification>> GetByTypeAsync(NotificationType type, int page = 1, int pageSize = 20);
        Task<List<Notification>> GetByStatusAsync(NotificationStatus status, int page = 1, int pageSize = 20);
        Task<Notification> CreateAsync(Notification notification);
        Task<Notification> UpdateAsync(Notification notification);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> MarkAsReadAsync(Guid id, Guid userId);
        Task<List<Notification>> GetScheduledNotificationsAsync(DateTime from, DateTime to);
    }
}