using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseSplitter.Models
{
    public class Invitation
    {
        [Key]
        public int InvitationId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        public int? InvitedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        public bool IsAccepted { get; set; } = false;

        public DateTime? AcceptedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(GroupId))]
        public Group Group { get; set; } = null!;

        [ForeignKey(nameof(InvitedByUserId))]
        public User? InvitedByUser { get; set; }
    }
}
