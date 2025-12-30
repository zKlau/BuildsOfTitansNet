using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

public class AuthenticationService
{
    public static void SetTokensInsideCookie(string refreshToken, HttpContext context)
    {
        context.Response.Cookies.Append("refreshToken", refreshToken,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
    }

    private static string ToHex(byte[] bytes, bool upperCase)
    {
        StringBuilder result = new StringBuilder(bytes.Length * 2);
        for (int i = 0; i < bytes.Length; i++)
            result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
        return result.ToString();
    }

    public static string SHA256HexHashString(string StringIn)
    {
        string hashString;
        using (var sha256 = SHA256.Create())
        {
            var hash = sha256.ComputeHash(Encoding.Default.GetBytes(StringIn));
            hashString = ToHex(hash, false);
        }

        return hashString;
    }

    public static string GenerateJwtToken(string userId, string email, string name)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "default_secret_key_please_change"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        Console.WriteLine("Generating JWT for User ID: " + userId);

        var claims = new[]
        {
                new Claim("userId", userId),
                new Claim("email", email),
                new Claim("name", name)
            };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string DecryptRefreshToken(string refreshToken)
    {

        return refreshToken;
    }


    public static (string refreshToken, string digest) GenerateRefreshToken()
    {
        string refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        string digest = SHA256HexHashString(refreshToken);

        return (refreshToken, digest);
    }
}