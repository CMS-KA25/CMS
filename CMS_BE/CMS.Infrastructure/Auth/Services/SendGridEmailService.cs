using CMS.Application.Auth.Interfaces;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace CMS.Infrastructure.Auth.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public SendGridEmailService(IConfiguration configuration)
        {
            _apiKey = configuration["SENDGRID_API_KEY"] ?? System.Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            _fromEmail = configuration["SENDGRID_FROM_EMAIL"] ?? "no-reply@yourdomain.com";
            _fromName = configuration["SENDGRID_FROM_NAME"] ?? "CMS";
        }

        public async Task SendEmailAsync(string toEmail, string subject, string plainTextContent, string htmlContent = null)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new System.InvalidOperationException("SendGrid API key is not configured. Set SENDGRID_API_KEY.");
            }

            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            // Optionally check response.StatusCode
        }
    }
}
