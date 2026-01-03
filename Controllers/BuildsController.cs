using Microsoft.AspNetCore.Mvc;
using BuildsOfTitansNet.Data;
using Microsoft.EntityFrameworkCore;
using BuildsOfTitansNet.Services;
using BuildsOfTitansNet.Models;

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
    private async Task<object> ReturnBuild(BuildsOfTitansNet.Models.Build s, BuildsOfTitansNet.Models.User? currentUser)
    {
        return new
        {
            s.Id,
            s.Name,
            created_at = s.CreatedAt,
            user_name = s.User?.Name ?? "Unknown",
            dinosaur = new
            {
                s.Species.Id,
                s.Species.Name,
                s.Species.Icon,
                s.Species.Description,
                diet = new { s.Species.Diet.Id, s.Species.Diet.Name },
                base_stat = new
                {
                    s.Species.BaseStat?.Damage,
                    s.Species.BaseStat?.Defense,
                    s.Species.BaseStat?.Recovery,
                    s.Species.BaseStat?.LandSpeed,
                    s.Species.BaseStat?.WaterSpeed,
                    s.Species.BaseStat?.Survivability
                },
                slots = s.Species.SpeciesAbilitySlots.Select(sas => new
                {
                    id = sas.AbilitySlot.Id,
                    name = sas.AbilitySlot.Name,
                    limit = sas.Limit
                }),
                subspecies = s.Subspecies
            },
            votes = new
            {
                user_vote = s.BuildVotes.FirstOrDefault(v => v.UserId == currentUser?.Id)?.VoteType ?? null,
                upvotes = s.BuildVotes.Count(v => v.VoteType == 0),
                downvotes = s.BuildVotes.Count(v => v.VoteType == 1)
            },
            abilities = s.BuildAbilities.Select(ba => new
            {
                id = ba.Id,
                slots = new
                {
                    id = ba.DinosaurAbility.AbilitySlot.Id,
                    name = ba.DinosaurAbility.AbilitySlot.Name
                },
                name = ba.DinosaurAbility.Ability.Name,
                description = ba.DinosaurAbility.Description,
                icon = ba.DinosaurAbility.Ability.Icon,
                price = ba.DinosaurAbility.Price,
            })
        };
    }

    private async Task<object> ReturnBuildPreview(BuildsOfTitansNet.Models.Build s, BuildsOfTitansNet.Models.User? currentUser)
    {
        return new
        {
            s.Id,
            s.Name,
            created_at = s.CreatedAt,
            user_name = s.User?.Name ?? "Unknown",
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

    [HttpPost]
    public async Task<IActionResult> CreateBuild([FromBody] BuildCreateRequest request)
    {
        var currentUser = await _currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Build name is required" });
        }

        if (request.Abilities == null || request.Abilities.Count == 0)
        {
            return BadRequest(new { message = "At least one ability is required" });
        }

        var speciesExists = await _dbContext.Species
            .Where(s => s.Id == request.Species)
            .Include(s => s.Subspecies)
            .FirstOrDefaultAsync();

        if (speciesExists == null)
        {
            return BadRequest(new { message = "Invalid species ID" });
        }

        if (!speciesExists.Subspecies.Any(ss => ss.Id == request.Subspecies))
        {
            return BadRequest(new { message = "Invalid subspecies ID for the given species" });
        }

        var newBuild = new Build
        {
            Name = request.Name,
            Description = request.Description ?? string.Empty,
            SpeciesId = request.Species,
            SubspeciesId = request.Subspecies,
            UserId = currentUser.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        foreach (var abilityId in request.Abilities)
        {
            var dinosaurAbility = await _dbContext.DinosaurAbilities
                .Where(da => da.AbilityId == abilityId.Id)
                .FirstOrDefaultAsync();

            if (dinosaurAbility == null)
            {
                return BadRequest(new { message = $"Invalid ability ID: {abilityId.Id}" });
            }

            var buildAbility = new BuildAbility
            {
                DinosaurAbilityId = dinosaurAbility.Id
            };

            newBuild.BuildAbilities.Add(buildAbility);
        }

        _dbContext.Builds.Add(newBuild);
        await _dbContext.SaveChangesAsync();

        return Ok(newBuild);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBuild(int id)
    {
        var currentUser = await _currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        var build = await _dbContext.Builds
            .Include(b => b.BuildAbilities)
            .Include(b => b.BuildVotes)
            .Where(b => b.Id == id)
            .FirstOrDefaultAsync();

        if (build == null)
        {
            return NotFound(new { message = "Build not found" });
        }

        if (build.UserId != currentUser.Id)
        {
            return Forbid();
        }

        _dbContext.Builds.Remove(build);
        _dbContext.BuildVotes.RemoveRange(build.BuildVotes);
        _dbContext.Builds.Remove(build);
        await _dbContext.SaveChangesAsync();

        return Ok(new { message = "Build deleted successfully" });
    }

    [HttpGet("dino/name/{name}")]
    public async Task<IActionResult> GetBuildPreviews(string name)
    {
        var currentUser = await _currentUserService.GetCurrentUserAsync();

        var builds = await _dbContext.Builds
            .Where(b => b.Species.Name.ToLower() == name.ToLower())
            .Include(b => b.User)
            .Where(b => b.User != null)
            .Include(b => b.Species)
            .Include(b => b.Subspecies)
            .Include(b => b.BuildVotes)
            .OrderByDescending(b => b.BuildVotes.Count(v => v.VoteType == 0))
            .ToListAsync();

        var buildPreviews = await Task.WhenAll(builds.Select(b => ReturnBuildPreview(b, currentUser)));

        return Ok(new { builds = buildPreviews });
    }

    [HttpGet("owner/{id}")]
    public async Task<IActionResult> GetBuildIsOwner(int id)
    {
        var currentUser = await _currentUserService.GetCurrentUserAsync();

        var build = await _dbContext.Builds
            .Include(b => b.User)
            .Where(b => b.Id == id)
            .FirstOrDefaultAsync();

        return Ok(new { isOwner = build != null && currentUser != null && build.UserId == currentUser.Id });

    }

    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetBuild(int id)
    {
        var currentUser = await _currentUserService.GetCurrentUserAsync();

        var build = await _dbContext.Builds
            .Include(b => b.User)
            .Include(b => b.Species)
                .ThenInclude(s => s.Diet)
            .Include(b => b.Species)
                .ThenInclude(s => s.BaseStat)
            .Include(b => b.Species)
                .ThenInclude(s => s.SpeciesAbilitySlots)
                    .ThenInclude(sas => sas.AbilitySlot)
            .Include(b => b.Subspecies)
            .Include(b => b.BuildVotes)
            .Include(b => b.BuildAbilities)
                .ThenInclude(ba => ba.DinosaurAbility)
                    .ThenInclude(da => da.Ability)
            .Include(b => b.BuildAbilities)
                .ThenInclude(ba => ba.DinosaurAbility)
                    .ThenInclude(da => da.AbilitySlot)
            .Where(b => b.Id == id)
            .FirstOrDefaultAsync();

        if (build == null || build.User == null || build.Species == null || build.Subspecies == null ||
            build.BuildAbilities.Any(ba => ba.DinosaurAbility == null ||
                                          ba.DinosaurAbility.Ability == null ||
                                          ba.DinosaurAbility.AbilitySlot == null))
        {
            return NotFound(new { message = "Build not found" });
        }

        var result = await ReturnBuild(build, currentUser);

        return Ok(result);
    }

    [HttpPost("{id}/vote")]
    public async Task<IActionResult> VoteBuild(int id, [FromBody] VoteRequest request)
    {
        var currentUser = await _currentUserService.GetCurrentUserAsync();
        var build = await _dbContext.Builds
           .Where(b => b.Id == id)
           .FirstOrDefaultAsync();

        if (currentUser == null)
        {
            return Unauthorized(new { message = "User not authenticated" });
        }
        
        if (build == null)
        {
            return NotFound(new { message = "Build not found" });
        }
        
        var existingVote = await _dbContext.BuildVotes
            .Where(v => v.BuildId == id && v.UserId == currentUser.Id)
            .FirstOrDefaultAsync();

        if (request.VoteType == VoteType.Type.upvote && existingVote != null && existingVote.VoteType == (int)VoteType.Type.upvote ||
            request.VoteType == VoteType.Type.downvote && existingVote != null && existingVote.VoteType == (int)VoteType.Type.downvote)
        {
            _dbContext.BuildVotes.Remove(existingVote);
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            var utcNow = new DateTime(DateTime.UtcNow.Ticks, DateTimeKind.Utc);
            if (existingVote != null)
            {
                _dbContext.BuildVotes.Remove(existingVote);
                var updatedVote = new BuildVote
                {
                    BuildId = id,
                    UserId = currentUser.Id,
                    VoteType = (int)request.VoteType,
                    CreatedAt = new DateTime(existingVote.CreatedAt.Ticks, DateTimeKind.Utc),
                    UpdatedAt = utcNow
                };
                _dbContext.BuildVotes.Add(updatedVote);
            }
            else
            {
                var newVote = new BuildVote
                {
                    BuildId = id,
                    UserId = currentUser.Id,
                    VoteType = (int)request.VoteType,
                    CreatedAt = utcNow,
                    UpdatedAt = utcNow
                };
                _dbContext.BuildVotes.Add(newVote);
            }
            await _dbContext.SaveChangesAsync();
        }
        var upvotes = await _dbContext.BuildVotes.CountAsync(v => v.BuildId == id && v.VoteType == (int)VoteType.Type.upvote);
        var downvotes = await _dbContext.BuildVotes.CountAsync(v => v.BuildId == id && v.VoteType == (int)VoteType.Type.downvote);
        
        return Ok(new { user_vote = request.VoteType, upvotes, downvotes });
    }
}