using InsureClaim.Application.DTOs;

namespace InsureClaim.Application.Interfaces;

/// <summary>
/// Interface for payment operations
/// Why: Abstracts payment processing logic from controllers
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Records a new payment for a policy
    /// </summary>
    Task<PaymentDto> RecordPaymentAsync(RecordPaymentDto recordPaymentDto);

    /// <summary>
    /// Gets all payments (Admin sees all, Customer sees only their own)
    /// </summary>
    Task<List<PaymentDto>> GetAllPaymentsAsync(int? userId = null, string? userRole = null);

    /// <summary>
    /// Gets a single payment by ID
    /// </summary>
    Task<PaymentDto?> GetPaymentByIdAsync(int paymentId);

    /// <summary>
    /// Updates payment status
    /// </summary>
    Task<PaymentDto?> UpdatePaymentAsync(int paymentId, UpdatePaymentDto updatePaymentDto);

    /// <summary>
    /// Gets all payments for a specific policy
    /// </summary>
    Task<List<PaymentDto>> GetPolicyPaymentsAsync(int policyId);

    /// <summary>
    /// Gets payment statistics for dashboard
    /// </summary>
    Task<object> GetPaymentStatisticsAsync();
}