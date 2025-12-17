using SendGrid;
using SendGrid.Helpers.Mail;
using CMS.Domain.NotificationModels.Configuration;
using Microsoft.Extensions.Options;
using CMS.Application.Notifications.Interfaces;
using Microsoft.Extensions.Logging;

namespace CMS.Infrastructure.Notifications.NotificationServices
{
    public class SendGridEmailService : INotificationProvider
    {
        private readonly SendGridClient _client;
        private readonly SendGridConfig _config;
        private readonly ILogger<SendGridEmailService> _logger;

        public SendGridEmailService(IOptions<SendGridConfig> config, ILogger<SendGridEmailService> logger)
        {
            _config = config.Value;
            _client = new SendGridClient(_config.ApiKey);
            _logger = logger;
        }

        public async Task<bool> SendAsync(string recipient, string subject, string body, string? recipientName = null)
        {
            try
            {
                var from = new EmailAddress(_config.FromEmail, _config.FromName);
                var to = new EmailAddress(recipient, recipientName);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);
                
                var response = await _client.SendEmailAsync(msg);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email sent successfully to {Recipient}", recipient);
                    return true;
                }
                else
                {
                    _logger.LogError("Failed to send email to {Recipient}. Status: {StatusCode}", recipient, response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Recipient}", recipient);
                return false;
            }
        }

        public async Task<bool> SendHtmlAsync(string recipient, string subject, string htmlBody, string? recipientName = null)
        {
            try
            {
                var from = new EmailAddress(_config.FromEmail, _config.FromName);
                var to = new EmailAddress(recipient, recipientName);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlBody);
                
                var response = await _client.SendEmailAsync(msg);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("HTML email sent successfully to {Recipient}", recipient);
                    return true;
                }
                else
                {
                    _logger.LogError("Failed to send HTML email to {Recipient}. Status: {StatusCode}", recipient, response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending HTML email to {Recipient}", recipient);
                return false;
            }
        }
    }
}
