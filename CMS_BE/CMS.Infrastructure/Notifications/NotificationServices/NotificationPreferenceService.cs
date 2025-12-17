using CMS.Application.Notifications.Services;
using CMS.Application.Notifications.Interfaces;
using CMS.Domain.NotificationModels;
using CMS.Domain.NotificationModels.Enums;
using CMS.Domain.Auth.Enums;

namespace CMS.Infrastructure.Notifications.NotificationServices
{
    public class NotificationPreferenceService : INotificationPreferenceService
    {
        private readonly INotificationPreferenceRepository _preferenceRepository;

        public NotificationPreferenceService(INotificationPreferenceRepository preferenceRepository)
        {
            _preferenceRepository = preferenceRepository;
        }

        public async Task<NotificationPreference?> GetPreferenceAsync(Guid userId, NotificationType type, NotificationChannelType channelType)
        {
            return await _preferenceRepository.GetByUserTypeAndChannelAsync(userId, type, channelType);
        }

        public async Task<List<NotificationPreference>> GetUserPreferencesAsync(Guid userId)
        {
            return await _preferenceRepository.GetByUserIdAsync(userId);
        }

        public async Task<bool> UpdatePreferenceAsync(NotificationPreference preference)
        {
            var existingPreference = await _preferenceRepository.GetByIdAsync(preference.Id);
            if (existingPreference == null)
                return false;

            existingPreference.IsEnabled = preference.IsEnabled;
            existingPreference.CustomSettings = preference.CustomSettings;
            existingPreference.UpdatedAt = DateTime.UtcNow;

            await _preferenceRepository.UpdateAsync(existingPreference);
            return true;
        }

        public async Task<bool> CreatePreferenceAsync(NotificationPreference preference)
        {
            preference.Id = Guid.NewGuid();
            preference.CreatedAt = DateTime.UtcNow;
            await _preferenceRepository.CreateAsync(preference);
            return true;
        }

        public async Task<bool> DeletePreferenceAsync(Guid preferenceId)
        {
            return await _preferenceRepository.DeleteAsync(preferenceId);
        }

        public async Task<bool> IsNotificationAllowedAsync(Guid userId, NotificationType type, NotificationChannelType channelType)
        {
            var preference = await GetPreferenceAsync(userId, type, channelType);
            
            if (preference == null)
            {
                // If no specific preference exists, use default based on user role and notification type
                return await GetDefaultPreferenceAsync(userId, type, channelType);
            }

            return preference.IsEnabled;
        }

        public async Task<List<NotificationChannelType>> GetAllowedChannelsAsync(Guid userId, NotificationType type)
        {
            var allowedChannels = new List<NotificationChannelType>();
            var channelTypes = Enum.GetValues<NotificationChannelType>();

            foreach (var channelType in channelTypes)
            {
                if (await IsNotificationAllowedAsync(userId, type, channelType))
                {
                    allowedChannels.Add(channelType);
                }
            }

            return allowedChannels;
        }

        public async Task<List<NotificationPreference>> GetPreferencesByUserRoleAsync(RoleType userRole)
        {
            return await _preferenceRepository.GetByUserRoleAsync(userRole);
        }

        private async Task<bool> GetDefaultPreferenceAsync(Guid userId, NotificationType type, NotificationChannelType channelType)
        {
            // Get user role (this would typically come from user service)
            var userRole = await GetUserRoleAsync(userId);
            
            // Default preferences based on user role and notification type
            return GetDefaultPreferenceForRole(userRole, type, channelType);
        }

        private async Task<RoleType> GetUserRoleAsync(Guid userId)
        {
            // This would typically query a user service
            // For demo purposes, return Patient as default
            return await Task.FromResult(RoleType.User);
        }

        private bool GetDefaultPreferenceForRole(RoleType userRole, NotificationType type, NotificationChannelType channelType)
        {
            // Critical notifications are always enabled
            if (IsCriticalNotification(type))
                return true;

            // Emergency notifications are always enabled
            if (type == NotificationType.EmergencyAlert)
                return true;

            // Default preferences by role
            return userRole switch
            {
                RoleType.User => GetPatientDefaultPreference(type, channelType),
                RoleType.Doctor => GetDoctorDefaultPreference(type, channelType),
                RoleType.Staff => GetStaffDefaultPreference(type, channelType),
                RoleType.Admin => true, // Admins get all notifications
                _ => false
            };
        }

        private bool GetPatientDefaultPreference(NotificationType type, NotificationChannelType channelType)
        {
            return type switch
            {
                NotificationType.AppointmentScheduled => channelType == NotificationChannelType.Email || channelType == NotificationChannelType.SMS,
                NotificationType.AppointmentReminder => channelType == NotificationChannelType.Email || channelType == NotificationChannelType.SMS || channelType == NotificationChannelType.Push,
                NotificationType.PaymentReminder => channelType == NotificationChannelType.Email || channelType == NotificationChannelType.SMS,
                NotificationType.LabResultReady => channelType == NotificationChannelType.Email || channelType == NotificationChannelType.InApp,
                NotificationType.PrescriptionReady => channelType == NotificationChannelType.Email || channelType == NotificationChannelType.InApp,
                NotificationType.Welcome => channelType == NotificationChannelType.Email,
                _ => channelType == NotificationChannelType.InApp
            };
        }

        private bool GetDoctorDefaultPreference(NotificationType type, NotificationChannelType channelType)
        {
            return type switch
            {
                NotificationType.EmergencyAlert => true, // All channels for emergencies
                NotificationType.AppointmentScheduled => channelType == NotificationChannelType.Email || channelType == NotificationChannelType.InApp,
                NotificationType.LabResultReady => channelType == NotificationChannelType.Email || channelType == NotificationChannelType.InApp,
                _ => channelType == NotificationChannelType.InApp
            };
        }

        private bool GetStaffDefaultPreference(NotificationType type, NotificationChannelType channelType)
        {
            return type switch
            {
                NotificationType.EmergencyAlert => true, // All channels for emergencies
                NotificationType.AppointmentScheduled => channelType == NotificationChannelType.Email || channelType == NotificationChannelType.InApp,
                NotificationType.PaymentReminder => channelType == NotificationChannelType.Email || channelType == NotificationChannelType.InApp,
                _ => channelType == NotificationChannelType.InApp
            };
        }

        private bool IsCriticalNotification(NotificationType type)
        {
            return type switch
            {
                NotificationType.EmergencyAlert => true,
                NotificationType.SecurityAlert => true,
                NotificationType.SystemMaintenance => true,
                _ => false
            };
        }
    }
}
