using Microsoft.EntityFrameworkCore;
using ExpenseSplitter.Data;
using ExpenseSplitter.Models;
using ExpenseSplitter.DTOs;

namespace ExpenseSplitter.Services
{
    public interface IExpenseService
    {
        Task<List<ExpenseDto>> GetGroupExpensesAsync(int groupId, int userId);
        Task<ExpenseDto> CreateExpenseAsync(CreateExpenseDto dto, int userId);
        Task<ExpenseDto> UpdateExpenseAsync(int expenseId, UpdateExpenseDto dto, int userId);
        Task DeleteExpenseAsync(int expenseId, int userId);
    }

    public class ExpenseService : IExpenseService
    {
        private readonly ApplicationDbContext _context;

        public ExpenseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ExpenseDto>> GetGroupExpensesAsync(int groupId, int userId)
        {
            // Check if user is a member
            var isMember = await _context.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (!isMember)
                throw new UnauthorizedAccessException("You are not a member of this group");

            var expenses = await _context.Expenses
                .Where(e => e.GroupId == groupId)
                .Include(e => e.PaidBy)
                .Include(e => e.ExpenseSplits)
                    .ThenInclude(es => es.User)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return expenses.Select(e => new ExpenseDto
            {
                ExpenseId = e.ExpenseId,
                GroupId = e.GroupId,
                Amount = e.Amount,
                PaidByUserId = e.PaidByUserId,
                PaidByUserName = e.PaidBy.Name,
                Description = e.Description,
                SplitType = e.SplitType,
                CreatedAt = e.CreatedAt,
                Splits = e.ExpenseSplits.Select(es => new ExpenseSplitDto
                {
                    SplitId = es.SplitId,
                    UserId = es.UserId,
                    UserName = es.User.Name,
                    ShareAmount = es.ShareAmount,
                    Percentage = es.Percentage
                }).ToList()
            }).ToList();
        }

        public async Task<ExpenseDto> CreateExpenseAsync(CreateExpenseDto dto, int userId)
        {
            // Check if user is a member
            var isMember = await _context.GroupMembers
                .AnyAsync(gm => gm.GroupId == dto.GroupId && gm.UserId == userId);

            if (!isMember)
                throw new UnauthorizedAccessException("You are not a member of this group");

            // Check if group is closed
            var group = await _context.Groups.FindAsync(dto.GroupId);
            if (group?.IsClosed == true)
                throw new InvalidOperationException("Cannot add expenses to a closed group");

            // Validate splits
            ValidateSplits(dto.Amount, dto.Splits, dto.SplitType);

            // Create expense
            var expense = new Expense
            {
                GroupId = dto.GroupId,
                Amount = dto.Amount,
                PaidByUserId = dto.PaidByUserId,
                Description = dto.Description,
                SplitType = dto.SplitType,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            // Create splits
            foreach (var split in dto.Splits)
            {
                var expenseSplit = new ExpenseSplit
                {
                    ExpenseId = expense.ExpenseId,
                    UserId = split.UserId,
                    ShareAmount = split.ShareAmount,
                    Percentage = split.Percentage
                };

                _context.ExpenseSplits.Add(expenseSplit);
            }

            await _context.SaveChangesAsync();

            // Reload with navigation properties
            await _context.Entry(expense).Reference(e => e.PaidBy).LoadAsync();
            await _context.Entry(expense).Collection(e => e.ExpenseSplits).LoadAsync();

            foreach (var split in expense.ExpenseSplits)
            {
                await _context.Entry(split).Reference(es => es.User).LoadAsync();
            }

            return new ExpenseDto
            {
                ExpenseId = expense.ExpenseId,
                GroupId = expense.GroupId,
                Amount = expense.Amount,
                PaidByUserId = expense.PaidByUserId,
                PaidByUserName = expense.PaidBy.Name,
                Description = expense.Description,
                SplitType = expense.SplitType,
                CreatedAt = expense.CreatedAt,
                Splits = expense.ExpenseSplits.Select(es => new ExpenseSplitDto
                {
                    SplitId = es.SplitId,
                    UserId = es.UserId,
                    UserName = es.User.Name,
                    ShareAmount = es.ShareAmount,
                    Percentage = es.Percentage
                }).ToList()
            };
        }

        public async Task<ExpenseDto> UpdateExpenseAsync(int expenseId, UpdateExpenseDto dto, int userId)
        {
            var expense = await _context.Expenses
                .Include(e => e.ExpenseSplits)
                .FirstOrDefaultAsync(e => e.ExpenseId == expenseId);

            if (expense == null)
                throw new KeyNotFoundException("Expense not found");

            // Check if user is admin
            var isAdmin = await _context.GroupMembers
                .AnyAsync(gm => gm.GroupId == expense.GroupId && gm.UserId == userId && gm.Role == "Admin");

            if (!isAdmin)
                throw new UnauthorizedAccessException("Only admins can edit expenses");

            // Validate splits
            ValidateSplits(dto.Amount, dto.Splits, dto.SplitType);

            // Update expense
            expense.Amount = dto.Amount;
            expense.Description = dto.Description;
            expense.SplitType = dto.SplitType;
            expense.UpdatedAt = DateTime.UtcNow;

            // Remove old splits
            _context.ExpenseSplits.RemoveRange(expense.ExpenseSplits);

            // Add new splits
            foreach (var split in dto.Splits)
            {
                var expenseSplit = new ExpenseSplit
                {
                    ExpenseId = expense.ExpenseId,
                    UserId = split.UserId,
                    ShareAmount = split.ShareAmount,
                    Percentage = split.Percentage
                };

                _context.ExpenseSplits.Add(expenseSplit);
            }

            await _context.SaveChangesAsync();

            // Reload with navigation properties
            await _context.Entry(expense).Reference(e => e.PaidBy).LoadAsync();
            await _context.Entry(expense).Collection(e => e.ExpenseSplits).LoadAsync();

            foreach (var split in expense.ExpenseSplits)
            {
                await _context.Entry(split).Reference(es => es.User).LoadAsync();
            }

            return new ExpenseDto
            {
                ExpenseId = expense.ExpenseId,
                GroupId = expense.GroupId,
                Amount = expense.Amount,
                PaidByUserId = expense.PaidByUserId,
                PaidByUserName = expense.PaidBy.Name,
                Description = expense.Description,
                SplitType = expense.SplitType,
                CreatedAt = expense.CreatedAt,
                Splits = expense.ExpenseSplits.Select(es => new ExpenseSplitDto
                {
                    SplitId = es.SplitId,
                    UserId = es.UserId,
                    UserName = es.User.Name,
                    ShareAmount = es.ShareAmount,
                    Percentage = es.Percentage
                }).ToList()
            };
        }

        public async Task DeleteExpenseAsync(int expenseId, int userId)
        {
            var expense = await _context.Expenses.FindAsync(expenseId);

            if (expense == null)
                throw new KeyNotFoundException("Expense not found");

            // Check if user is admin
            var isAdmin = await _context.GroupMembers
                .AnyAsync(gm => gm.GroupId == expense.GroupId && gm.UserId == userId && gm.Role == "Admin");

            if (!isAdmin)
                throw new UnauthorizedAccessException("Only admins can delete expenses");

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
        }

        private void ValidateSplits(decimal amount, List<SplitDto> splits, string splitType)
        {
            if (splits == null || splits.Count == 0)
                throw new InvalidOperationException("At least one participant is required");

            if (splitType == "Unequal")
            {
                var total = splits.Sum(s => s.ShareAmount);
                if (Math.Abs(total - amount) > 0.01m)
                    throw new InvalidOperationException($"Split amounts must equal total amount. Expected: {amount}, Got: {total}");
            }

            if (splitType == "Percentage")
            {
                var totalPercentage = splits.Sum(s => s.Percentage ?? 0);
                if (Math.Abs(totalPercentage - 100) > 0.01m)
                    throw new InvalidOperationException($"Percentages must equal 100%. Got: {totalPercentage}%");
            }
        }
    }
}
