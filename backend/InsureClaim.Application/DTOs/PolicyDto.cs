namespace InsureClaim.Application.DTOs;

/// <summary>
/// DTO for policy response
/// Why: Returns policy information to the client
/// </summary>
public class PolicyDto
{
    public int Id { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty; // For display
    public string Type { get; set; } = string.Empty; // Life, Auto, Health, Property
    public decimal CoverageAmount { get; set; }
    public decimal PremiumAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty; // Active, Expired, etc.
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}