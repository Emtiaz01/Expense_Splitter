using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseSplitter.Models
{
    public class ExpenseSplit
    {
        [Key]
        public int SplitId { get; set; }

        [Required]
        public int ExpenseId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ShareAmount { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Percentage { get; set; }

        // Navigation properties
        [ForeignKey("ExpenseId")]
        public Expense Expense { get; set; } = null!;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}
