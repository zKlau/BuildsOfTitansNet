using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BuildsOfTitansNet.Models
{
    [Table("species")]
    public class Species
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("diet_id")]
        public int DietId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("icon")]
        public string? Icon { get; set; }

        // Navigation properties
        [ForeignKey("DietId")]
        public Diet Diet { get; set; } = null!;

        [JsonPropertyName("base_stat")]
        public BaseStat? BaseStat { get; set; }
        
        public ICollection<Build> Builds { get; set; } = new List<Build>();
        
        [JsonPropertyName("abilities")]
        public ICollection<DinosaurAbility> DinosaurAbilities { get; set; } = new List<DinosaurAbility>();
        
        [JsonPropertyName("slots")]
        public ICollection<SpeciesAbilitySlot> SpeciesAbilitySlots { get; set; } = new List<SpeciesAbilitySlot>();
        public ICollection<Subspecies> Subspecies { get; set; } = new List<Subspecies>();
    }
}