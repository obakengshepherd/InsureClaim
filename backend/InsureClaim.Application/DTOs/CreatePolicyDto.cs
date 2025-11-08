using System.ComponentModel.DataAnnotations;
using InsureClaim.Core.Entities;

namespace InsureClaim.Application.DTOs;

/// <summary>
/// DTO for creating a new insurance policy
/// Why: Validates policy creation data before processing
/// Business Rule: Coverage amount determines premium calculation
/// </summary>
public class CreatePolicyDto
{
    [Required(ErrorMessage = "User ID is required")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Policy type is required")]
    [Range(1, 4, ErrorMessage = "Policy type must be Life (1), Auto (2), Health (3), or Property (4)")]
    public int Type { get; set; }

    [Required(ErrorMessage = "Coverage amount is required")]
    [Range(1000, 100000000, ErrorMessage = "Coverage amount must be between R1,000 and R100,000,000")]
    public decimal CoverageAmount { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Policy duration is required")]
    [Range(1, 60, ErrorMessage = "Policy duration must be between 1 and 60 months")]
    public int DurationMonths { get; set; } // How many months the policy lasts
}