namespace CMS.Application.Auth.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(Guid userId, string action, string tableName, bool success = true, string? errorMessage = null);
        Task LogAsync(string action, string tableName, bool success = true, string? errorMessage = null);
    }
}