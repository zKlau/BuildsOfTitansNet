using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildsOfTitansNet.Models
{
    [Table("build_votes")]
    public class BuildVote
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("build_id")]
        public int BuildId { get; set; }

        [Column("vote_type")]
        public int? VoteType { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [ForeignKey("BuildId")]
        public Build Build { get; set; } = null!;
    }
}