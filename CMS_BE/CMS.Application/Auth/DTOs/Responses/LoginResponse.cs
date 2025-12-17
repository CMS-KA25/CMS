namespace CMS.Application.Auth.DTOs.Responses
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public UserResponse User { get; set; } = null!;
    }
}

