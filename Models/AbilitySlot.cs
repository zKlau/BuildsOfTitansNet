using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BuildsOfTitansNet.Models
{
    [Table("ability_slots")]
    public class AbilitySlot
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [JsonIgnore]
        public ICollection<DinosaurAbility> DinosaurAbilities { get; set; } = new List<DinosaurAbility>();
        [JsonIgnore]
        public ICollection<SpeciesAbilitySlot> SpeciesAbilitySlots { get; set; } = new List<SpeciesAbilitySlot>();
    }
}