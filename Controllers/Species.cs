
using Microsoft.AspNetCore.Mvc;
using BuildsOfTitansNet.Data;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class SpeciesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public SpeciesController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private object ReturnSpecies(BuildsOfTitansNet.Models.Species s)
    {
        return new {
            s.Id,
            s.Name,
            s.Icon,
            s.Description,
            diet = new { s.Diet.Id, s.Diet.Name },
            base_stat = new {s.BaseStat?.Damage, s.BaseStat?.Defense, s.BaseStat?.Recovery, s.BaseStat?.LandSpeed, s.BaseStat?.WaterSpeed, s.BaseStat?.Survivability },
            slots = s.SpeciesAbilitySlots.Select(sas => new 
            {
                id = sas.AbilitySlot.Id,
                name = sas.AbilitySlot.Name,
                limit = sas.Limit
            }),
            subspecies = s.Subspecies,
        };
    }

    private object ReturnSpeciesPreview(BuildsOfTitansNet.Models.Species s)
    {
        return new {
            s.Id,
            s.Name,
            s.Icon,
            diet = s.Diet.Name,
        };
    }

    private List<BuildsOfTitansNet.Models.Species> GetSpeciesFromDb()
    {
        return _dbContext.Species
            .Include(s => s.Diet)
            .Include(s => s.BaseStat)
            .Include(s => s.Subspecies)
            .Include(s => s.DinosaurAbilities)
                .ThenInclude(da => da.Ability)
            .Include(s => s.SpeciesAbilitySlots)
                .ThenInclude(sas => sas.AbilitySlot)
            .ToList();
    }

    [HttpGet]
    public async Task<IActionResult> GetSpeciesPreview() 
    {
        var species = GetSpeciesFromDb();
        return Ok(new { species = species.Select(s => ReturnSpeciesPreview(s) )});
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetSpecies() 
    {
        var species = GetSpeciesFromDb();
        return Ok(new { species = species.Select(s => ReturnSpecies(s) )});
    }


    [HttpGet("{name}")]
    public async Task<IActionResult> GetSpeciesName(string name) 
    {
        var species = GetSpeciesFromDb();
        species = species.Where(s => s.Name.ToLower().Contains(name.ToLower())).ToList();

        return Ok(new {species = ReturnSpecies(species.FirstOrDefault() ?? new BuildsOfTitansNet.Models.Species())});
    }
}