using InsureClaim.Application.DTOs;
using InsureClaim.Application.Interfaces;
using InsureClaim.Core.Entities;
using InsureClaim.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace InsureClaim.Application.Services;

/// <summary>
/// Service handling user authentication and registration
/// Why: Encapsulates complex business logic away from controllers
/// Business Impact: Ensures secure user management with proper validation
/// </summary>
public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ApplicationDbContext context,
        IJwtService jwtService,
        ILogger<AuthService> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            _logger.LogInformation("Registration attempt for email: {Email}", registerDto.Email);

            // Check if user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == registerDto.Email.ToLower());

            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed - email already exists: {Email}", registerDto.Email);
                throw new InvalidOperationException("User with this email already exists");
            }

            // Hash password using BCrypt
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Create new user entity
            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email.ToLower(),
                PasswordHash = passwordHash,
                PhoneNumber = registerDto.PhoneNumber,
                Role = (UserRole)registerDto.Role,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Save to database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User registered successfully: {Email} with Role: {Role}", 
                user.Email, user.Role);

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);

            // Return response
            return new AuthResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = token,
                TokenExpiration = _jwtService.GetTokenExpiration()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration for email: {Email}", registerDto.Email);
            throw;
        }
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        try
        {
            _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

            // Find user by email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower());

            if (user == null)
            {
                _logger.LogWarning("Login failed - user not found: {Email}", loginDto.Email);
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            // Verify password
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed - invalid password for: {Email}", loginDto.Email);
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            // Check if account is active
            if (!user.IsActive)
            {
                _logger.LogWarning("Login failed - account inactive: {Email}", loginDto.Email);
                throw new UnauthorizedAccessException("Account is inactive. Please contact support.");
            }

            _logger.LogInformation("User logged in successfully: {Email}", user.Email);

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = token,
                TokenExpiration = _jwtService.GetTokenExpiration()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", loginDto.Email);
            throw;
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        try
        {
            var user = await _context.Users
                .Where(u => u.Id == userId && u.IsActive)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role.ToString(),
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", userId);
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID: {UserId}", userId);
            throw;
        }
    }
}