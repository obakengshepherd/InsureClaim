using System.ComponentModel.DataAnnotations;

namespace InsureClaim.Application.DTOs;

/// <summary>
/// DTO for updating an existing policy
/// Why: Allows modification of policy details and status
/// Business Rule: Only certain fields can be updated after creation
/// </summary>
public class UpdatePolicyDto
{
    [Range(1000, 100000000, ErrorMessage = "Coverage amount must be between R1,000 and R100,000,000")]
    public decimal? CoverageAmount { get; set; }

    [Range(1, 4, ErrorMessage = "Status must be Active (1), Expired (2), Cancelled (3), or Suspended (4)")]
    public int? Status { get; set; }

    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }
}