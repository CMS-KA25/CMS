using CMS.Domain.Auth.Enums;
using System.ComponentModel.DataAnnotations;

namespace CMS.Application.Auth.DTOs.Requests
{
    public class ResendInviteRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public RoleType? Role { get; set; }
    }
}
