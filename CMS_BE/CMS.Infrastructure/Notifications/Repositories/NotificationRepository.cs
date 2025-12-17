using Microsoft.EntityFrameworkCore;
using CMS.Application.Notifications.Interfaces;
using CMS.Domain.NotificationModels;
using CMS.Domain.NotificationModels.Enums;
using CMS.Data;

namespace CMS.Infrastructure.Notifications.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly CmsDbContext _context;

        public NotificationRepository(CmsDbContext context)
        {
            _context = context;
        }

        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications
                .Include(n => n.Channels)
                .Include(n => n.Template)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<List<Notification>> GetByUserIdAsync(Guid userId, int page = 1, int pageSize = 20)
        {
            return await _context.Notifications
                .Where(n => n.RecipientId == userId)
                .Include(n => n.Channels)
                .Include(n => n.Template)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetUnreadByUserIdAsync(Guid userId)
        {
            return await _context.Notifications
                .Where(n => n.RecipientId == userId && n.ReadAt == null)
                .Include(n => n.Channels)
                .Include(n => n.Template)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.RecipientId == userId && n.ReadAt == null);
        }

        public async Task<List<Notification>> GetByTypeAsync(NotificationType type, int page = 1, int pageSize = 20)
        {
            return await _context.Notifications
                .Where(n => n.Type == type)
                .Include(n => n.Channels)
                .Include(n => n.Template)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetByStatusAsync(NotificationStatus status, int page = 1, int pageSize = 20)
        {
            return await _context.Notifications
                .Where(n => n.Status == status)
                .Include(n => n.Channels)
                .Include(n => n.Template)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Notification> CreateAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<Notification> UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return false;

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAsReadAsync(Guid id, Guid userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.RecipientId == userId);
            
            if (notification == null)
                return false;

            notification.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Notification>> GetScheduledNotificationsAsync(DateTime from, DateTime to)
        {
            return await _context.Notifications
                .Where(n => n.ScheduledFor >= from && n.ScheduledFor <= to && n.Status == NotificationStatus.Pending)
                .Include(n => n.Channels)
                .Include(n => n.Template)
                .OrderBy(n => n.ScheduledFor)
                .ToListAsync();
        }
    }


}
