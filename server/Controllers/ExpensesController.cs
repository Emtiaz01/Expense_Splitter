using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseSplitter.DTOs;
using ExpenseSplitter.Services;
using System.Security.Claims;

namespace ExpenseSplitter.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        [HttpPost]
        public async Task<ActionResult<ExpenseDto>> CreateExpense([FromBody] CreateExpenseDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var expense = await _expenseService.CreateExpenseAsync(dto, userId);
                return CreatedAtAction(nameof(GetExpense), new { id = expense.ExpenseId }, expense);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseDto>> GetExpense(int id)
        {
            // This would need implementation in the service
            return NotFound();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ExpenseDto>> UpdateExpense(int id, [FromBody] UpdateExpenseDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var expense = await _expenseService.UpdateExpenseAsync(id, dto, userId);
                return Ok(expense);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _expenseService.DeleteExpenseAsync(id, userId);
                return Ok(new { message = "Expense deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }

    [Authorize]
    [ApiController]
    [Route("api/groups/{groupId}/expenses")]
    public class GroupExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public GroupExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        [HttpGet]
        public async Task<ActionResult<List<ExpenseDto>>> GetGroupExpenses(int groupId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var expenses = await _expenseService.GetGroupExpensesAsync(groupId, userId);
                return Ok(expenses);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}
