using System.ComponentModel.DataAnnotations;

namespace ExpenseSplitter.DTOs
{
    public class CreateGroupDto
    {
        [Required]
        [MaxLength(200)]
        public string GroupName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }
    }

    public class GroupDto
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CreatedBy { get; set; }
        public string CreatorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsClosed { get; set; }
        public int MemberCount { get; set; }
        public int ExpenseCount { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal? UserBalance { get; set; }
        public List<GroupMemberDto> Members { get; set; } = new();
    }

    public class GroupMemberDto
    {
        public int GroupMemberId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
    }

    public class AddMemberDto
    {
        [Required]
        public int UserId { get; set; }
    }
}
