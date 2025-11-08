using InsureClaim.Application.DTOs;

namespace InsureClaim.Application.Interfaces;

/// <summary>
/// Interface for authentication operations
/// Why: Decouples business logic from API controllers
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user in the system
    /// </summary>
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);

    /// <summary>
    /// Authenticates user and returns JWT token
    /// </summary>
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);

    /// <summary>
    /// Gets user profile by ID
    /// </summary>
    Task<UserDto?> GetUserByIdAsync(int userId);
}