using InsureClaim.Application.DTOs;
using InsureClaim.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsureClaim.API.Controllers;

/// <summary>
/// Handles insurance claim operations
/// Why: RESTful endpoints for submitting and managing claims
/// Business Impact: Enables customers to file claims and admins to process them efficiently
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints require authentication
public class ClaimController : ControllerBase
{
    private readonly IClaimService _claimService;
    private readonly ILogger<ClaimController> _logger;

    public ClaimController(IClaimService claimService, ILogger<ClaimController> logger)
    {
        _claimService = claimService;
        _logger = logger;
    }

    /// <summary>
    /// Submit a new insurance claim
    /// </summary>
    /// <param name="submitClaimDto">Claim details</param>
    /// <returns>Created claim</returns>
    /// <response code="201">Claim submitted successfully</response>
    /// <response code="400">Invalid input or business rule violation</response>
    /// <response code="401">Not authenticated</response>
    [HttpPost]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitClaim([FromBody] SubmitClaimDto submitClaimDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get current user ID from token
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var claim = await _claimService.SubmitClaimAsync(currentUserId, submitClaimDto);

            _logger.LogInformation("Claim submitted: {ClaimNumber} by user {UserId}", 
                claim.ClaimNumber, currentUserId);

            return CreatedAtAction(
                nameof(GetClaimById),
                new { id = claim.Id },
                claim
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting claim");
            return StatusCode(500, new { message = "An error occurred while submitting the claim" });
        }
    }

    /// <summary>
    /// Get all claims (Admin sees all, Customer sees only their own)
    /// </summary>
    /// <returns>List of claims</returns>
    /// <response code="200">Claims retrieved successfully</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ClaimDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllClaims()
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            var claims = await _claimService.GetAllClaimsAsync(currentUserId, userRole);

            return Ok(claims);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving claims");
            return StatusCode(500, new { message = "An error occurred while retrieving claims" });
        }
    }

    /// <summary>
    /// Get a single claim by ID
    /// </summary>
    /// <param name="id">Claim ID</param>
    /// <returns>Claim details</returns>
    /// <response code="200">Claim found</response>
    /// <response code="404">Claim not found</response>
    /// <response code="403">Not authorized to view this claim</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetClaimById(int id)
    {
        try
        {
            var claim = await _claimService.GetClaimByIdAsync(id);

            if (claim == null)
            {
                return NotFound(new { message = $"Claim with ID {id} not found" });
            }

            // Authorization: Users can only view their own claims (unless Admin)
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            if (userRole != "Admin" && claim.UserId != currentUserId)
            {
                return Forbid();
            }

            return Ok(claim);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving claim {ClaimId}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Update claim status and review information
    /// Requires: Admin role
    /// </summary>
    /// <param name="id">Claim ID</param>
    /// <param name="updateClaimDto">Updated claim details</param>
    /// <returns>Updated claim</returns>
    /// <response code="200">Claim updated successfully</response>
    /// <response code="404">Claim not found</response>
    /// <response code="403">Not authorized (Admin only)</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")] // Only admins can update claims
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateClaim(int id, [FromBody] UpdateClaimDto updateClaimDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var claim = await _claimService.UpdateClaimAsync(id, updateClaimDto);

            if (claim == null)
            {
                return NotFound(new { message = $"Claim with ID {id} not found" });
            }

            _logger.LogInformation("Claim updated: {ClaimNumber} to status {Status}", 
                claim.ClaimNumber, claim.Status);

            return Ok(claim);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating claim {ClaimId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the claim" });
        }
    }

    /// <summary>
    /// Get all claims for a specific policy
    /// </summary>
    /// <param name="policyId">Policy ID</param>
    /// <returns>List of claims for the policy</returns>
    /// <response code="200">Claims retrieved successfully</response>
    [HttpGet("policy/{policyId}")]
    [ProducesResponseType(typeof(List<ClaimDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPolicyClaims(int policyId)
    {
        try
        {
            var claims = await _claimService.GetPolicyClaimsAsync(policyId);

            return Ok(claims);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving claims for policy {PolicyId}", policyId);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Get all claims for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user's claims</returns>
    /// <response code="200">Claims retrieved successfully</response>
    /// <response code="403">Not authorized to view other users' claims</response>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(List<ClaimDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUserClaims(int userId)
    {
        try
        {
            // Authorization: Users can only view their own claims (unless Admin)
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            if (userRole != "Admin" && userId != currentUserId)
            {
                return Forbid();
            }

            var claims = await _claimService.GetUserClaimsAsync(userId);

            return Ok(claims);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving claims for user {UserId}", userId);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Get claims statistics for dashboard
    /// Requires: Admin role
    /// </summary>
    /// <returns>Claims statistics</returns>
    /// <response code="200">Statistics retrieved successfully</response>
    /// <response code="403">Not authorized (Admin only)</response>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetClaimsStatistics()
    {
        try
        {
            var statistics = await _claimService.GetClaimsStatisticsAsync();

            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving claims statistics");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }
}