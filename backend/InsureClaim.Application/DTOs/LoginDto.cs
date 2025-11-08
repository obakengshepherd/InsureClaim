using System.ComponentModel.DataAnnotations;

namespace InsureClaim.Application.DTOs;

/// <summary>
/// Data Transfer Object for user login
/// Why: Minimal data required for authentication
/// </summary>
public class LoginDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}