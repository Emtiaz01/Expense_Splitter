using Microsoft.EntityFrameworkCore;
using ExpenseSplitter.Data;
using ExpenseSplitter.Models;
using ExpenseSplitter.DTOs;

namespace ExpenseSplitter.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthService(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            // Check if user exists
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                throw new InvalidOperationException("User with this email already exists");
            }

            // Create user
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate token
            var token = _jwtService.GenerateToken(user);

            return new AuthResponseDto
            {
                User = new UserDto
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email
                },
                Token = token
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            // Find user
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            // Generate token
            var token = _jwtService.GenerateToken(user);

            return new AuthResponseDto
            {
                User = new UserDto
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email
                },
                Token = token
            };
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }
    }
}
