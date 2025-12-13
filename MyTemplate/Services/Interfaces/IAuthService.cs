using MyTemplate.Contracts.Requests;

namespace MyTemplate.Services.Interfaces;

public interface IAuthService
{
    Task<Response<AuthResponse>> LoginAsync(AuthRequest authRequest);
    //Task<Response<AuthResponse>> RegisterAsync(RegisterRequest registerRequest);
    Task<Response<AuthResponse>> RefreshTokenAsync(string token);
    Task<Response<bool>> RevokeTokenAsync(string token);
}
