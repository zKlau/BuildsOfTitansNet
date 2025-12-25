using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildsOfTitansNet.Models
{
    [Table("build_abilities")]
    public class BuildAbility
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("build_id")]
        public int BuildId { get; set; }

        [Column("dinosaur_ability_id")]
        public int DinosaurAbilityId { get; set; }

        [Column("slot_id")]
        public int? SlotId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("BuildId")]
        public Build Build { get; set; } = null!;

        [ForeignKey("DinosaurAbilityId")]
        public DinosaurAbility DinosaurAbility { get; set; } = null!;
    }
}