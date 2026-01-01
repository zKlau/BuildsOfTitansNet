
using Microsoft.AspNetCore.Mvc;
using BuildsOfTitansNet.Data;
using Microsoft.EntityFrameworkCore;
using BuildsOfTitansNet.Services;
using BuildsOfTitansNet.Models;

[ApiController]
[Route("v1/[controller]")]
public class AbilitiesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public AbilitiesController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetAbilitiesBySpeciesName(string name)
    {
        var abilities = await _dbContext.DinosaurAbilities
            .Include(da => da.Ability)
            .Include(da => da.Species)
            .Include(da => da.AbilitySlot)
            .Where(da => da.Species.Name.ToLower() == name.ToLower())
            .ToListAsync();

        var result = abilities.Select(da => new
        {
            id = da.Ability.Id,
            name = da.Ability.Name,
            icon = da.Ability.Icon,
            description = da.Ability.DinosaurAbilities.FirstOrDefault(dab => dab.Id == da.Id)?.Description,
            price = da.Price,
            slots = new
            {
                id = da.AbilitySlot.Id,
                name = da.AbilitySlot.Name,
            }
        });

        return Ok(new { abilities = result });
    }

}