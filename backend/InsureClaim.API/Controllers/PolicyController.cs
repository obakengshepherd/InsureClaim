using InsureClaim.Application.DTOs;
using InsureClaim.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsureClaim.API.Controllers;

/// <summary>
/// Handles insurance policy management operations
/// Why: RESTful endpoints for creating and managing policies
/// Business Impact: Enables customers to purchase policies and admins to manage them
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints require authentication
public class PolicyController : ControllerBase
{
    private readonly IPolicyService _policyService;
    private readonly ILogger<PolicyController> _logger;

    public PolicyController(IPolicyService policyService, ILogger<PolicyController> logger)
    {
        _policyService = policyService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new insurance policy
    /// </summary>
    /// <param name="createPolicyDto">Policy details</param>
    /// <returns>Created policy</returns>
    /// <response code="201">Policy created successfully</response>
    /// <response code="400">Invalid input</response>
    /// <response code="401">Not authenticated</response>
    [HttpPost]
    [ProducesResponseType(typeof(PolicyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePolicy([FromBody] CreatePolicyDto createPolicyDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get current user ID from token
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            // Business Rule: Customers can only create policies for themselves
            if (userRole == "Customer" && createPolicyDto.UserId != currentUserId)
            {
                return Forbid(); // 403 Forbidden
            }

            var policy = await _policyService.CreatePolicyAsync(createPolicyDto);

            _logger.LogInformation(
                "Policy created: {PolicyNumber} by user {UserId}",
                policy.PolicyNumber,
                currentUserId
            );

            return CreatedAtAction(
                nameof(GetPolicyById),
                new { id = policy.Id },
                policy
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating policy");
            return StatusCode(500, new { message = "An error occurred while creating the policy" });
        }
    }

    /// <summary>
    /// Get all policies (Admin sees all, Customer sees only their own)
    /// </summary>
    /// <returns>List of policies</returns>
    /// <response code="200">Policies retrieved successfully</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<PolicyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPolicies()
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            var policies = await _policyService.GetAllPoliciesAsync(currentUserId, userRole);

            return Ok(policies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving policies");
            return StatusCode(500, new { message = "An error occurred while retrieving policies" });
        }
    }

    /// <summary>
    /// Get a single policy by ID
    /// </summary>
    /// <param name="id">Policy ID</param>
    /// <returns>Policy details</returns>
    /// <response code="200">Policy found</response>
    /// <response code="404">Policy not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PolicyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPolicyById(int id)
    {
        try
        {
            var policy = await _policyService.GetPolicyByIdAsync(id);

            if (policy == null)
            {
                return NotFound(new { message = $"Policy with ID {id} not found" });
            }

            // Authorization: Users can only view their own policies (unless Admin)
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            if (userRole != "Admin" && policy.UserId != currentUserId)
            {
                return Forbid();
            }

            return Ok(policy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving policy {PolicyId}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Update an existing policy
    /// Requires: Admin role
    /// </summary>
    /// <param name="id">Policy ID</param>
    /// <param name="updatePolicyDto">Updated policy details</param>
    /// <returns>Updated policy</returns>
    /// <response code="200">Policy updated successfully</response>
    /// <response code="404">Policy not found</response>
    /// <response code="403">Not authorized (Admin only)</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")] // Only admins can update policies
    [ProducesResponseType(typeof(PolicyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdatePolicy(int id, [FromBody] UpdatePolicyDto updatePolicyDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var policy = await _policyService.UpdatePolicyAsync(id, updatePolicyDto);

            if (policy == null)
            {
                return NotFound(new { message = $"Policy with ID {id} not found" });
            }

            _logger.LogInformation("Policy updated: {PolicyNumber}", policy.PolicyNumber);

            return Ok(policy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating policy {PolicyId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the policy" });
        }
    }

    /// <summary>
    /// Delete (cancel) a policy
    /// Requires: Admin role
    /// </summary>
    /// <param name="id">Policy ID</param>
    /// <returns>Success status</returns>
    /// <response code="204">Policy cancelled successfully</response>
    /// <response code="404">Policy not found</response>
    /// <response code="403">Not authorized (Admin only)</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] // Only admins can delete policies
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeletePolicy(int id)
    {
        try
        {
            var result = await _policyService.DeletePolicyAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"Policy with ID {id} not found" });
            }

            _logger.LogInformation("Policy deleted: {PolicyId}", id);

            return NoContent(); // 204
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting policy {PolicyId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the policy" });
        }
    }

    /// <summary>
    /// Get policies for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user's policies</returns>
    /// <response code="200">Policies retrieved successfully</response>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(List<PolicyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserPolicies(int userId)
    {
        try
        {
            // Authorization: Users can only view their own policies (unless Admin)
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            if (userRole != "Admin" && userId != currentUserId)
            {
                return Forbid();
            }

            var policies = await _policyService.GetUserPoliciesAsync(userId);

            return Ok(policies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving policies for user {UserId}", userId);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }
}