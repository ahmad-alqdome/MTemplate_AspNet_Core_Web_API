using System.Security.Cryptography;

namespace MyTemplate.Helpers;

public class RefreshTokenHelper
{
    public static (string PlainToken, string TokenHash) GenerateTokenAndHash(int sizeBytes = 32)
    {
        var randomNumber = new byte[sizeBytes];
        RandomNumberGenerator.Fill(randomNumber);

        var plainToken = Convert.ToBase64String(randomNumber);
        var tokenHash = ComputeSha256Hash(plainToken);

        return (plainToken, tokenHash);

    }

    public static string ComputeSha256Hash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha.ComputeHash(bytes);
        var sb = new StringBuilder();
        foreach (var b in hash) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
}
