using InsureClaim.Application.DTOs;
using InsureClaim.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsureClaim.API.Controllers;

/// <summary>
/// Handles payment operations and transaction history
/// Why: RESTful endpoints for recording and managing payments
/// Business Impact: Provides complete financial audit trail and revenue tracking
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints require authentication
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Record a new payment for a policy
    /// </summary>
    /// <param name="recordPaymentDto">Payment details</param>
    /// <returns>Recorded payment</returns>
    /// <response code="201">Payment recorded successfully</response>
    /// <response code="400">Invalid input</response>
    /// <response code="401">Not authenticated</response>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecordPayment([FromBody] RecordPaymentDto recordPaymentDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var payment = await _paymentService.RecordPaymentAsync(recordPaymentDto);

            _logger.LogInformation("Payment recorded: {TransactionId} for policy {PolicyId}", 
                payment.TransactionId, payment.PolicyId);

            return CreatedAtAction(
                nameof(GetPaymentById),
                new { id = payment.Id },
                payment
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording payment");
            return StatusCode(500, new { message = "An error occurred while recording the payment" });
        }
    }

    /// <summary>
    /// Get all payments (Admin sees all, Customer sees only their own)
    /// </summary>
    /// <returns>List of payments</returns>
    /// <response code="200">Payments retrieved successfully</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPayments()
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

            var payments = await _paymentService.GetAllPaymentsAsync(currentUserId, userRole);

            return Ok(payments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments");
            return StatusCode(500, new { message = "An error occurred while retrieving payments" });
        }
    }

    /// <summary>
    /// Get a single payment by ID
    /// </summary>
    /// <param name="id">Payment ID</param>
    /// <returns>Payment details</returns>
    /// <response code="200">Payment found</response>
    /// <response code="404">Payment not found</response>
    /// <response code="403">Not authorized to view this payment</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPaymentById(int id)
    {
        try
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);

            if (payment == null)
            {
                return NotFound(new { message = $"Payment with ID {id} not found" });
            }

            // Authorization check can be added here if needed

            return Ok(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment {PaymentId}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Update payment status
    /// Requires: Admin role
    /// </summary>
    /// <param name="id">Payment ID</param>
    /// <param name="updatePaymentDto">Updated payment details</param>
    /// <returns>Updated payment</returns>
    /// <response code="200">Payment updated successfully</response>
    /// <response code="404">Payment not found</response>
    /// <response code="403">Not authorized (Admin only)</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")] // Only admins can update payments
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdatePayment(int id, [FromBody] UpdatePaymentDto updatePaymentDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var payment = await _paymentService.UpdatePaymentAsync(id, updatePaymentDto);

            if (payment == null)
            {
                return NotFound(new { message = $"Payment with ID {id} not found" });
            }

            _logger.LogInformation("Payment updated: {TransactionId} to status {Status}", 
                payment.TransactionId, payment.Status);

            return Ok(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment {PaymentId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the payment" });
        }
    }

    /// <summary>
    /// Get all payments for a specific policy
    /// </summary>
    /// <param name="policyId">Policy ID</param>
    /// <returns>List of payments for the policy</returns>
    /// <response code="200">Payments retrieved successfully</response>
    [HttpGet("policy/{policyId}")]
    [ProducesResponseType(typeof(List<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPolicyPayments(int policyId)
    {
        try
        {
            var payments = await _paymentService.GetPolicyPaymentsAsync(policyId);

            return Ok(payments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments for policy {PolicyId}", policyId);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Get payment statistics for dashboard
    /// Requires: Admin role
    /// </summary>
    /// <returns>Payment statistics</returns>
    /// <response code="200">Statistics retrieved successfully</response>
    /// <response code="403">Not authorized (Admin only)</response>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPaymentStatistics()
    {
        try
        {
            var statistics = await _paymentService.GetPaymentStatisticsAsync();

            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment statistics");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }
}