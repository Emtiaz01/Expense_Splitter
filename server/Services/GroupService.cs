using Microsoft.EntityFrameworkCore;
using ExpenseSplitter.Data;
using ExpenseSplitter.Models;
using ExpenseSplitter.DTOs;

namespace ExpenseSplitter.Services
{
    public interface IGroupService
    {
        Task<List<GroupDto>> GetUserGroupsAsync(int userId);
        Task<GroupDto> GetGroupByIdAsync(int groupId, int userId);
        Task<GroupDto> CreateGroupAsync(CreateGroupDto dto, int creatorId);
        Task AddMemberAsync(int groupId, int userId, int requestingUserId);
        Task RemoveMemberAsync(int groupId, int memberId, int requestingUserId);
        Task CloseGroupAsync(int groupId, int requestingUserId);
    }

    public class GroupService : IGroupService
    {
        private readonly ApplicationDbContext _context;

        public GroupService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GroupDto>> GetUserGroupsAsync(int userId)
        {
            var groups = await _context.GroupMembers
                .Where(gm => gm.UserId == userId)
                .Include(gm => gm.Group)
                    .ThenInclude(g => g.Creator)
                .Include(gm => gm.Group)
                    .ThenInclude(g => g.GroupMembers)
                        .ThenInclude(m => m.User)
                .Include(gm => gm.Group)
                    .ThenInclude(g => g.Expenses)
                .Select(gm => gm.Group)
                .ToListAsync();

            var groupDtos = new List<GroupDto>();

            foreach (var group in groups)
            {
                var totalExpenses = group.Expenses.Sum(e => e.Amount);
                var userBalance = await CalculateUserBalance(group.GroupId, userId);

                groupDtos.Add(new GroupDto
                {
                    GroupId = group.GroupId,
                    GroupName = group.GroupName,
                    Description = group.Description,
                    CreatedBy = group.CreatedBy,
                    CreatorName = group.Creator.Name,
                    CreatedAt = group.CreatedAt,
                    IsClosed = group.IsClosed,
                    MemberCount = group.GroupMembers.Count,
                    ExpenseCount = group.Expenses.Count,
                    TotalExpenses = totalExpenses,
                    UserBalance = userBalance
                });
            }

            return groupDtos;
        }

        public async Task<GroupDto> GetGroupByIdAsync(int groupId, int userId)
        {
            var group = await _context.Groups
                .Include(g => g.Creator)
                .Include(g => g.GroupMembers)
                    .ThenInclude(gm => gm.User)
                .Include(g => g.Expenses)
                .FirstOrDefaultAsync(g => g.GroupId == groupId);

            if (group == null)
                throw new KeyNotFoundException("Group not found");

            // Check if user is a member
            if (!group.GroupMembers.Any(gm => gm.UserId == userId))
                throw new UnauthorizedAccessException("You are not a member of this group");

            var totalExpenses = group.Expenses.Sum(e => e.Amount);

            return new GroupDto
            {
                GroupId = group.GroupId,
                GroupName = group.GroupName,
                Description = group.Description,
                CreatedBy = group.CreatedBy,
                CreatorName = group.Creator.Name,
                CreatedAt = group.CreatedAt,
                IsClosed = group.IsClosed,
                MemberCount = group.GroupMembers.Count,
                ExpenseCount = group.Expenses.Count,
                TotalExpenses = totalExpenses,
                Members = group.GroupMembers.Select(gm => new GroupMemberDto
                {
                    GroupMemberId = gm.GroupMemberId,
                    UserId = gm.UserId,
                    Name = gm.User.Name,
                    Email = gm.User.Email,
                    Role = gm.Role,
                    JoinedAt = gm.JoinedAt
                }).ToList()
            };
        }

        public async Task<GroupDto> CreateGroupAsync(CreateGroupDto dto, int creatorId)
        {
            var group = new Group
            {
                GroupName = dto.GroupName,
                Description = dto.Description,
                CreatedBy = creatorId,
                CreatedAt = DateTime.UtcNow,
                IsClosed = false
            };

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            // Add creator as admin member
            var groupMember = new GroupMember
            {
                GroupId = group.GroupId,
                UserId = creatorId,
                Role = "Admin",
                JoinedAt = DateTime.UtcNow
            };

            _context.GroupMembers.Add(groupMember);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            await _context.Entry(group).Reference(g => g.Creator).LoadAsync();

            return new GroupDto
            {
                GroupId = group.GroupId,
                GroupName = group.GroupName,
                Description = group.Description,
                CreatedBy = group.CreatedBy,
                CreatorName = group.Creator.Name,
                CreatedAt = group.CreatedAt,
                IsClosed = group.IsClosed,
                MemberCount = 1,
                ExpenseCount = 0,
                TotalExpenses = 0
            };
        }

        public async Task AddMemberAsync(int groupId, int userId, int requestingUserId)
        {
            // Check if requesting user is admin
            var isAdmin = await _context.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == requestingUserId && gm.Role == "Admin");

            if (!isAdmin)
                throw new UnauthorizedAccessException("Only admins can add members");

            // Check if user already exists in group
            var exists = await _context.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (exists)
                throw new InvalidOperationException("User is already a member of this group");

            var groupMember = new GroupMember
            {
                GroupId = groupId,
                UserId = userId,
                Role = "Member",
                JoinedAt = DateTime.UtcNow
            };

            _context.GroupMembers.Add(groupMember);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveMemberAsync(int groupId, int memberId, int requestingUserId)
        {
            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupMemberId == memberId && gm.GroupId == groupId);

            if (member == null)
                throw new KeyNotFoundException("Member not found");

            // Check if requesting user is admin
            var isAdmin = await _context.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == requestingUserId && gm.Role == "Admin");

            if (!isAdmin && member.UserId != requestingUserId)
                throw new UnauthorizedAccessException("Only admins can remove other members");

            _context.GroupMembers.Remove(member);
            await _context.SaveChangesAsync();
        }

        public async Task CloseGroupAsync(int groupId, int requestingUserId)
        {
            var group = await _context.Groups.FindAsync(groupId);

            if (group == null)
                throw new KeyNotFoundException("Group not found");

            // Check if requesting user is admin
            var isAdmin = await _context.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == requestingUserId && gm.Role == "Admin");

            if (!isAdmin)
                throw new UnauthorizedAccessException("Only admins can close groups");

            group.IsClosed = true;
            await _context.SaveChangesAsync();
        }

        private async Task<decimal> CalculateUserBalance(int groupId, int userId)
        {
            var paid = await _context.Expenses
                .Where(e => e.GroupId == groupId && e.PaidByUserId == userId)
                .SumAsync(e => e.Amount);

            var share = await _context.ExpenseSplits
                .Where(es => es.UserId == userId && es.Expense.GroupId == groupId)
                .SumAsync(es => es.ShareAmount);

            return paid - share;
        }
    }
}
