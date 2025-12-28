using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using BuildsOfTitansNet.Data;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    public AuthController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GoogleJsonWebSignature.Payload> ValidateGoogleToken(string idToken)
    {
        return await GoogleJsonWebSignature.ValidateAsync(idToken);

    }

    private string GenerateJwtToken(string userId, string email, string name)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "default_secret_key_please_change"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
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
    public string? ExtractTokenFromHeader(string authorizationHeader)
    {
        if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var headerValue))
        {
            if (headerValue.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase))
            {
                return headerValue.Parameter;
            }
        }
        return null;
    }

    [HttpPost("login")]
    public async Task<IActionResult> CreateLoginUser([FromHeader] string Authorization)
    {
        string? googleToken = ExtractTokenFromHeader(Authorization);
        
        if (googleToken == null)
        {
            return Unauthorized(new { message = "Invalid Authorization header." });
        }

        try {
            GoogleJsonWebSignature.Payload tokenInfo = await ValidateGoogleToken(googleToken);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Uid == tokenInfo.Subject);

            if (user == null)
            {
                user = new BuildsOfTitansNet.Models.User
                {
                    Uid = tokenInfo.Subject,
                    Name = tokenInfo.Name,
                    Email = tokenInfo.Email,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _dbContext.Users.Add(user);

                await _dbContext.SaveChangesAsync();
            }

            if(user != null)
            {
                string jwtToken = GenerateJwtToken(user.Uid ?? "", user.Email ?? "", user.Name ?? "");

                return Ok(new { token = jwtToken });
            }

            return Ok(new { message = "Google token is valid.", tokenInfo });

        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = "Invalid Google token.", error = ex.Message });
        }


    }

}




