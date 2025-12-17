using Microsoft.AspNetCore.Mvc;
using CMS.Application.Notifications.Services;
using CMS.Domain.NotificationModels;
using CMS.Domain.NotificationModels.Enums;
using CMS.Application.Notifications.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace CMS.Api.Controllers.Notifications
{
    [ApiController]
    [Route("api/[controller]")]
   
    public class NotificationTemplateController : ControllerBase
    {
        private readonly INotificationTemplateService _templateService;
        private readonly ITemplateNotificationService _notificationService;
        private readonly ILogger<NotificationTemplateController> _logger;

        public NotificationTemplateController(
            INotificationTemplateService templateService,
            ITemplateNotificationService notificationService,
            ILogger<NotificationTemplateController> logger)
        {
            _templateService = templateService;
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Get all notification templates
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<NotificationTemplateResponseDto>>> GetAllTemplates()
        {
            try
            {
                var templates = await _templateService.GetAllTemplatesAsync();
                var response = templates.Select(MapToResponseDto).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all templates");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get active notification templates only
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<List<NotificationTemplateResponseDto>>> GetActiveTemplates()
        {
            try
            {
                var templates = await _templateService.GetActiveTemplatesAsync();
                var response = templates.Select(MapToResponseDto).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active templates");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get templates by category
        /// </summary>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<List<NotificationTemplateResponseDto>>> GetTemplatesByCategory(NotificationCategory category)
        {
            try
            {
                var templates = await _templateService.GetTemplatesByCategoryAsync(category);
                var response = templates.Select(MapToResponseDto).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting templates by category {Category}", category);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get template by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationTemplateResponseDto>> GetTemplate(Guid id)
        {
            try
            {
                var template = await _templateService.GetTemplateByIdAsync(id);
                if (template == null)
                {
                    return NotFound($"Template with ID {id} not found");
                }

                return Ok(MapToResponseDto(template));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting template {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create a new notification template
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateTemplate([FromBody] CreateNotificationTemplateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                var template = MapFromCreateDto(dto);
                var templateId = await _templateService.CreateTemplateAsync(template);

                _logger.LogInformation("Template created successfully with ID {TemplateId}", templateId);
                return new CreatedAtActionResult(nameof(GetTemplate), null, new { id = templateId }, templateId);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error creating template");
                return new BadRequestObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating template");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update an existing notification template
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemplate(Guid id, [FromBody] UpdateNotificationTemplateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                if (id != dto.Id)
                {
                    return new BadRequestObjectResult("Template ID mismatch");
                }

                var template = MapFromUpdateDto(dto);
                var success = await _templateService.UpdateTemplateAsync(template);

                if (!success)
                {
                    return new NotFoundObjectResult($"Template with ID {id} not found");
                }

                _logger.LogInformation("Template updated successfully with ID {TemplateId}", id);
                return new NoContentResult();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error updating template {Id}", id);
                return new BadRequestObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating template {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete a notification template
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTemplate(Guid id)
        {
            try
            {
                var success = await _templateService.DeleteTemplateAsync(id);
                if (!success)
                {
                    return new NotFoundObjectResult($"Template with ID {id} not found");
                }

                _logger.LogInformation("Template updated successfully with ID {TemplateId}", id);
                return new NoContentResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting template {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Send a test notification using a template
        /// </summary>
        [HttpPost("send-test")]
        public async Task<ActionResult> SendTestNotification([FromBody] SendNotificationDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Get the template to determine the channel type
                var template = await _templateService.GetTemplateByIdAsync(dto.TemplateId);
                if (template == null)
                {
                    return NotFound($"Template with ID {dto.TemplateId} not found");
                }

                // Use appropriate recipient based on channel type
                string recipient;
                if (template.ChannelType == NotificationChannelType.SMS)
                {
                    // For SMS templates a phone number is required. Do not fall back to email here.
                    if (string.IsNullOrWhiteSpace(dto.RecipientPhone))
                    {
                        return BadRequest("Recipient phone number is required for SMS templates");
                    }

                    recipient = dto.RecipientPhone;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(dto.RecipientEmail))
                    {
                        return BadRequest("Recipient email is required for non-SMS templates");
                    }

                    recipient = dto.RecipientEmail;
                }

                var success = await _notificationService.SendNotificationAsync(
                    dto.TemplateId,
                    recipient,
                    dto.RecipientName,
                    dto.Variables
                );

                if (success)
                {
                    _logger.LogInformation("Test notification sent successfully to {Recipient}", recipient);
                    return Ok(new { message = "Test notification sent successfully" });
                }
                else
                {
                    _logger.LogError("Failed to send test notification to {Recipient}", recipient);
                    return BadRequest("Failed to send test notification");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending test notification");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Send notification by type and channel
        /// </summary>
        [HttpPost("send-by-type")]
        public async Task<ActionResult> SendNotificationByType([FromBody] SendNotificationByTypeDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var recipient = dto.ChannelType == NotificationChannelType.Email ? dto.RecipientEmail : dto.RecipientPhone;
                if (string.IsNullOrEmpty(recipient))
                {
                    return BadRequest("Recipient information is required for the specified channel type");
                }

                var success = await _notificationService.SendNotificationByTypeAsync(
                    dto.Type,
                    dto.ChannelType,
                    recipient,
                    dto.RecipientName,
                    dto.Variables
                );

                if (success)
                {
                    _logger.LogInformation("Notification sent successfully to {Recipient}", recipient);
                    return Ok(new { message = "Notification sent successfully" });
                }
                else
                {
                    _logger.LogError("Failed to send notification to {Recipient}", recipient);
                    return BadRequest("Failed to send notification");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification by type");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Send SMS notification using template ID
        /// </summary>
        [HttpPost("send-sms")]
        public async Task<ActionResult> SendSmsNotification([FromBody] SendSmsNotificationDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Get the template to verify it's an SMS template
                var template = await _templateService.GetTemplateByIdAsync(dto.TemplateId);
                if (template == null)
                {
                    return NotFound($"Template with ID {dto.TemplateId} not found");
                }

                if (template.ChannelType != NotificationChannelType.SMS)
                {
                    return BadRequest($"Template {dto.TemplateId} is not an SMS template. Channel type: {template.ChannelType}");
                }

                var success = await _notificationService.SendNotificationAsync(
                    dto.TemplateId,
                    dto.RecipientPhone,
                    dto.RecipientName,
                    dto.Variables
                );

                if (success)
                {
                    _logger.LogInformation("SMS notification sent successfully to {RecipientPhone}", dto.RecipientPhone);
                    return Ok(new { message = "SMS notification sent successfully" });
                }
                else
                {
                    _logger.LogError("Failed to send SMS notification to {RecipientPhone}", dto.RecipientPhone);
                    return BadRequest("Failed to send SMS notification");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SMS notification");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Send SMS notification by notification type (finds SMS template automatically)
        /// </summary>
        [HttpPost("send-sms-by-type")]
        public async Task<ActionResult> SendSmsNotificationByType([FromBody] SendSmsByTypeDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _notificationService.SendNotificationByTypeAsync(
                    dto.Type,
                    NotificationChannelType.SMS,
                    dto.RecipientPhone,
                    dto.RecipientName,
                    dto.Variables
                );

                if (success)
                {
                    _logger.LogInformation("SMS notification sent successfully to {RecipientPhone}", dto.RecipientPhone);
                    return Ok(new { message = "SMS notification sent successfully" });
                }
                else
                {
                    _logger.LogError("Failed to send SMS notification to {RecipientPhone}", dto.RecipientPhone);
                    return BadRequest("Failed to send SMS notification");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SMS notification by type");
                return StatusCode(500, "Internal server error");
            }
        }

        private NotificationTemplateResponseDto MapToResponseDto(NotificationTemplate template)
        {
            return new NotificationTemplateResponseDto
            {
                Id = template.Id,
                Name = template.Name,
                Subject = template.Subject,
                Body = template.Body,
                Type = template.Type,
                ChannelType = template.ChannelType,
                Category = template.Category,
                Variables = template.Variables,
                Description = template.Description,
                IsActive = template.IsActive,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt,
                CreatedBy = template.CreatedBy,
                UpdatedBy = template.UpdatedBy
            };
        }

        private NotificationTemplate MapFromCreateDto(CreateNotificationTemplateDto dto)
        {
            return new NotificationTemplate
            {
                Name = dto.Name,
                Subject = dto.Subject,
                Body = dto.Body,
                Type = dto.Type,
                ChannelType = dto.ChannelType,
                Category = dto.Category,
                Variables = dto.Variables,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedBy = User.Identity?.Name ?? "System"
            };
        }

        private NotificationTemplate MapFromUpdateDto(UpdateNotificationTemplateDto dto)
        {
            return new NotificationTemplate
            {
                Id = dto.Id,
                Name = dto.Name,
                Subject = dto.Subject,
                Body = dto.Body,
                Type = dto.Type,
                ChannelType = dto.ChannelType,
                Category = dto.Category,
                Variables = dto.Variables,
                Description = dto.Description,
                IsActive = dto.IsActive,
                UpdatedBy = User.Identity?.Name ?? "System"
            };
        }
    }

    public class SendNotificationByTypeDto
    {
        public NotificationType Type { get; set; }
        public NotificationChannelType ChannelType { get; set; }
        public string RecipientEmail { get; set; } = string.Empty;
        public string? RecipientPhone { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public Dictionary<string, object>? Variables { get; set; }
    }

    public class SendSmsNotificationDto
    {
        public Guid TemplateId { get; set; }
        public string RecipientPhone { get; set; } = string.Empty;
        public string RecipientName { get; set; } = string.Empty;
        public Dictionary<string, object>? Variables { get; set; }
    }

    public class SendSmsByTypeDto
    {
        public NotificationType Type { get; set; }
        public string RecipientPhone { get; set; } = string.Empty;
        public string RecipientName { get; set; } = string.Empty;
        public Dictionary<string, object>? Variables { get; set; }
    }
}
