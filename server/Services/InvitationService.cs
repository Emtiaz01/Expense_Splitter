using Microsoft.EntityFrameworkCore;
using ExpenseSplitter.Data;
using ExpenseSplitter.DTOs;
using ExpenseSplitter.Models;
using System.Security.Cryptography;

namespace ExpenseSplitter.Services
{
    public interface IInvitationService
    {
        Task<InvitationDto> SendInvitationAsync(int groupId, string email, int invitedByUserId);
        Task<InvitationDto?> GetInvitationByTokenAsync(string token);
        Task<bool> AcceptInvitationAsync(string token, int userId);
        Task<List<InvitationDto>> GetPendingInvitationsAsync(string email);
    }

    public class InvitationService : IInvitationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public InvitationService(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<InvitationDto> SendInvitationAsync(int groupId, string email, int invitedByUserId)
        {
            // Check if group exists
            var group = await _context.Groups
                .Include(g => g.GroupMembers)
                .FirstOrDefaultAsync(g => g.GroupId == groupId);

            if (group == null)
                throw new Exception("Group not found");

            // Check if user already exists and is already a member
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (existingUser != null)
            {
                var isMember = await _context.GroupMembers
                    .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == existingUser.UserId);

                if (isMember)
                    throw new Exception("User is already a member of this group");
            }

            // Check if there's already a pending invitation - if yes, invalidate old ones
            var existingInvitations = await _context.Invitations
                .Where(i => i.GroupId == groupId && 
                           i.Email.ToLower() == email.ToLower() && 
                           !i.IsAccepted && 
                           i.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            // Mark all existing pending invitations as expired (superseded by new invitation)
            foreach (var oldInvitation in existingInvitations)
            {
                oldInvitation.ExpiresAt = DateTime.UtcNow.AddSeconds(-1); // Expire immediately
            }

            // Get inviter details
            var inviter = await _context.Users.FindAsync(invitedByUserId);
            if (inviter == null)
                throw new Exception("Inviter not found");

            // Create invitation
            var token = GenerateSecureToken();
            var invitation = new Invitation
            {
                GroupId = groupId,
                Email = email.ToLower(),
                Token = token,
                InvitedByUserId = invitedByUserId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _context.Invitations.Add(invitation);
            await _context.SaveChangesAsync();

            // Send email
            await _emailService.SendInvitationEmailAsync(email, group.GroupName, inviter.Name, token);

            return new InvitationDto
            {
                InvitationId = invitation.InvitationId,
                GroupId = invitation.GroupId,
                GroupName = group.GroupName,
                Email = invitation.Email,
                Token = invitation.Token,
                InvitedByName = inviter.Name,
                CreatedAt = invitation.CreatedAt,
                ExpiresAt = invitation.ExpiresAt,
                IsAccepted = invitation.IsAccepted
            };
        }

        public async Task<InvitationDto?> GetInvitationByTokenAsync(string token)
        {
            var invitation = await _context.Invitations
                .Include(i => i.Group)
                .Include(i => i.InvitedByUser)
                .FirstOrDefaultAsync(i => i.Token == token);

            if (invitation == null)
                return null;

            // Check if expired
            if (invitation.ExpiresAt < DateTime.UtcNow)
                return null;

            return new InvitationDto
            {
                InvitationId = invitation.InvitationId,
                GroupId = invitation.GroupId,
                GroupName = invitation.Group.GroupName,
                Email = invitation.Email,
                Token = invitation.Token,
                InvitedByName = invitation.InvitedByUser?.Name ?? "Someone",
                CreatedAt = invitation.CreatedAt,
                ExpiresAt = invitation.ExpiresAt,
                IsAccepted = invitation.IsAccepted
            };
        }

        public async Task<bool> AcceptInvitationAsync(string token, int userId)
        {
            var invitation = await _context.Invitations
                .FirstOrDefaultAsync(i => i.Token == token && !i.IsAccepted);

            if (invitation == null || invitation.ExpiresAt < DateTime.UtcNow)
                return false;

            // Get user details
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.Email.ToLower() != invitation.Email.ToLower())
                return false;

            // Check if already a member
            var existingMember = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == invitation.GroupId && gm.UserId == userId);

            if (existingMember == null)
            {
                // Add as group member
                var groupMember = new GroupMember
                {
                    GroupId = invitation.GroupId,
                    UserId = userId,
                    Role = "Member",
                    JoinedAt = DateTime.UtcNow
                };

                _context.GroupMembers.Add(groupMember);
            }

            // Mark invitation as accepted
            invitation.IsAccepted = true;
            invitation.AcceptedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<InvitationDto>> GetPendingInvitationsAsync(string email)
        {
            var invitations = await _context.Invitations
                .Include(i => i.Group)
                .Include(i => i.InvitedByUser)
                .Where(i => i.Email.ToLower() == email.ToLower() && 
                           !i.IsAccepted && 
                           i.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            return invitations.Select(i => new InvitationDto
            {
                InvitationId = i.InvitationId,
                GroupId = i.GroupId,
                GroupName = i.Group.GroupName,
                Email = i.Email,
                Token = i.Token,
                InvitedByName = i.InvitedByUser?.Name ?? "Someone",
                CreatedAt = i.CreatedAt,
                ExpiresAt = i.ExpiresAt,
                IsAccepted = i.IsAccepted
            }).ToList();
        }

        private string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
    }
}
