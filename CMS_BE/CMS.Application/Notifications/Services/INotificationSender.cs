using CMS.Domain.NotificationModels;

namespace CMS.Application.Notifications.Services
{
    public interface INotificationSender
    {
        Task<Guid> SendNotificationAsync(Notification notification);
        Task<Notification?> GetNotificationByIdAsync(Guid id);
    }
}