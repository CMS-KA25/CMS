using CMS.Application.Notifications.Services;
using CMS.Application.Notifications.Interfaces;
using CMS.Domain.NotificationModels;
using CMS.Domain.NotificationModels.Enums;

namespace CMS.Infrastructure.Notifications.NotificationServices
{
    public class NotificationScheduler : INotificationScheduler
    {
        private readonly INotificationQueueRepository _queueRepository;
        private readonly INotificationSender _notificationSender;

        public NotificationScheduler(
            INotificationQueueRepository queueRepository,
            INotificationSender notificationSender)
        {
            _queueRepository = queueRepository;
            _notificationSender = notificationSender;
        }

        public async Task<Guid> ScheduleNotificationAsync(Notification notification, DateTime scheduledFor)
        {
            try
            {
                var queueItem = new NotificationQueue
                {
                    NotificationId = notification.Id,
                    Priority = notification.Priority,
                    ScheduledFor = scheduledFor,
                    Status = NotificationStatus.Pending
                };

                var createdQueue = await _queueRepository.CreateAsync(queueItem);
                
                Console.WriteLine($"Notification scheduled for {scheduledFor}: {notification.Title}");
                
                return createdQueue.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scheduling notification: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> CancelScheduledNotificationAsync(Guid notificationId)
        {
            try
            {
                var queueItems = await _queueRepository.GetByNotificationIdAsync(notificationId);
                var pendingItems = queueItems.Where(q => q.Status == NotificationStatus.Pending).ToList();

                foreach (var item in pendingItems)
                {
                    await _queueRepository.UpdateStatusAsync(item.Id, NotificationStatus.Cancelled);
                }
                
                Console.WriteLine($"Scheduled notification cancelled: {notificationId}");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cancelling scheduled notification: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RescheduleNotificationAsync(Guid notificationId, DateTime newScheduledFor)
        {
            try
            {
                var queueItems = await _queueRepository.GetByNotificationIdAsync(notificationId);
                var pendingItems = queueItems.Where(q => q.Status == NotificationStatus.Pending).ToList();

                foreach (var item in pendingItems)
                {
                    item.ScheduledFor = newScheduledFor;
                    item.Status = NotificationStatus.Pending;
                    item.ProcessedAt = null;
                    await _queueRepository.UpdateAsync(item);
                }
                
                Console.WriteLine($"Notification rescheduled to {newScheduledFor}: {notificationId}");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rescheduling notification: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Notification>> GetScheduledNotificationsAsync(DateTime from, DateTime to)
        {
            try
            {
                var queueItems = await _queueRepository.GetScheduledAsync(from, to);
                var notifications = new List<Notification>();

                foreach (var queueItem in queueItems)
                {
                    if (queueItem.Notification != null)
                    {
                        notifications.Add(queueItem.Notification);
                    }
                }

                return notifications;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting scheduled notifications: {ex.Message}");
                return new List<Notification>();
            }
        }

        public async Task<List<Notification>> ProcessScheduledNotificationsAsync()
        {
            var processedNotifications = new List<Notification>();
            var pendingItems = await _queueRepository.GetPendingAsync(100);

            foreach (var queueItem in pendingItems)
            {
                try
                {
                    // Mark as processing
                    await _queueRepository.UpdateStatusAsync(queueItem.Id, NotificationStatus.Processing);
                    await _queueRepository.IncrementAttemptCountAsync(queueItem.Id);

                    // Get the notification
                    var notification = await _notificationSender.GetNotificationByIdAsync(queueItem.NotificationId);
                    if (notification == null)
                    {
                        await _queueRepository.UpdateStatusAsync(queueItem.Id, NotificationStatus.Failed, "Notification not found");
                        continue;
                    }

                    // Send the notification
                    await _notificationSender.SendNotificationAsync(notification);
                    
                    // Mark as sent
                    await _queueRepository.UpdateStatusAsync(queueItem.Id, NotificationStatus.Sent);
                    processedNotifications.Add(notification);
                    
                    Console.WriteLine($"Scheduled notification sent: {notification.Title}");
                }
                catch (Exception ex)
                {
                    if (queueItem.AttemptCount >= queueItem.MaxAttempts)
                    {
                        await _queueRepository.UpdateStatusAsync(queueItem.Id, NotificationStatus.Failed, ex.Message);
                        Console.WriteLine($"Scheduled notification failed after {queueItem.MaxAttempts} attempts: {ex.Message}");
                    }
                    else
                    {
                        // Reschedule for retry (exponential backoff)
                        var retryDelay = TimeSpan.FromMinutes(Math.Pow(2, queueItem.AttemptCount - 1));
                        queueItem.ScheduledFor = DateTime.UtcNow.Add(retryDelay);
                        queueItem.Status = NotificationStatus.Pending;
                        await _queueRepository.UpdateAsync(queueItem);
                        Console.WriteLine($"Scheduled notification will retry in {retryDelay.TotalMinutes} minutes: {ex.Message}");
                    }
                }
            }

            return processedNotifications;
        }

        public async Task<bool> IsNotificationScheduledAsync(Guid notificationId)
        {
            return await _queueRepository.ExistsAsync(notificationId);
        }

        public async Task<List<NotificationQueue>> GetQueuedNotificationsAsync(int maxCount = 100)
        {
            return await _queueRepository.GetPendingAsync(maxCount);
        }

        public async Task<bool> UpdateQueueStatusAsync(Guid queueId, NotificationStatus status, string? errorMessage = null)
        {
            return await _queueRepository.UpdateStatusAsync(queueId, status, errorMessage);
        }
    }
}
