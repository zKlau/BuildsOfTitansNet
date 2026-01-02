using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BuildsOfTitansNet.Models
{
    [Table("dinosaur_abilities")]
    public class DinosaurAbility
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("species_id")]
        public int SpeciesId { get; set; }

        [Column("ability_id")]
        public int AbilityId { get; set; }

        [Column("price")]
        public int? Price { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("ability_slot_id")]
        public int AbilitySlotId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [JsonIgnore]
        [ForeignKey("SpeciesId")]
        public Species Species { get; set; } = null!;

        [JsonIgnore]
        [ForeignKey("AbilityId")]
        public Ability Ability { get; set; } = null!;

        [ForeignKey("AbilitySlotId")]
        public AbilitySlot AbilitySlot { get; set; } = null!;

        public ICollection<BuildAbility> BuildAbilities { get; set; } = new List<BuildAbility>();
    }
}