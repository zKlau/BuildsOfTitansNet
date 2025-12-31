
using Microsoft.AspNetCore.Mvc;
using BuildsOfTitansNet.Data;
using Microsoft.EntityFrameworkCore;
using BuildsOfTitansNet.Services;

[ApiController]
[Route("v1/[controller]")]
public class BuildsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public BuildsController(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    private async Task<object> ReturnBuildPreview(BuildsOfTitansNet.Models.Build s, BuildsOfTitansNet.Models.User? currentUser)
    {


        return new
        {
            s.Id,
            s.Name,
            created_at = s.CreatedAt,
            user_name = s.User.Name,
            dinosaur = new
            {
                s.Species.Id,
                s.Species.Name,
                s.Species.Icon,
                subspecies = s.Subspecies
            },
            votes = new
            {
                user_vote = s.BuildVotes.FirstOrDefault(v => v.UserId == currentUser?.Id)?.VoteType ?? null,
                upvotes = s.BuildVotes.Count(v => v.VoteType == 0),
                downvotes = s.BuildVotes.Count(v => v.VoteType == 1)
            }
        };
    }

    [HttpGet("dino/name/{name}")]
    public async Task<IActionResult> GetBuildPreviews(string name)
    {
        var currentUser = await _currentUserService.GetCurrentUserAsync();

        var builds = await _dbContext.Builds
            .Where(b => b.Species.Name.ToLower() == name.ToLower())
            .Include(b => b.Species)
            .Include(b => b.Subspecies)
            .Include(b => b.User)
            .Include(b => b.BuildVotes)
            .OrderByDescending(b => b.BuildVotes.Count(v => v.VoteType == 0))
            .ToListAsync();

        var buildPreviews = await Task.WhenAll(builds.Select(b => ReturnBuildPreview(b, currentUser)));

        return Ok(new { builds = buildPreviews });
    }

}
