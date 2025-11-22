using MyTemplate.Entities;
using System.Security.Claims;

namespace MyTemplate.Authentication;

public interface IJwtProvider
{
    (string token, int expiresIn) GenrateToken(ApplicationUser user , IEnumerable<string>roles);
    ClaimsPrincipal? ValidateToken(string token);

}
