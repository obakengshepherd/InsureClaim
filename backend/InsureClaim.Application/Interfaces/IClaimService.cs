using InsureClaim.Application.DTOs;

namespace InsureClaim.Application.Interfaces;

/// <summary>
/// Interface for insurance claim operations
/// Why: Abstracts claim processing logic from controllers
/// </summary>
public interface IClaimService
{
    /// <summary>
    /// Submits a new insurance claim
    /// </summary>
    Task<ClaimDto> SubmitClaimAsync(int userId, SubmitClaimDto submitClaimDto);

    /// <summary>
    /// Gets all claims (Admin sees all, Customer sees only their own)
    /// </summary>
    Task<List<ClaimDto>> GetAllClaimsAsync(int? userId = null, string? userRole = null);

    /// <summary>
    /// Gets a single claim by ID
    /// </summary>
    Task<ClaimDto?> GetClaimByIdAsync(int claimId);

    /// <summary>
    /// Updates claim status and review information (Admin only)
    /// </summary>
    Task<ClaimDto?> UpdateClaimAsync(int claimId, UpdateClaimDto updateClaimDto);

    /// <summary>
    /// Gets all claims for a specific policy
    /// </summary>
    Task<List<ClaimDto>> GetPolicyClaimsAsync(int policyId);

    /// <summary>
    /// Gets all claims for a specific user
    /// </summary>
    Task<List<ClaimDto>> GetUserClaimsAsync(int userId);

    /// <summary>
    /// Gets claims statistics for dashboard
    /// </summary>
    Task<object> GetClaimsStatisticsAsync();
}