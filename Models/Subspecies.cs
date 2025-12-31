using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BuildsOfTitansNet.Models
{
    [Table("subspecies")]
    public class Subspecies
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("species_id")]
        public int SpeciesId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [JsonIgnore]
        [ForeignKey("SpeciesId")]
        public Species Species { get; set; } = null!;

        [JsonIgnore]
        public ICollection<Build> Builds { get; set; } = new List<Build>();
    }
}