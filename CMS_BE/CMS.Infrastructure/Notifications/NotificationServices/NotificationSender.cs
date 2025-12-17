using CMS.Application.Notifications.Services;
using CMS.Application.Notifications.Interfaces;
using CMS.Domain.NotificationModels;
using Microsoft.Extensions.Logging;

namespace CMS.Infrastructure.Notifications.NotificationServices
{
    public class NotificationSender : INotificationSender
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationTemplateService _templateService;
        private readonly ITemplateNotificationService _templateNotifier;
        private readonly ILogger<NotificationSender> _logger;

        public NotificationSender(
            INotificationRepository notificationRepository,
            INotificationTemplateService templateService,
            ITemplateNotificationService templateNotifier,
            ILogger<NotificationSender> logger)
        {
            _notificationRepository = notificationRepository;
            _templateService = templateService;
            _templateNotifier = templateNotifier;
            _logger = logger;
        }

        public async Task<Guid> SendNotificationAsync(Notification notification)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            // If notification has a template, use the template notifier to send
            if (notification.TemplateId.HasValue)
            {
                var template = await _templateService.GetTemplateByIdAsync(notification.TemplateId.Value);
                if (template == null)
                {
                    _logger.LogWarning("Template {TemplateId} not found for notification {NotificationId}", notification.TemplateId, notification.Id);
                    return notification.Id;
                }

                string recipient = template.ChannelType == Domain.NotificationModels.Enums.NotificationChannelType.SMS
                    ? (notification.RecipientPhone ?? string.Empty)
                    : (notification.RecipientEmail ?? string.Empty);

                try
                {
                    var variables = notification.Metadata ?? new Dictionary<string, object>();
                    var sent = await _templateNotifier.SendNotificationAsync(template.Id, recipient, string.Empty, variables);
                    if (sent)
                    {
                        notification.SentAt = DateTime.UtcNow;
                        notification.Status = Domain.NotificationModels.Enums.NotificationStatus.Sent;
                        await _notificationRepository.UpdateAsync(notification);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending templated notification {NotificationId}", notification.Id);
                    notification.ErrorMessage = ex.Message;
                    await _notificationRepository.UpdateAsync(notification);
                }

                return notification.Id;
            }

            // Fallback: mark as sent locally
            try
            {
                notification.SentAt = DateTime.UtcNow;
                notification.Status = Domain.NotificationModels.Enums.NotificationStatus.Sent;
                await _notificationRepository.UpdateAsync(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification status for {NotificationId}", notification.Id);
            }

            return notification.Id;
        }

        public async Task<Notification?> GetNotificationByIdAsync(Guid id)
        {
            return await _notificationRepository.GetByIdAsync(id);
        }
    }
}
