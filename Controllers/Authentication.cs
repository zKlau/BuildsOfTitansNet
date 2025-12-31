using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using BuildsOfTitansNet.Data;
using BuildsOfTitansNet.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("v1/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public AuthenticationController(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
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
        (string refreshToken, string digest) = AuthenticationService.GenerateRefreshToken();
        AuthenticationService.SetTokensInsideCookie(refreshToken, HttpContext);

        user.RefreshToken = digest;
        user.RefreshTokenCreatedAt = DateTime.UtcNow;

        _dbContext.Users.Update(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> CreateLoginUser([FromHeader] string Authorization)
    {
        string? googleToken = ExtractTokenFromHeader(Authorization);

        if (googleToken == null)
        {
            return Unauthorized(new { message = "Invalid Authorization header." });
        }

        try
        {
            GoogleJsonWebSignature.Payload tokenInfo = await ValidateGoogleToken(googleToken);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Uid == tokenInfo.Subject);

            if (user == null)
            {
                user = new BuildsOfTitansNet.Models.User
                {
                    Uid = tokenInfo.Subject,
                    Name = tokenInfo.Name,
                    Email = tokenInfo.Email,
                };

                _dbContext.Users.Add(user);

                await _dbContext.SaveChangesAsync();
            }
            else
            {
                string jwtToken = AuthenticationService.GenerateJwtToken(user.Uid ?? "", user.Email ?? "", user.Name ?? "");
                GenerateAndSaveRefreshToken(user);
                await _dbContext.SaveChangesAsync();

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


        var digest = AuthenticationService.SHA256HexHashString(refreshToken);
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == digest);

        if (user == null)
        {
            return Unauthorized(new { message = "Invalid refresh token." });
        }

        string newJwtToken = AuthenticationService.GenerateJwtToken(user.Id.ToString() ?? "", user.Email ?? "", user.Name ?? "");
        GenerateAndSaveRefreshToken(user);

        await _dbContext.SaveChangesAsync();

        return Ok(new { access_token = newJwtToken });
    }

    [HttpDelete("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var currentUser = await _currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
        {
            return Unauthorized();
        }

        currentUser.RefreshToken = null;
        _dbContext.Users.Update(currentUser);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

}




