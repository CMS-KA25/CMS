namespace CMS.Application.Auth.Interfaces
{
    public interface IUserSessionService
    {
        Task CreateSessionAsync(Guid userId, string sessionToken, string ipAddress, string? userAgent = null);
        Task UpdateActivityAsync(string sessionToken);
        Task LogoutSessionAsync(string sessionToken, string reason = "Manual");
        Task CleanupExpiredSessionsAsync();
    }
}