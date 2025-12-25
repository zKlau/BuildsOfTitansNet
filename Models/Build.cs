using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildsOfTitansNet.Models
{
    [Table("builds")]
    public class Build
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("species_id")]
        public int SpeciesId { get; set; }

        [Column("subspecies_id")]
        public int SubspeciesId { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [ForeignKey("SpeciesId")]
        public Species Species { get; set; } = null!;

        [ForeignKey("SubspeciesId")]
        public Subspecies Subspecies { get; set; } = null!;

        public ICollection<BuildAbility> BuildAbilities { get; set; } = new List<BuildAbility>();
        public ICollection<BuildVote> BuildVotes { get; set; } = new List<BuildVote>();
    }
}