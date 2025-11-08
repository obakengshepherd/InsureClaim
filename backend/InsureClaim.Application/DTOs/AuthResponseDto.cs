namespace InsureClaim.Application.DTOs;

/// <summary>
/// Response after successful login or registration
/// Why: Returns JWT token + user info to the client
/// Business Impact: Client stores token for subsequent authenticated requests
/// </summary>
public class AuthResponseDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // Customer, Agent, Admin
    public string Token { get; set; } = string.Empty; // JWT token
    public DateTime TokenExpiration { get; set; }
}