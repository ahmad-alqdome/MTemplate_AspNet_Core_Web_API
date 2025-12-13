using MyTemplate.Contracts.Requests;

namespace MyTemplate.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private IJwtProvider _jwtProvider;
    private ILogger<AuthService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private const int REFRESH_TOKEN_DAYS = 30;
    private const int MAX_REFRESH_TOKENS = 20;

    public AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider, ILogger<AuthService> logger, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
        _logger = logger;
        _signInManager = signInManager;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Response<AuthResponse>> LoginAsync(AuthRequest authRequest)
    {
        try
        {

            var requestIp = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();


            _logger.LogInformation("LoginAsync method called for user: {Email}", authRequest.Email);
            var user = await _userManager.Users
                            .Include(u => u.RefreshTokens)
                            .FirstOrDefaultAsync(u => u.Email == authRequest.Email);

            if (user == null)
                return Response.Failure<AuthResponse>(AuthError.InvalidCredentials);

            var result = await _signInManager.PasswordSignInAsync(user, authRequest.Password, false, true);

            if (!result.Succeeded)
                return Response.Failure<AuthResponse>(AuthError.InvalidCredentials);


            _logger.LogInformation("User authenticated successfully: {Username}", user.UserName);

            var userRoles = await _userManager.GetRolesAsync(user);


            var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRoles);

            // prepare refreshtoken 
            var (plainRefresh, refreshHash) = RefreshTokenHelper.GenerateTokenAndHash();

            //var refreshToken = GenerateRefreshToken();
            //var refreshTokenExpiration = refreshToken.ExpiresOn;
            //var refreshTokenSeconds = (int)TimeSpan.FromDays(REFRESH_TOKEN_DAYS).TotalSeconds;


            var refreshEntity = new RefreshToken
            {
                TokenHash = refreshHash,
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(REFRESH_TOKEN_DAYS),
                CreatedByIp = requestIp,
                UserId = user.Id
            };

            // محدودية عدد التوكنات
            if (user.RefreshTokens.Count >= MAX_REFRESH_TOKENS)
            {
                var oldest = user.RefreshTokens.OrderBy(t => t.CreatedOn).First();
                user.RefreshTokens.Remove(oldest);
            }
            user.RefreshTokens.Add(refreshEntity);
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("JWT and refresh token generated for user: {Username}", user.UserName);

            var authResponse = new AuthResponse
            {
                Token = token,
                RefreshToken = plainRefresh,
                Email = user.Email,
                ExpiresOn = expiresIn,
                RefreshTokenExpiration = (int)(refreshEntity.ExpiresOn - DateTime.UtcNow).TotalSeconds,
                UserId = user.Id,
                Roles = userRoles.ToList()
            };

            return Response.Success(authResponse);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for user {Email}", authRequest.Email);

            return Response.Failure<AuthResponse>(AuthError.LoginFailed);
        }

    }

    public async Task<Response<AuthResponse>> RefreshTokenAsync(string incomingPlainToken)
    {
        try
        {
            _logger.LogInformation("RefreshTokenAsync called for token: {Token}", incomingPlainToken);

            var incomingHash = RefreshTokenHelper.ComputeSha256Hash(incomingPlainToken);
            var requestIp = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();


            var user = await _userManager.Users
               .Include(u => u.RefreshTokens)
               .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.TokenHash == incomingHash));


            if (user == null)
                return Response.Failure<AuthResponse>(AuthError.InvalidToken);

            var refreshToken = user.RefreshTokens.First(t => t.TokenHash == incomingHash);

            // 1) token expired
            if (refreshToken.IsExpired)
                return Response.Failure<AuthResponse>(AuthError.TokenExpired);

            // 2) token revoked → suspicious reuse
            if (refreshToken.RevokedOn != null)
            {
                // optional: revoke all tokens for security
                foreach (var t in user.RefreshTokens.Where(x => x.IsActive))
                {
                    t.RevokedOn = DateTime.UtcNow;
                    t.RevokedByIp = requestIp;
                    t.RevokedReason = "Suspicious reuse";
                }
                await _userManager.UpdateAsync(user);

                return Response.Failure<AuthResponse>(AuthError.TokenReused);
            }

            // rotate: revoke current
            refreshToken.RevokedOn = DateTime.UtcNow;
            refreshToken.RevokedByIp = requestIp;
            refreshToken.RevokedReason = "Rotated";

            var (newPlain, newHash) = RefreshTokenHelper.GenerateTokenAndHash();

            var newEntity = new RefreshToken
            {
                TokenHash = newHash,
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(REFRESH_TOKEN_DAYS),
                CreatedByIp = requestIp,
                UserId = user.Id
            };

            user.RefreshTokens.Add(newEntity);

            var toRemove = user.RefreshTokens
                              .Where(t => t.IsExpired && (t.RevokedOn != null || t.CreatedOn < DateTime.UtcNow.AddMonths(-3)))
                              .ToList();

            foreach (var token in toRemove)
            {
                user.RefreshTokens.Remove(token);
            }

            await _userManager.UpdateAsync(user);

            // create JWT
            var roles = await _userManager.GetRolesAsync(user);
            var (newJwtToken, expiresIn) = _jwtProvider.GenerateToken(user, roles);

            return Response.Success(new AuthResponse
            {
                Token = newJwtToken,
                RefreshToken = newPlain,
                Email = user.Email,
                ExpiresOn = expiresIn,
                RefreshTokenExpiration = (int)TimeSpan.FromDays(REFRESH_TOKEN_DAYS).TotalSeconds,
                UserId = user.Id,
                Roles = roles.ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RefreshTokenAsync for token: {Token}", incomingPlainToken);
            return Response.Failure<AuthResponse>(AuthError.LoginFailed);
        }
    }

    public async Task<Response<bool>> RevokeTokenAsync(string incomingPlainToken)
    {
        try
        {
            _logger.LogInformation("RevokeTokenAsync method called for token: {Token}", incomingPlainToken);
            var incomingHash = RefreshTokenHelper.ComputeSha256Hash(incomingPlainToken);
            var requestIp = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();


            var user = await _userManager.Users
               .Include(u => u.RefreshTokens)
               .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.TokenHash == incomingHash));


            if (user == null)
            {
                _logger.LogInformation("User not found for token: {Token}", incomingPlainToken);
                return Response.Failure<bool>(AuthError.UserNotFound);
            }

            var refreshToken = user.RefreshTokens.FirstOrDefault(t => t.TokenHash == incomingHash);

            if (refreshToken == null || !refreshToken.IsActive)
            {
                _logger.LogInformation("Token already inactive: {Token}", incomingPlainToken);
                return Response.Failure<bool>(AuthError.InActiveToken);
            }
            refreshToken.RevokedOn = DateTime.UtcNow;
            refreshToken.RevokedByIp = requestIp;
            refreshToken.RevokedReason = "User logout";

            await _userManager.UpdateAsync(user);
            return Response.Success(true);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in RevokeTokenAsync for token: {Token}", incomingPlainToken);
            return Response.Failure<bool>(AuthError.RevokeTokenFaild);
        }
    }
}
