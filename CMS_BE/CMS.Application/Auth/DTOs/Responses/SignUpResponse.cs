namespace CMS.Application.Auth.DTOs.Responses
{
    public class SignUpResponse
    {
        public string Message { get; set; } = string.Empty;
        public UserResponse User { get; set; } = null!;
    }
}

