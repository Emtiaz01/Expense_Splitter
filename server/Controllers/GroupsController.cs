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
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        [HttpGet]
        public async Task<ActionResult<List<GroupDto>>> GetUserGroups()
        {
            var userId = GetCurrentUserId();
            var groups = await _groupService.GetUserGroupsAsync(userId);
            return Ok(groups);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDto>> GetGroup(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var group = await _groupService.GetGroupByIdAsync(id, userId);
                return Ok(group);
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

        [HttpPost]
        public async Task<ActionResult<GroupDto>> CreateGroup([FromBody] CreateGroupDto dto)
        {
            var userId = GetCurrentUserId();
            var group = await _groupService.CreateGroupAsync(dto, userId);
            return CreatedAtAction(nameof(GetGroup), new { id = group.GroupId }, group);
        }

        [HttpPost("{id}/members")]
        public async Task<IActionResult> AddMember(int id, [FromBody] AddMemberDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _groupService.AddMemberAsync(id, dto.UserId, userId);
                return Ok(new { message = "Member added successfully" });
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

        [HttpDelete("{id}/members/{memberId}")]
        public async Task<IActionResult> RemoveMember(int id, int memberId)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _groupService.RemoveMemberAsync(id, memberId, userId);
                return Ok(new { message = "Member removed successfully" });
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

        [HttpPut("{id}/close")]
        public async Task<IActionResult> CloseGroup(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _groupService.CloseGroupAsync(id, userId);
                return Ok(new { message = "Group closed successfully" });
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
}
