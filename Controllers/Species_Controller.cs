
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

    [HttpGet("all")]
    public async Task<IActionResult> GetSpecies() 
    {
        var species = await _dbContext.Species
            .Include(s => s.Diet)
            .Include(s => s.BaseStat)
            .Include(s => s.DinosaurAbilities)
                .ThenInclude(da => da.Ability)
            .Include(s => s.SpeciesAbilitySlots)
                .ThenInclude(sas => sas.AbilitySlot)
            .ToListAsync();

        var response = species.Select(s => new 
        {
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
        });
        
            
        return Ok(new { species = response });
    }
}