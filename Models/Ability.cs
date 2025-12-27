using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BuildsOfTitansNet.Models
{
    [Table("abilities")]
    public class Ability
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("icon")]
        public string? Icon { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [JsonIgnore]
        public ICollection<DinosaurAbility> DinosaurAbilities { get; set; } = new List<DinosaurAbility>();
    }
}