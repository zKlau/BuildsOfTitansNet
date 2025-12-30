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

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasConversion(
                    v => v,
                    v => new DateTime(v.Ticks, DateTimeKind.Utc))
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
                .Property(u => u.UpdatedAt)
                .HasConversion(
                    v => v,
                    v => new DateTime(v.Ticks, DateTimeKind.Utc))
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
                .Property(u => u.RefreshTokenCreatedAt)
                .HasConversion(
                    v => v,
                    v => v.HasValue ? new DateTime(v.Value.Ticks, DateTimeKind.Utc) : null);
        }
    }
}