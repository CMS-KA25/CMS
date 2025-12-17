using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using CMS.Domain.NotificationModels.Configuration;
using Microsoft.Extensions.Options;
using CMS.Application.Notifications.Interfaces;
using Microsoft.Extensions.Logging;

namespace CMS.Infrastructure.Notifications.NotificationServices
{
    public class TwilioSmsService : INotificationProvider
    {
        private readonly TwilioConfig _config;
        private readonly ILogger<TwilioSmsService> _logger;

        public TwilioSmsService(IOptions<TwilioConfig> config, ILogger<TwilioSmsService> logger)
        {
            _config = config.Value;
            TwilioClient.Init(_config.AccountSid, _config.AuthToken);
            _logger = logger;
        }

        public async Task<bool> SendAsync(string recipient, string subject, string body, string? recipientName = null)
        {
            try
            {
                // Format phone number to international format if needed
                var formattedPhoneNumber = FormatPhoneNumber(recipient);
                
                _logger.LogInformation("Sending SMS to {OriginalNumber} -> {FormattedNumber}", recipient, formattedPhoneNumber);
                
                // For SMS, we ignore the subject and recipientName parameters
                var message = await MessageResource.CreateAsync(
                    body: body,
                    from: new PhoneNumber(_config.FromNumber),
                    to: new PhoneNumber(formattedPhoneNumber)
                );

                if (message.Status != MessageResource.StatusEnum.Failed)
                {
                    _logger.LogInformation("SMS sent successfully to {Recipient}. Message SID: {MessageSid}", recipient, message.Sid);
                    return true;
                }
                else
                {
                    _logger.LogError("Failed to send SMS to {Recipient}. Status: {Status}", recipient, message.Status);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SMS to {Recipient}", recipient);
                return false;
            }
        }

        public async Task<bool> SendHtmlAsync(string recipient, string subject, string htmlBody, string? recipientName = null)
        {
            // SMS doesn't support HTML, so we'll strip HTML tags and send as plain text
            var plainTextBody = System.Text.RegularExpressions.Regex.Replace(htmlBody, "<[^>]*>", "");
            return await SendAsync(recipient, subject, plainTextBody, recipientName);
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            // Remove all non-digit characters
            var digitsOnly = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");
            
            // If it already starts with country code, return as is
            if (digitsOnly.StartsWith("91") && digitsOnly.Length == 12)
            {
                return "+" + digitsOnly;
            }
            
            // If it's a 10-digit Indian number, add +91
            if (digitsOnly.Length == 10)
            {
                return "+91" + digitsOnly;
            }
            
            // If it's already in international format, return as is
            if (phoneNumber.StartsWith("+"))
            {
                return phoneNumber;
            }
            
            // Default: assume it's an Indian number and add +91
            return "+91" + digitsOnly;
        }
    }
}
