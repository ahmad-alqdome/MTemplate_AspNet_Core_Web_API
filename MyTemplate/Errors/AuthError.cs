using MyTemplate.APIResponses;

namespace MyTemplate.Errors;
public class AuthError
{
    public static readonly Error InvalidCredentials = new Error("INVALID_CREDENTIALS", "Username or password is incorrect.", Code.Status401Unauthorized);
    public static readonly Error ZatcaInvalidCredentials = new Error("ZATCA_INVALID_CREDENTIALS", "Zatca username or password is incorrect.", Code.Status401Unauthorized);
    public static readonly Error LoginFailed = new Error("LOGIN_FAILED", "An error occurred while attempting to log in.", Code.Status500InternalServerError);
    public static readonly Error ExistUser = new Error("EXIST_USER", "User already exists.", Code.Status409Conflict);
    public static readonly Error UserNotFound = new Error("USER_NOT_FOUND", "The User is Not Found.", Code.Status404NotFound);
    public static readonly Error MissingToken = new Error("MISSING_TOKEN", "Token is required!", Code.Status400BadRequest);

    public static readonly Error TokenNotFound = new Error("TOKEN_NOT_FOUND", "The Refresh Token Not Found", Code.Status404NotFound);
    public static readonly Error InvalidToken = new Error("INVALID_TOKEN", "The Token is invalid", Code.Status404NotFound);
    public static readonly Error InActiveToken = new Error("INACTIVE_TOKEN", "The Token is inactive", Code.Status404NotFound);
    public static readonly Error RefreashTokenFaild = new Error("REFREASH_TOKEN_FAILED", "An error occurred while refreshing the token.;", Code.Status500InternalServerError);
    public static readonly Error RefreashTokenNotFound = new Error("REFREASH_TOKEN_NOTFOUND", "Refresh Token is not found.;", Code.Status404NotFound);
    public static readonly Error RevokeTokenFaild = new Error("RVOKE_TOKEN_FAILED", "An error occurred while revoke the token.;", Code.Status500InternalServerError);
    public static readonly Error TokenExpired = new Error("TOKEN_EXPIRED", "Refresh token has expired. Please login again.", Code.Status401Unauthorized);
    public static readonly Error TokenReused = new Error("TOKEN_REUSED", "Detected reuse of an old refresh token. Account security may be compromised.", Code.Status401Unauthorized);

}