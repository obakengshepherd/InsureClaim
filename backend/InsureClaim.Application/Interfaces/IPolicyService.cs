using InsureClaim.Application.DTOs;

namespace InsureClaim.Application.Interfaces;

/// <summary>
/// Interface for policy management operations
/// Why: Abstracts business logic from API controllers
/// </summary>
public interface IPolicyService
{
    /// <summary>
    /// Creates a new insurance policy with auto-generated policy number
    /// </summary>
    Task<PolicyDto> CreatePolicyAsync(CreatePolicyDto createPolicyDto);

    /// <summary>
    /// Gets all policies (Admin sees all, Customer sees only their own)
    /// </summary>
    Task<List<PolicyDto>> GetAllPoliciesAsync(int? userId = null, string? userRole = null);

    /// <summary>
    /// Gets a single policy by ID
    /// </summary>
    Task<PolicyDto?> GetPolicyByIdAsync(int policyId);

    /// <summary>
    /// Updates an existing policy
    /// </summary>
    Task<PolicyDto?> UpdatePolicyAsync(int policyId, UpdatePolicyDto updatePolicyDto);

    /// <summary>
    /// Deletes (cancels) a policy
    /// </summary>
    Task<bool> DeletePolicyAsync(int policyId);

    /// <summary>
    /// Gets policies for a specific user
    /// </summary>
    Task<List<PolicyDto>> GetUserPoliciesAsync(int userId);
}