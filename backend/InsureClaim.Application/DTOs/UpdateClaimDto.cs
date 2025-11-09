using System.ComponentModel.DataAnnotations;

namespace InsureClaim.Application.DTOs;

/// <summary>
/// DTO for updating claim status (Admin only)
/// Why: Allows admins to review and process claims
/// Business Rule: Only admins can change claim status
/// </summary>
public class UpdateClaimDto
{
    [Required(ErrorMessage = "Status is required")]
    [Range(1, 5, ErrorMessage = "Status must be Submitted (1), UnderReview (2), Approved (3), Denied (4), or Paid (5)")]
    public int Status { get; set; }

    [Range(0, 100000000, ErrorMessage = "Approved amount must be between R0 and R100,000,000")]
    public decimal? ApprovedAmount { get; set; }

    [StringLength(1000, ErrorMessage = "Review notes cannot exceed 1000 characters")]
    public string? ReviewNotes { get; set; }
}