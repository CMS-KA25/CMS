using Microsoft.EntityFrameworkCore;
using CMS.Application.Notifications.Interfaces;
using CMS.Domain.NotificationModels;
using CMS.Domain.NotificationModels.Enums;
using CMS.Data;

namespace CMS.Infrastructure.Notifications.Repositories
{
    public class NotificationTemplateRepository : INotificationTemplateRepository
    {
        private readonly CmsDbContext _context;

        public NotificationTemplateRepository(CmsDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationTemplate?> GetByIdAsync(Guid id)
        {
            return await _context.NotificationTemplates.FindAsync(id);
        }

        public async Task<NotificationTemplate?> GetTemplateByTypeAndChannelAsync(NotificationType type, NotificationChannelType channelType)
        {
            return await _context.NotificationTemplates
                .FirstOrDefaultAsync(t => t.Type == type && t.ChannelType == channelType && t.IsActive);
        }

        public async Task<List<NotificationTemplate>> GetTemplatesByCategoryAsync(NotificationCategory category)
        {
            return await _context.NotificationTemplates
                .Where(t => t.Category == category && t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<NotificationTemplate?> GetTemplateByNameAndTypeAsync(string name, NotificationType type)
        {
            return await _context.NotificationTemplates
                .FirstOrDefaultAsync(t => t.Name == name && t.Type == type);
        }

        public async Task<List<NotificationTemplate>> GetActiveTemplatesAsync()
        {
            return await _context.NotificationTemplates
                .Where(t => t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<List<NotificationTemplate>> GetAllAsync()
        {
            return await _context.NotificationTemplates
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task AddAsync(NotificationTemplate template)
        {
            _context.NotificationTemplates.Add(template);
        }

        public async Task UpdateAsync(NotificationTemplate template)
        {
            var existing = await _context.NotificationTemplates.FindAsync(template.Id);
            if (existing != null)
            {
                // Map updated fields from 'template' to 'existing'
                _context.Entry(existing).CurrentValues.SetValues(template);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var template = await _context.NotificationTemplates.FindAsync(id);
            if (template != null)
            {
                _context.NotificationTemplates.Remove(template);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
