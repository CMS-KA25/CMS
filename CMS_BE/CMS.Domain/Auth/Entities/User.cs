using CMS.Domain.Auth.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Auth.Entities
{
    public class User
    {
        [Required]
        public Guid UserID { get; set; }
        public string? GoogleID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public long PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        [Url]
        public string? ProfilePictureURL { get; set; }
        [Required]
        public RoleType Role { get; set; } = RoleType.User;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
    }
}

