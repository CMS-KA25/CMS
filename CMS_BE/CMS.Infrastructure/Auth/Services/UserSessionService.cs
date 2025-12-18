using CMS.Application.Auth.Interfaces;
using CMS.Domain.Auth.Entities;
using CMS.Data;
using Microsoft.EntityFrameworkCore;

namespace CMS.Infrastructure.Auth.Services
{
    public class UserSessionService : IUserSessionService
    {
        private readonly CmsDbContext _context;

        public UserSessionService(CmsDbContext context)
        {
            _context = context;
        }

        public async Task CreateSessionAsync(Guid userId, string sessionToken, string ipAddress, string? userAgent = null)
        {
            var session = new UserSession
            {
                SessionID = Guid.NewGuid(),
                UserID = userId,
                SessionToken = sessionToken,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                LoginTimestamp = DateTime.UtcNow,
                LastActivityTimestamp = DateTime.UtcNow,
                ExpiryTimestamp = DateTime.UtcNow.AddHours(1), // Match JWT expiry
                IsActive = true
            };

            _context.UserSessions.Add(session);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateActivityAsync(string sessionToken)
        {
            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.SessionToken == sessionToken && s.IsActive);
            
            if (session != null)
            {
                session.LastActivityTimestamp = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task LogoutSessionAsync(string sessionToken, string reason = "Manual")
        {
            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.SessionToken == sessionToken && s.IsActive);
            
            if (session != null)
            {
                session.IsActive = false;
                session.LogoutTimestamp = DateTime.UtcNow;
                session.LogoutReason = reason;
                await _context.SaveChangesAsync();
            }
        }

        public async Task CleanupExpiredSessionsAsync()
        {
            var expiredSessions = await _context.UserSessions
                .Where(s => s.IsActive && s.ExpiryTimestamp < DateTime.UtcNow)
                .ToListAsync();

            foreach (var session in expiredSessions)
            {
                session.IsActive = false;
                session.LogoutTimestamp = DateTime.UtcNow;
                session.LogoutReason = "Expired";
            }

            if (expiredSessions.Any())
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}