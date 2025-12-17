namespace CMS.Domain.NotificationModels.Configuration
{
    public class SendGridConfig
    {
        public string ApiKey { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = "Clinic Management System";
    }
}
