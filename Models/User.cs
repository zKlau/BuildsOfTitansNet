using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildsOfTitansNet.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("uid")]
        public string? Uid { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("refresh_token")]
        public string? RefreshToken { get; set; }

        [Column("refresh_token_created_at")]
        public DateTime? RefreshTokenCreatedAt { get; set; }

        // Navigation properties
        public ICollection<Build> Builds { get; set; } = new List<Build>();
        public ICollection<BuildVote> BuildVotes { get; set; } = new List<BuildVote>();
    }
}