using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseSplitter.Models
{
    public class GroupMember
    {
        [Key]
        public int GroupMemberId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "Member"; // Admin or Member

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("GroupId")]
        public Group Group { get; set; } = null!;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}
