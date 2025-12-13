using Microsoft.AspNetCore.Mvc;
using MyTemplate.Contracts.Requests;

namespace MyTemplate.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private const string REFRESH_COOKIE_NAME = "refreshToken";

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] AuthRequest authRequest)
    {
        var result = await authService.LoginAsync(authRequest);
        if (!result.IsSuccess)
            return this.ToActionResult(result);

        if (!string.IsNullOrEmpty(result.Result!.RefreshToken))
            SetRefreshTokenInCookie(result.Result.RefreshToken, result.Result.RefreshTokenExpiration);
        return Ok(result);
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync()
    {


        if (!Request.Cookies.TryGetValue(REFRESH_COOKIE_NAME, out var plainRefresh) || string.IsNullOrEmpty(plainRefresh))
        {
            var error = Response<string>.Failure(AuthError.RefreashTokenNotFound);
            return Unauthorized(error);
        }
        var result = await authService.RefreshTokenAsync(plainRefresh);

        if (!result.IsSuccess)
        {
            Response.Cookies.Delete(REFRESH_COOKIE_NAME, new CookieOptions { HttpOnly = true, Secure = true });
            return Unauthorized(result);
        }

        SetRefreshTokenInCookie(result.Result!.RefreshToken!, result.Result.RefreshTokenExpiration);
        return Ok(result);
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest? request)
    {
        if (!Request.Cookies.TryGetValue(REFRESH_COOKIE_NAME, out var plainRefresh) || string.IsNullOrEmpty(plainRefresh))
        {
            var error = Response<string>.Failure(AuthError.RefreashTokenNotFound);
            return BadRequest(error);
        }

        var result = await authService.RevokeTokenAsync(plainRefresh);
        Response.Cookies.Delete(REFRESH_COOKIE_NAME, new CookieOptions { HttpOnly = true, Secure = true });


        if (!result.IsSuccess)
            return this.ToActionResult(result);

        return Ok(result);
    }


    private void SetRefreshTokenInCookie(string refreshToken, int expiresOnSeconds)
    {
        var expirationTime = DateTime.UtcNow.AddSeconds(expiresOnSeconds);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = expiresOnSeconds > 0 ? DateTime.UtcNow.AddSeconds(expiresOnSeconds) : expirationTime,
            Secure = true,
            IsEssential = true,
            SameSite = SameSiteMode.None
        };

        Response.Cookies.Append(REFRESH_COOKIE_NAME, refreshToken, cookieOptions);
    }
}
