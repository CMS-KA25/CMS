namespace CMS.Domain.Auth.Entities
{
    public class UserSession
    {
        public Guid SessionID { get; set; }
        public Guid UserID { get; set; }
        public string SessionToken { get; set; }
        public string IPAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime LoginTimestamp { get; set; }
        public DateTime LastActivityTimestamp { get; set; }
        public DateTime ExpiryTimestamp { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LogoutTimestamp { get; set; }
        public string? LogoutReason { get; set; }

        public User? User { get; set; }
    }
}
