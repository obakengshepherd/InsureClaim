using InsureClaim.Core.Entities;

namespace InsureClaim.Application.Interfaces;

/// <summary>
/// Interface for JWT token generation and validation
/// Why: Abstraction allows easy testing and future token provider changes
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JWT token for authenticated user
    /// </summary>
    /// <param name="user">The authenticated user</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(User user);

    /// <summary>
    /// Gets token expiration time
    /// </summary>
    DateTime GetTokenExpiration();
}