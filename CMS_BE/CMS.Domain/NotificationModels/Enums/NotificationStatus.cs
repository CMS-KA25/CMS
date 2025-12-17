namespace CMS.Domain.NotificationModels.Enums
{
    public enum NotificationStatus
    {
        Pending = 1,
        Processing = 2,
        Sent = 3,
        Delivered = 4,
        Failed = 5,
        Cancelled = 6,
        Read = 7,
        Unread = 8,
        Expired = 9
    }
}
