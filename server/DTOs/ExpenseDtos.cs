using System.ComponentModel.DataAnnotations;

namespace ExpenseSplitter.DTOs
{
    public class CreateExpenseDto
    {
        [Required]
        public int GroupId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public int PaidByUserId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string SplitType { get; set; } = "Equal"; // Equal, Unequal, Percentage

        [Required]
        public List<SplitDto> Splits { get; set; } = new();
    }

    public class UpdateExpenseDto
    {
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string SplitType { get; set; } = "Equal";

        [Required]
        public List<SplitDto> Splits { get; set; } = new();
    }

    public class SplitDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal ShareAmount { get; set; }

        public decimal? Percentage { get; set; }
    }

    public class ExpenseDto
    {
        public int ExpenseId { get; set; }
        public int GroupId { get; set; }
        public decimal Amount { get; set; }
        public int PaidByUserId { get; set; }
        public string PaidByUserName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SplitType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<ExpenseSplitDto> Splits { get; set; } = new();
    }

    public class ExpenseSplitDto
    {
        public int SplitId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal ShareAmount { get; set; }
        public decimal? Percentage { get; set; }
    }
}
