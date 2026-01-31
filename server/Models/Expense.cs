using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseSplitter.Models
{
    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public int PaidByUserId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string SplitType { get; set; } = "Equal"; // Equal, Unequal, Percentage

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("GroupId")]
        public Group Group { get; set; } = null!;

        [ForeignKey("PaidByUserId")]
        public User PaidBy { get; set; } = null!;

        public ICollection<ExpenseSplit> ExpenseSplits { get; set; } = new List<ExpenseSplit>();
    }
}
