using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

namespace CMS.Domain.Auth.Entities
{
    public class Token
    {
        public Guid TokenID { get; set; }
        public Guid UserID { get; set; }
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }
        [JsonPropertyName("refresh_token_expires_in")]
        public int? RefreshTokenExpiresIn { get; set; }
        [JsonPropertyName("scope")]
        public string? Scope { get; set; }
        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }
        public DateTime GeneratedOn { get; set; }
        public DateTime AccessTokenExpiresOn { get; set; }
        public DateTime? RefreshTokenExpiresOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsTokenExpired => DateTime.UtcNow >= AccessTokenExpiresOn;
        public bool IsRefreshTokenExpired => RefreshTokenExpiresOn.HasValue && DateTime.UtcNow >= RefreshTokenExpiresOn.Value;

    }
}
