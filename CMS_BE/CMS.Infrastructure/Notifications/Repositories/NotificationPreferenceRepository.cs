using Microsoft.EntityFrameworkCore;
using CMS.Application.Notifications.Interfaces;
using CMS.Domain.NotificationModels;
using CMS.Domain.NotificationModels.Enums;
using CMS.Domain.Auth.Enums;
using CMS.Data;

namespace CMS.Infrastructure.Notifications.Repositories
{
    public class NotificationPreferenceRepository : INotificationPreferenceRepository
    {
        private readonly CmsDbContext _context;

        public NotificationPreferenceRepository(CmsDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationPreference?> GetByIdAsync(Guid id)
        {
            return await _context.NotificationPreferences.FindAsync(id);
        }

        public async Task<NotificationPreference?> GetByUserTypeAndChannelAsync(Guid userId, NotificationType type, NotificationChannelType channelType)
        {
            return await _context.NotificationPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Type == type && p.ChannelType == channelType);
        }

        public async Task<List<NotificationPreference>> GetByUserIdAsync(Guid userId)
        {
            return await _context.NotificationPreferences
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.Type)
                .ThenBy(p => p.ChannelType)
                .ToListAsync();
        }

        public async Task<List<NotificationPreference>> GetByUserRoleAsync(RoleType userRole)
        {
            return await _context.NotificationPreferences
                .Where(p => p.UserRole == userRole)
                .OrderBy(p => p.Type)
                .ThenBy(p => p.ChannelType)
                .ToListAsync();
        }

        public async Task<NotificationPreference> CreateAsync(NotificationPreference preference)
        {
            _context.NotificationPreferences.Add(preference);
            await _context.SaveChangesAsync();
            return preference;
        }

        public async Task<NotificationPreference> UpdateAsync(NotificationPreference preference)
        {
            _context.NotificationPreferences.Update(preference);
            await _context.SaveChangesAsync();
            return preference;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var preference = await _context.NotificationPreferences.FindAsync(id);
            if (preference == null)
                return false;

            _context.NotificationPreferences.Remove(preference);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid userId, NotificationType type, NotificationChannelType channelType)
        {
            return await _context.NotificationPreferences
                .AnyAsync(p => p.UserId == userId && p.Type == type && p.ChannelType == channelType);
        }
    }


}
