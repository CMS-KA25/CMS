using System;

namespace CMS.Domain.Auth.Entities
{
    public class Invitation
    {
        public Guid InvitationId { get; set; }
        public Guid? UserID { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsAccepted { get; set; }
        public Guid? InvitedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
