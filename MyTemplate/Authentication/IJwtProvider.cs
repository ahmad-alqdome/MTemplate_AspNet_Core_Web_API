using MyTemplate.Entities;
using System.Security.Claims;

namespace MyTemplate.Authentication;

public interface IJwtProvider
{
    (string token, int expiresIn) GenerateToken(ApplicationUser user , IEnumerable<string>roles);
    ClaimsPrincipal? ValidateToken(string token);

}
