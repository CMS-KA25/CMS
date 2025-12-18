using CMS.Application.Auth.Interfaces;
using CMS.Domain.Auth.Entities;
using CMS.Data;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CMS.Infrastructure.Auth.Services
{
    public class AuditService : IAuditService
    {
        private readonly CmsDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(CmsDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(Guid userId, string action, string tableName, bool success = true, string? errorMessage = null)
        {
            var audit = new AuditLog
            {
                AuditID = Guid.NewGuid(),
                UserID = userId,
                Action = action,
                TableName = tableName,
                ActionResult = success,
                ErrorMessage = errorMessage,
                ActionTimestamp = DateTime.UtcNow,
                CorrelationId = _httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString(),
                IPAddress = GetClientIP()
            };

            _context.AuditLogs.Add(audit);
            await _context.SaveChangesAsync();
        }

        public async Task LogAsync(string action, string tableName, bool success = true, string? errorMessage = null)
        {
            var userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                await LogAsync(userId.Value, action, tableName, success, errorMessage);
            }
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim?.Value, out var userId) ? userId : null;
        }

        private string? GetClientIP()
        {
            return _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        }
    }
}