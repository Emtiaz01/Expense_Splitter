using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseSplitter.Models
{
    public class Settlement
    {
        [Key]
        public int SettlementId { get; set; }

        [Required]
        public int FromUserId { get; set; }

        [Required]
        public int ToUserId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public int GroupId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Note { get; set; }

        // Navigation properties
        [ForeignKey("FromUserId")]
        public User FromUser { get; set; } = null!;

        [ForeignKey("ToUserId")]
        public User ToUser { get; set; } = null!;

        [ForeignKey("GroupId")]
        public Group Group { get; set; } = null!;
    }
}
