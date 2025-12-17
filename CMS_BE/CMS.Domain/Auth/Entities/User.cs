using System.ComponentModel.DataAnnotations;
using CMS.Domain.Auth.Enums;

namespace CMS.Domain.Auth.Entities
{
    public class User
    {
        [Required]
        public Guid UserID { get; set; }
        public string? GoogleID { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Url]
        public string? ProfilePictureURL { get; set; }
        [Required]
        public RoleType Role { get; set; } = RoleType.User;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<User_Sessions> Sessions { get; set; } = new List<User_Sessions>();
    }
}

