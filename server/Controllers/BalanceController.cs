using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseSplitter.DTOs;
using ExpenseSplitter.Services;
using System.Security.Claims;

namespace ExpenseSplitter.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/groups/{groupId}")]
    public class BalanceController : ControllerBase
    {
        private readonly IBalanceService _balanceService;

        public BalanceController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        [HttpGet("balances")]
        public async Task<ActionResult<List<BalanceDto>>> GetGroupBalances(int groupId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var balances = await _balanceService.GetGroupBalancesAsync(groupId, userId);
                return Ok(balances);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpGet("settlements")]
        public async Task<ActionResult<List<SettlementDto>>> GetGroupSettlements(int groupId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var settlements = await _balanceService.GetGroupSettlementsAsync(groupId, userId);
                return Ok(settlements);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPost("settle")]
        public async Task<ActionResult<SettlementDto>> CreateSettlement(int groupId, [FromBody] CreateSettlementDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var settlement = await _balanceService.CreateSettlementAsync(groupId, dto, userId);
                return Ok(settlement);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}
