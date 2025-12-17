namespace CMS.Application.Auth.DTOs.Requests
{
    public class AcceptInviteRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
