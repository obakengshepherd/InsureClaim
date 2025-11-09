using System.ComponentModel.DataAnnotations;

namespace InsureClaim.Application.DTOs;

/// <summary>
/// DTO for admin claim review
/// Why: Allows admins to approve/deny claims with notes
/// Business Rule: Only admins can review claims
/// </summary>
public class ReviewClaimDto
{
    [Required(ErrorMessage = "Status is required")]
    [Range(3, 5, ErrorMessage = "Status must be Approved (3), Denied (4), or Paid (5)")]
    public int Status { get; set; }

    [Range(0, 100000000, ErrorMessage = "Approved amount must be between R0 and R100,000,000")]
    public decimal? ApprovedAmount { get; set; }

    [StringLength(1000, ErrorMessage = "Review notes cannot exceed 1000 characters")]
    public string? ReviewNotes { get; set; }
}