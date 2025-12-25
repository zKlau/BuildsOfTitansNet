using BuildsOfTitansNet.Models;
using Microsoft.EntityFrameworkCore;

namespace BuildsOfTitansNet.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ability> Abilities { get; set; }
        public DbSet<AbilitySlot> AbilitySlots { get; set; }
        public DbSet<BaseStat> BaseStats { get; set; }
        public DbSet<Build> Builds { get; set; }
        public DbSet<BuildAbility> BuildAbilities { get; set; }
        public DbSet<BuildVote> BuildVotes { get; set; }
        public DbSet<Diet> Diets { get; set; }
        public DbSet<DinosaurAbility> DinosaurAbilities { get; set; }
        public DbSet<Species> Species { get; set; }
        public DbSet<SpeciesAbilitySlot> SpeciesAbilitySlots { get; set; }
        public DbSet<Subspecies> Subspecies { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}