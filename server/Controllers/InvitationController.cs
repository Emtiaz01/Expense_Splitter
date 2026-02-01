using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseSplitter.DTOs;
using ExpenseSplitter.Services;
using System.Security.Claims;

namespace ExpenseSplitter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvitationController : ControllerBase
    {
        private readonly IInvitationService _invitationService;

        public InvitationController(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        [HttpPost("send/{groupId}")]
        [Authorize]
        public async Task<ActionResult<InvitationDto>> SendInvitation(int groupId, [FromBody] SendInvitationDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var invitation = await _invitationService.SendInvitationAsync(groupId, dto.Email, userId);
                return Ok(invitation);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("verify/{token}")]
        public async Task<ActionResult<InvitationDto>> VerifyInvitation(string token)
        {
            var invitation = await _invitationService.GetInvitationByTokenAsync(token);
            
            if (invitation == null)
                return NotFound(new { message = "Invitation not found or expired" });

            return Ok(invitation);
        }

        [HttpPost("accept")]
        [Authorize]
        public async Task<ActionResult> AcceptInvitation([FromBody] AcceptInvitationDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var success = await _invitationService.AcceptInvitationAsync(dto.Token, userId);

                if (!success)
                    return BadRequest(new { message = "Failed to accept invitation" });

                return Ok(new { message = "Invitation accepted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("pending")]
        [Authorize]
        public async Task<ActionResult<List<InvitationDto>>> GetPendingInvitations()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
            var invitations = await _invitationService.GetPendingInvitationsAsync(email);
            return Ok(invitations);
        }
    }
}
