using CMS.Domain.Auth.Enums;

namespace CMS.Application.Auth.DTOs.Responses
{
    public class UserResponse
    {
        public Guid UserID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ProfilePictureURL { get; set; }
        public RoleType Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

