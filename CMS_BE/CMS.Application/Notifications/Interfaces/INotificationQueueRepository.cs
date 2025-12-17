using CMS.Domain.NotificationModels;
using CMS.Domain.NotificationModels.Enums;

namespace CMS.Application.Notifications.Interfaces
{
    public interface INotificationQueueRepository
    {
        Task<NotificationQueue?> GetByIdAsync(Guid id);
        Task<List<NotificationQueue>> GetScheduledAsync(DateTime from, DateTime to);
        Task<List<NotificationQueue>> GetPendingAsync(int maxCount = 100);
        Task<List<NotificationQueue>> GetByNotificationIdAsync(Guid notificationId);
        Task<NotificationQueue> CreateAsync(NotificationQueue queue);
        Task<NotificationQueue> UpdateAsync(NotificationQueue queue);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> UpdateStatusAsync(Guid id, NotificationStatus status, string? errorMessage = null);
        Task<bool> IncrementAttemptCountAsync(Guid id);
        Task<bool> ExistsAsync(Guid notificationId);
    }
}