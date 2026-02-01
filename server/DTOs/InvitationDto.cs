using System.ComponentModel.DataAnnotations;

namespace ExpenseSplitter.DTOs
{
    public class InvitationDto
    {
        public int InvitationId { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string InvitedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsAccepted { get; set; }
    }

    public class SendInvitationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class AcceptInvitationDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
