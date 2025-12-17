using CMS.Domain.NotificationModels;
using CMS.Domain.NotificationModels.Enums;

namespace CMS.Application.Notifications.Services
{
    public interface INotificationService
    {
        Task<Guid> SendNotificationAsync(Notification notification);
        Task<Guid> SendNotificationAsync(string title, string message, NotificationType type, 
            Guid recipientId, NotificationPriority priority = NotificationPriority.Normal);
        Task<bool> SendBulkNotificationsAsync(List<Notification> notifications);
        Task<Notification?> GetNotificationByIdAsync(Guid id);
        Task<List<Notification>> GetNotificationsByUserIdAsync(Guid userId, int page = 1, int pageSize = 20);
        Task<bool> MarkAsReadAsync(Guid notificationId);
        Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId);
        Task<bool> DeleteNotificationAsync(Guid notificationId);
        Task<List<Notification>> GetUnreadNotificationsAsync(Guid userId);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task<List<Notification>> GetNotificationsByTypeAsync(NotificationType type, int page = 1, int pageSize = 20);
        Task<List<Notification>> GetNotificationsByStatusAsync(NotificationStatus status, int page = 1, int pageSize = 20);
    }
}
