using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildsOfTitansNet.Models
{
    [Table("species_ability_slots")]
    public class SpeciesAbilitySlot
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("ability_slot_id")]
        public int AbilitySlotId { get; set; }

        [Column("species_id")]
        public int SpeciesId { get; set; }

        [Column("limit")]
        public int? Limit { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("AbilitySlotId")]
        public AbilitySlot AbilitySlot { get; set; } = null!;

        [ForeignKey("SpeciesId")]
        public Species Species { get; set; } = null!;
    }
}