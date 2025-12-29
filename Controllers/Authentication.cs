using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using BuildsOfTitansNet.Data;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("v1/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    public AuthenticationController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GoogleJsonWebSignature.Payload> ValidateGoogleToken(string idToken) => await GoogleJsonWebSignature.ValidateAsync(idToken);

 
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

    public void GenerateAndSaveRefreshToken(BuildsOfTitansNet.Models.User user)
    {
        string refreshToken = AuthenticationService.GenerateRefreshToken();
        AuthenticationService.SetTokensInsideCookie(refreshToken, HttpContext);

        user.RefreshToken = refreshToken;
        user.RefreshTokenCreatedAt = DateTime.UtcNow;
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
                string jwtToken = AuthenticationService.GenerateJwtToken(user.Uid ?? "", user.Email ?? "", user.Name ?? "");
                GenerateAndSaveRefreshToken(user);
                
                return Ok(new { token = jwtToken });
            }

            return Ok(new { message = "Google token is valid.", tokenInfo });

        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = "Invalid Google token.", error = ex.Message });
        }
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshUser()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            return Unauthorized(new { message = "Refresh token cookie is missing." });
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null)
        {
            return Unauthorized(new { message = "Invalid refresh token." });
        }

        string newJwtToken = AuthenticationService.GenerateJwtToken(user.Uid ?? "", user.Email ?? "", user.Name ?? "");
        GenerateAndSaveRefreshToken(user);

        await _dbContext.SaveChangesAsync();

        return Ok(new { token = newJwtToken });
    }

}




