using CMS.Domain.NotificationModels;
using CMS.Domain.NotificationModels.Enums;

namespace CMS.Application.Notifications.Services
{
    public interface INotificationScheduler
    {
        Task<Guid> ScheduleNotificationAsync(Notification notification, DateTime scheduledFor);
        Task<bool> CancelScheduledNotificationAsync(Guid notificationId);
        Task<bool> RescheduleNotificationAsync(Guid notificationId, DateTime newScheduledFor);
        Task<List<Notification>> GetScheduledNotificationsAsync(DateTime from, DateTime to);
        Task<List<Notification>> ProcessScheduledNotificationsAsync();
        Task<bool> IsNotificationScheduledAsync(Guid notificationId);
        Task<List<NotificationQueue>> GetQueuedNotificationsAsync(int maxCount = 100);
        Task<bool> UpdateQueueStatusAsync(Guid queueId, NotificationStatus status, string? errorMessage = null);
    }
}
