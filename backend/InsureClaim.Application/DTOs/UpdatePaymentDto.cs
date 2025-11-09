using System.ComponentModel.DataAnnotations;

namespace InsureClaim.Application.DTOs;

/// <summary>
/// DTO for updating payment status
/// Why: Allows marking payments as completed, failed, or refunded
/// Business Rule: Only admins can update payment status
/// </summary>
public class UpdatePaymentDto
{
    [Required(ErrorMessage = "Status is required")]
    [Range(1, 4, ErrorMessage = "Status must be Pending (1), Completed (2), Failed (3), or Refunded (4)")]
    public int Status { get; set; }

    [StringLength(200, ErrorMessage = "Reference cannot exceed 200 characters")]
    public string? Reference { get; set; }
}