using CMS.Domain.NotificationModels;
using CMS.Domain.NotificationModels.Enums;
using CMS.Application.Notifications.Interfaces;
using CMS.Application.Notifications.Services;
using CMS.Infrastructure.Notifications.Repositories;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace CMS.Infrastructure.Notifications.NotificationServices
{
    public class NotificationTemplateService : INotificationTemplateService
    {
        private readonly INotificationTemplateRepository _repository;
        private readonly ILogger<NotificationTemplateService> _logger;

        public NotificationTemplateService(INotificationTemplateRepository repository, ILogger<NotificationTemplateService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<NotificationTemplate?> GetTemplateAsync(NotificationType type, NotificationChannelType channelType)
        {
            try
            {
                return await _repository.GetTemplateByTypeAndChannelAsync(type, channelType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting template for type {Type} and channel {Channel}", type, channelType);
                return null;
            }
        }

        public async Task<NotificationTemplate?> GetTemplateByIdAsync(Guid id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting template by ID {Id}", id);
                return null;
            }
        }

        public async Task<List<NotificationTemplate>> GetTemplatesByCategoryAsync(NotificationCategory category)
        {
            try
            {
                return await _repository.GetTemplatesByCategoryAsync(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting templates by category {Category}", category);
                return new List<NotificationTemplate>();
            }
        }

        public async Task<Guid> CreateTemplateAsync(NotificationTemplate template)
        {
            try
            {
                // Validate template name uniqueness per trigger event
                var existingTemplate = await _repository.GetTemplateByNameAndTypeAsync(template.Name, template.Type);
                if (existingTemplate != null)
                {
                    throw new InvalidOperationException($"Template with name '{template.Name}' already exists for notification type '{template.Type}'");
                }

                template.Id = Guid.NewGuid();
                template.CreatedAt = DateTime.UtcNow;
                
                await _repository.AddAsync(template);
                await _repository.SaveChangesAsync();
                
                _logger.LogInformation("Template created successfully with ID {Id}", template.Id);
                return template.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating template");
                throw;
            }
        }

        public async Task<bool> UpdateTemplateAsync(NotificationTemplate template)
        {
            try
            {
                var existingTemplate = await _repository.GetByIdAsync(template.Id);
                if (existingTemplate == null)
                {
                    return false;
                }

                // Check for name uniqueness if name changed
                if (existingTemplate.Name != template.Name)
                {
                    var duplicateTemplate = await _repository.GetTemplateByNameAndTypeAsync(template.Name, template.Type);
                    if (duplicateTemplate != null && duplicateTemplate.Id != template.Id)
                    {
                        throw new InvalidOperationException($"Template with name '{template.Name}' already exists for notification type '{template.Type}'");
                    }
                }

                template.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateAsync(template);
                await _repository.SaveChangesAsync();
                
                _logger.LogInformation("Template updated successfully with ID {Id}", template.Id);
                return true;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating template with ID {Id}", template.Id);
                return false;
            }
        }

        public async Task<bool> DeleteTemplateAsync(Guid id)
        {
            try
            {
                var template = await _repository.GetByIdAsync(id);
                if (template == null)
                {
                    return false;
                }

                await _repository.DeleteAsync(id);
                await _repository.SaveChangesAsync();
                
                _logger.LogInformation("Template deleted successfully with ID {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting template with ID {Id}", id);
                return false;
            }
        }

        public async Task<string> ProcessTemplateAsync(NotificationTemplate template, Dictionary<string, object> variables)
        {
            try
            {
                var processedBody = template.Body;
                var processedSubject = template.Subject;

                // Replace variables in the template
                foreach (var variable in variables)
                {
                    var placeholder = $"{{{variable.Key.Trim()}}}";

                    var value = variable.Value?.ToString() ?? string.Empty;
                    
                    processedBody = processedBody.Replace(placeholder, value);
                    processedSubject = processedSubject.Replace(placeholder, value);
                }
                
                // Validate that all placeholders have been replaced
                var remainingPlaceholders = Regex.Matches(processedBody, @"\{[^}]+\}").Cast<Match>().Select(m => m.Value).ToList();
                remainingPlaceholders.AddRange(Regex.Matches(processedSubject, @"\{[^}]+\}").Cast<Match>().Select(m => m.Value));

                if (remainingPlaceholders.Any())
                {
                    _logger.LogWarning("Unprocessed placeholders found in template {TemplateId}: {Placeholders}", template.Id, string.Join(", ", remainingPlaceholders));
                }

                return processedBody;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing template {TemplateId}", template.Id);
                throw;
            }
        }

        public async Task<List<NotificationTemplate>> GetAllTemplatesAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all templates");
                return new List<NotificationTemplate>();
            }
        }

        public async Task<List<NotificationTemplate>> GetActiveTemplatesAsync()
        {
            try
            {
                return await _repository.GetActiveTemplatesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active templates");
                return new List<NotificationTemplate>();
            }
        }
    }
}