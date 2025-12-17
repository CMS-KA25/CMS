namespace CMS.Application.Auth.DTOs.Responses
{
    public class InviteUserResponse
    {
        public string Message { get; set; } = string.Empty;
        public UserResponse User { get; set; } = null!;
        public string? InvitationLink { get; set; }
    }
}

