using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseSplitter.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Group> CreatedGroups { get; set; } = new List<Group>();
        public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
        public ICollection<Expense> PaidExpenses { get; set; } = new List<Expense>();
        public ICollection<ExpenseSplit> ExpenseSplits { get; set; } = new List<ExpenseSplit>();
    }
}
