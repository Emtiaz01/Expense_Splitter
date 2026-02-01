        using Microsoft.AspNetCore.Authorization;
        using Microsoft.AspNetCore.Mvc;
        using ExpenseSplitter.DTOs;
        using ExpenseSplitter.Services;
        using System.Security.Claims;

namespace ExpenseSplitter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var response = await _authService.RegisterAsync(dto);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            try
            {
                var response = await _authService.LoginAsync(dto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var user = await _authService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new UserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email
            });
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<ActionResult<UserDto>> SearchUserByEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { message = "Email is required" });

            var user = await _authService.GetUserByEmailAsync(email);

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new UserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email
            });
        }
    }
}
