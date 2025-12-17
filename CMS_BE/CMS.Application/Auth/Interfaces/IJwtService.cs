using CMS.Domain.Auth.Entities;

namespace CMS.Application.Auth.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        Task<Token> SaveTokenAsync(Guid userId, string accessToken, string refreshToken, int expiresInMinutes, int refreshExpiresInDays);
    }
}

