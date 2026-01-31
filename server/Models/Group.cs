using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseSplitter.Models
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }

        [Required]
        [MaxLength(200)]
        public string GroupName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsClosed { get; set; } = false;

        // Navigation properties
        [ForeignKey("CreatedBy")]
        public User Creator { get; set; } = null!;
        public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<Settlement> Settlements { get; set; } = new List<Settlement>();
    }
}
