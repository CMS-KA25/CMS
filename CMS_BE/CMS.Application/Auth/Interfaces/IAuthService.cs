using CMS.Application.Auth.DTOs.Requests;
using CMS.Application.Auth.DTOs.Responses;

namespace CMS.Application.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<SignUpResponse> SignUpAsync(SignUpRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<InviteUserResponse> InviteUserAsync(InviteUserRequest request, Guid adminUserId);
        Task<InviteUserResponse> ResendInvitationAsync(string email, CMS.Domain.Auth.Enums.RoleType? role, Guid adminUserId);
        Task SendSignUpOtpAsync(string email);
        Task VerifySignUpOtpAsync(string email, string code);
        Task SendForgotPasswordOtpAsync(string email);
        Task ResetPasswordWithOtpAsync(string email, string code, string newPassword);
        Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task AcceptInvitationAsync(string token, string newPassword);
    }
}

