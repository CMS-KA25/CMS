using CMS.Domain.NotificationModels;
using CMS.Domain.NotificationModels.Enums;
using CMS.Application.Notifications.Interfaces;
using CMS.Application.Notifications.Services;
using CMS.Infrastructure.Notifications.NotificationServices;
using CMS.Infrastructure.Notifications.Repositories;
using Microsoft.Extensions.Logging;

namespace CMS.Infrastructure.Notifications.NotificationServices
{
    public class NotificationService : ITemplateNotificationService
    {
        private readonly INotificationTemplateService _templateService;
        private readonly INotificationProvider _emailService;
        private readonly INotificationProvider _smsService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationTemplateService templateService,
            INotificationProvider emailService,
            INotificationProvider smsService,
            ILogger<NotificationService> logger)
        {
            _templateService = templateService;
            _emailService = emailService;
            _smsService = smsService;
            _logger = logger;
        }

        public async Task<bool> SendNotificationAsync(Guid templateId, string recipient, string recipientName, Dictionary<string, object>? variables = null)
        {
            try
            {
                var template = await _templateService.GetTemplateByIdAsync(templateId);
                if (template == null)
                {
                    _logger.LogError("Template not found with ID {TemplateId}", templateId);
                    return false;
                }

                if (!template.IsActive)
                {
                    _logger.LogWarning("Template {TemplateId} is not active", templateId);
                    return false;
                }

                var processedBody = await _templateService.ProcessTemplateAsync(template, variables ?? new Dictionary<string, object>());
                var processedSubject = ProcessSubject(template.Subject, variables ?? new Dictionary<string, object>());

                bool success = false;

                switch (template.ChannelType)
                {
                    case NotificationChannelType.Email:
                        success = await _emailService.SendAsync(recipient, processedSubject, processedBody, recipientName);
                        break;
                    case NotificationChannelType.SMS:
                        success = await _smsService.SendAsync(recipient, processedSubject, processedBody, recipientName);
                        break;
                    default:
                        _logger.LogWarning("Unsupported channel type {ChannelType} for template {TemplateId}", template.ChannelType, templateId);
                        return false;
                }

                if (success)
                {
                    _logger.LogInformation("Notification sent successfully using template {TemplateId} to {Recipient}", templateId, recipient);
                }
                else
                {
                    _logger.LogError("Failed to send notification using template {TemplateId} to {Recipient}", templateId, recipient);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification using template {TemplateId} to {Recipient}", templateId, recipient);
                return false;
            }
        }

        public async Task<bool> SendNotificationByTypeAsync(NotificationType type, NotificationChannelType channelType, string recipient, string recipientName, Dictionary<string, object>? variables = null)
        {
            try
            {
                var template = await _templateService.GetTemplateAsync(type, channelType);
                if (template == null)
                {
                    _logger.LogError("No template found for type {Type} and channel {Channel}", type, channelType);
                    return false;
                }

                return await SendNotificationAsync(template.Id, recipient, recipientName, variables);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification by type {Type} to {Recipient}", type, recipient);
                return false;
            }
        }

        private string ProcessSubject(string subject, Dictionary<string, object> variables)
        {
            var processedSubject = subject;
            
            foreach (var variable in variables)
            {
                var placeholder = $"{{{variable.Key}}}";
                var value = variable.Value?.ToString() ?? string.Empty;
                processedSubject = processedSubject.Replace(placeholder, value);
            }

            return processedSubject;
        }
    }
}