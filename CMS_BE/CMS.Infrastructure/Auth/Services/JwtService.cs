using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CMS.Application.Auth.Interfaces;
using CMS.Application.Shared.Configuration;
using CMS.Data;
using CMS.Domain.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CMS.Infrastructure.Auth.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly CmsDbContext _context;

        public JwtService(IOptions<JwtSettings> jwtSettings, CmsDbContext context)
        {
            _jwtSettings = jwtSettings.Value;
            _context = context;
        }

        public string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<Token> SaveTokenAsync(Guid userId, string accessToken, string refreshToken, int expiresInMinutes, int refreshExpiresInDays)
        {
            var token = new Token
            {
                TokenID = Guid.NewGuid(),
                UserID = userId,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = expiresInMinutes * 60, // Convert to seconds
                RefreshTokenExpiresIn = refreshExpiresInDays * 24 * 60 * 60, // Convert to seconds
                TokenType = "Bearer",
                Scope = "read write",
                GeneratedOn = DateTime.UtcNow,
                AccessTokenExpiresOn = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                RefreshTokenExpiresOn = DateTime.UtcNow.AddDays(refreshExpiresInDays)
            };

            _context.Tokens.Add(token);
            await _context.SaveChangesAsync();
            return token;
        }
    }
}

