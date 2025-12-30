
using Microsoft.AspNetCore.Mvc;
using BuildsOfTitansNet.Data;
using BuildsOfTitansNet.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public UserController(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUsersPreview()
    {

        var currentUser = await _currentUserService.GetCurrentUserAsync();
        Console.WriteLine("Current user: " + (currentUser?.Email ?? "null"));        
        if (currentUser == null)
        {
            return Unauthorized();
        }


        return Ok(new { name = currentUser.Name });
    }

    [HttpPut("change_name")]
    [Authorize]
    public async Task<IActionResult> UpdateUserName([FromBody] ChangeNameRequest request)
    {
     var currentUser = await _currentUserService.GetCurrentUserAsync();
        if (currentUser == null)
        {
            return Unauthorized();
        }

        currentUser.Name = request.Name;
        _dbContext.Users.Update(currentUser);
        await _dbContext.SaveChangesAsync();

        return Ok(new {
            currentUser.Id,
            currentUser.Name,
            currentUser.Email
        });   
    }

}
