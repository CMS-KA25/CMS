using System;

namespace CMS.Domain.Auth.Entities
{
    public class VerificationCode
    {
        public Guid Id { get; set; }
        public Guid? UserID { get; set; }
        public string Code { get; set; }
        public string Purpose { get; set; } // e.g., "Signup", "ForgotPassword"
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
