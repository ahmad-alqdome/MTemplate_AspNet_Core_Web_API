using System.Text.Json.Serialization;

namespace MyTemplate.Contracts.Responses;

public class AuthResponse
{
    public Guid UserId { get; set; }
    public string? Email { get; set; }
    public List<string>? Roles { get; set; }
    public string? Token { get; set; }
    public int ExpiresOn { get; set; }
    [JsonIgnore]
    public string? RefreshToken { get; set; }
    public int RefreshTokenExpiration { get; set; }
}

