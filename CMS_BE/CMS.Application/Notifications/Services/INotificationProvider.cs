namespace CMS.Application.Notifications.Interfaces
{
    public interface INotificationProvider
    {
        Task<bool> SendAsync(string recipient, string subject, string body, string? recipientName = null);
        Task<bool> SendHtmlAsync(string recipient, string subject, string htmlBody, string? recipientName = null);
    }
}
