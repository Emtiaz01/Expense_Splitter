using Microsoft.EntityFrameworkCore;
using ExpenseSplitter.Data;
using ExpenseSplitter.DTOs;

namespace ExpenseSplitter.Services
{
    public interface IBalanceService
    {
        Task<List<BalanceDto>> GetGroupBalancesAsync(int groupId, int userId);
        Task<List<SettlementDto>> GetGroupSettlementsAsync(int groupId, int userId);
        Task<SettlementDto> CreateSettlementAsync(int groupId, CreateSettlementDto dto, int userId);
    }

    public class BalanceService : IBalanceService
    {
        private readonly ApplicationDbContext _context;

        public BalanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BalanceDto>> GetGroupBalancesAsync(int groupId, int userId)
        {
            // Check if user is a member
            var isMember = await _context.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (!isMember)
                throw new UnauthorizedAccessException("You are not a member of this group");

            var members = await _context.GroupMembers
                .Where(gm => gm.GroupId == groupId)
                .Include(gm => gm.User)
                .ToListAsync();

            var balances = new List<BalanceDto>();

            foreach (var member in members)
            {
                var totalPaid = await _context.Expenses
                    .Where(e => e.GroupId == groupId && e.PaidByUserId == member.UserId)
                    .SumAsync(e => e.Amount);

                var totalShare = await _context.ExpenseSplits
                    .Where(es => es.UserId == member.UserId && es.Expense.GroupId == groupId)
                    .SumAsync(es => es.ShareAmount);

                var balance = totalPaid - totalShare;

                balances.Add(new BalanceDto
                {
                    UserId = member.UserId,
                    UserName = member.User.Name,
                    TotalPaid = totalPaid,
                    TotalShare = totalShare,
                    Balance = balance
                });
            }

            return balances.OrderByDescending(b => b.Balance).ToList();
        }

        public async Task<List<SettlementDto>> GetGroupSettlementsAsync(int groupId, int userId)
        {
            // Check if user is a member
            var isMember = await _context.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (!isMember)
                throw new UnauthorizedAccessException("You are not a member of this group");

            var settlements = await _context.Settlements
                .Where(s => s.GroupId == groupId)
                .Include(s => s.FromUser)
                .Include(s => s.ToUser)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return settlements.Select(s => new SettlementDto
            {
                SettlementId = s.SettlementId,
                FromUserId = s.FromUserId,
                FromUserName = s.FromUser.Name,
                ToUserId = s.ToUserId,
                ToUserName = s.ToUser.Name,
                Amount = s.Amount,
                CreatedAt = s.CreatedAt,
                Note = s.Note
            }).ToList();
        }

        public async Task<SettlementDto> CreateSettlementAsync(int groupId, CreateSettlementDto dto, int userId)
        {
            // Check if user is a member
            var isMember = await _context.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (!isMember)
                throw new UnauthorizedAccessException("You are not a member of this group");

            // Verify that the fromUser is the current user or user is admin
            var isAdmin = await _context.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId && gm.Role == "Admin");

            if (dto.FromUserId != userId && !isAdmin)
                throw new UnauthorizedAccessException("You can only record settlements for yourself");

            var settlement = new Models.Settlement
            {
                FromUserId = dto.FromUserId,
                ToUserId = dto.ToUserId,
                Amount = dto.Amount,
                GroupId = groupId,
                CreatedAt = DateTime.UtcNow,
                Note = dto.Note
            };

            _context.Settlements.Add(settlement);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            await _context.Entry(settlement).Reference(s => s.FromUser).LoadAsync();
            await _context.Entry(settlement).Reference(s => s.ToUser).LoadAsync();

            return new SettlementDto
            {
                SettlementId = settlement.SettlementId,
                FromUserId = settlement.FromUserId,
                FromUserName = settlement.FromUser.Name,
                ToUserId = settlement.ToUserId,
                ToUserName = settlement.ToUser.Name,
                Amount = settlement.Amount,
                CreatedAt = settlement.CreatedAt,
                Note = settlement.Note
            };
        }
    }
}
