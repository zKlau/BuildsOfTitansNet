using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BuildsOfTitansNet.Models
{
    [Table("base_stats")]
    public class BaseStat
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("species_id")]
        public int SpeciesId { get; set; }

        [Column("damage")]
        public int? Damage { get; set; }

        [Column("defense")]
        public int? Defense { get; set; }

        [Column("recovery")]
        public int? Recovery { get; set; }

        [Column("land_speed")]
        public int? LandSpeed { get; set; }

        [Column("water_speed")]
        public int? WaterSpeed { get; set; }

        [Column("survivability")]
        public int? Survivability { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        [JsonIgnore]
        [ForeignKey("SpeciesId")]
        public Species Species { get; set; } = null!;
    }
}