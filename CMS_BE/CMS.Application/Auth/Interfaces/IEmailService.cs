using System.Threading.Tasks;

namespace CMS.Application.Auth.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string plainTextContent, string htmlContent = null);
    }
}
