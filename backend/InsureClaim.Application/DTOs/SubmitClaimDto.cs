using System.ComponentModel.DataAnnotations;

namespace InsureClaim.Application.DTOs;

/// <summary>
/// DTO for submitting a new insurance claim
/// Why: Validates claim submission data before processing
/// Business Rule: Claims must be linked to active policies
/// </summary>
public class SubmitClaimDto
{
    [Required(ErrorMessage = "Policy ID is required")]
    public int PolicyId { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Claim amount is required")]
    [Range(100, 100000000, ErrorMessage = "Claim amount must be between R100 and R100,000,000")]
    public decimal ClaimAmount { get; set; }

    [Required(ErrorMessage = "Incident date is required")]
    [DataType(DataType.Date)]
    public DateTime IncidentDate { get; set; }

    // Optional: Path to uploaded document (we'll implement upload later)
    public string? DocumentPath { get; set; }
}