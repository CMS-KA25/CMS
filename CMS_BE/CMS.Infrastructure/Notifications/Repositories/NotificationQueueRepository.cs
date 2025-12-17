using Microsoft.EntityFrameworkCore;
using CMS.Application.Notifications.Interfaces;
using CMS.Domain.NotificationModels;
using CMS.Domain.NotificationModels.Enums;
using CMS.Data;

namespace CMS.Infrastructure.Notifications.Repositories
{
    public class NotificationQueueRepository : INotificationQueueRepository
    {
        private readonly CmsDbContext _context;

        public NotificationQueueRepository(CmsDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationQueue?> GetByIdAsync(Guid id)
        {
            return await _context.NotificationQueues
                .Include(q => q.Notification)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<List<NotificationQueue>> GetScheduledAsync(DateTime from, DateTime to)
        {
            return await _context.NotificationQueues
                .Where(q => q.ScheduledFor >= from && q.ScheduledFor <= to && q.Status == NotificationStatus.Pending)
                .Include(q => q.Notification)
                .OrderBy(q => q.Priority)
                .ThenBy(q => q.ScheduledFor)
                .ToListAsync();
        }

        public async Task<List<NotificationQueue>> GetPendingAsync(int maxCount = 100)
        {
            return await _context.NotificationQueues
                .Where(q => q.Status == NotificationStatus.Pending && q.ScheduledFor <= DateTime.UtcNow)
                .Include(q => q.Notification)
                .OrderBy(q => q.Priority)
                .ThenBy(q => q.ScheduledFor)
                .Take(maxCount)
                .ToListAsync();
        }

        public async Task<List<NotificationQueue>> GetByNotificationIdAsync(Guid notificationId)
        {
            return await _context.NotificationQueues
                .Where(q => q.NotificationId == notificationId)
                .Include(q => q.Notification)
                .OrderBy(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<NotificationQueue> CreateAsync(NotificationQueue queue)
        {
            _context.NotificationQueues.Add(queue);
            await _context.SaveChangesAsync();
            return queue;
        }

        public async Task<NotificationQueue> UpdateAsync(NotificationQueue queue)
        {
            _context.NotificationQueues.Update(queue);
            await _context.SaveChangesAsync();
            return queue;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var queue = await _context.NotificationQueues.FindAsync(id);
            if (queue == null)
                return false;

            _context.NotificationQueues.Remove(queue);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusAsync(Guid id, NotificationStatus status, string? errorMessage = null)
        {
            var queue = await _context.NotificationQueues.FindAsync(id);
            if (queue == null)
                return false;

            queue.Status = status;
            queue.ProcessedAt = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                queue.ErrorMessage = errorMessage;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IncrementAttemptCountAsync(Guid id)
        {
            var queue = await _context.NotificationQueues.FindAsync(id);
            if (queue == null)
                return false;

            queue.AttemptCount++;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid notificationId)
        {
            return await _context.NotificationQueues
                .AnyAsync(q => q.NotificationId == notificationId && q.Status == NotificationStatus.Pending);
        }
    }


}
