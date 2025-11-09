using InsureClaim.Application.DTOs;
using InsureClaim.Application.Interfaces;
using InsureClaim.Core.Entities;
using InsureClaim.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InsureClaim.Application.Services;

/// <summary>
/// Service handling payment processing and transaction history
/// Why: Encapsulates payment logic and financial tracking
/// Business Impact: Provides complete audit trail for all financial transactions
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(ApplicationDbContext context, ILogger<PaymentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaymentDto> RecordPaymentAsync(RecordPaymentDto recordPaymentDto)
    {
        try
        {
            _logger.LogInformation("Recording payment for policy {PolicyId}", recordPaymentDto.PolicyId);

            // Verify policy exists
            var policy = await _context.Policies
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == recordPaymentDto.PolicyId);

            if (policy == null)
            {
                throw new InvalidOperationException($"Policy with ID {recordPaymentDto.PolicyId} not found");
            }

            // Business Rule: Can record payments for any policy status (even expired, for arrears)

            // Generate unique transaction ID
            var transactionId = await GenerateTransactionIdAsync();

            // Create payment entity
            var payment = new Payment
            {
                TransactionId = transactionId,
                PolicyId = recordPaymentDto.PolicyId,
                Amount = recordPaymentDto.Amount,
                Method = (PaymentMethod)recordPaymentDto.Method,
                Status = PaymentStatus.Completed, // Assuming successful payment
                PaymentDate = DateTime.UtcNow,
                ProcessedDate = DateTime.UtcNow,
                Reference = recordPaymentDto.Reference,
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Payment recorded successfully: {TransactionId} for policy {PolicyNumber}", 
                transactionId, policy.PolicyNumber);

            // Return DTO
            payment.Policy = policy;
            return MapToPaymentDto(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording payment for policy {PolicyId}", recordPaymentDto.PolicyId);
            throw;
        }
    }

    public async Task<List<PaymentDto>> GetAllPaymentsAsync(int? userId = null, string? userRole = null)
    {
        try
        {
            var query = _context.Payments
                .Include(p => p.Policy)
                    .ThenInclude(pol => pol.User)
                .AsQueryable();

            // If user is Customer, filter by their policies
            if (userId.HasValue && userRole != "Admin")
            {
                query = query.Where(p => p.Policy.UserId == userId.Value);
            }

            var payments = await query
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return payments.Select(p => MapToPaymentDto(p)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments");
            throw;
        }
    }

    public async Task<PaymentDto?> GetPaymentByIdAsync(int paymentId)
    {
        try
        {
            var payment = await _context.Payments
                .Include(p => p.Policy)
                    .ThenInclude(pol => pol.User)
                .FirstOrDefaultAsync(p => p.Id == paymentId);

            if (payment == null)
            {
                _logger.LogWarning("Payment not found with ID: {PaymentId}", paymentId);
                return null;
            }

            return MapToPaymentDto(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment {PaymentId}", paymentId);
            throw;
        }
    }

    public async Task<PaymentDto?> UpdatePaymentAsync(int paymentId, UpdatePaymentDto updatePaymentDto)
    {
        try
        {
            var payment = await _context.Payments
                .Include(p => p.Policy)
                    .ThenInclude(pol => pol.User)
                .FirstOrDefaultAsync(p => p.Id == paymentId);

            if (payment == null)
            {
                _logger.LogWarning("Payment not found with ID: {PaymentId}", paymentId);
                return null;
            }

            // Update status
            var oldStatus = payment.Status;
            payment.Status = (PaymentStatus)updatePaymentDto.Status;

            // If marking as completed, set processed date
            if (payment.Status == PaymentStatus.Completed && oldStatus != PaymentStatus.Completed)
            {
                payment.ProcessedDate = DateTime.UtcNow;
            }

            // Update reference if provided
            if (!string.IsNullOrWhiteSpace(updatePaymentDto.Reference))
            {
                payment.Reference = updatePaymentDto.Reference;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Payment {TransactionId} updated: {OldStatus} → {NewStatus}", 
                payment.TransactionId, oldStatus, payment.Status);

            return MapToPaymentDto(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment {PaymentId}", paymentId);
            throw;
        }
    }

    public async Task<List<PaymentDto>> GetPolicyPaymentsAsync(int policyId)
    {
        try
        {
            var payments = await _context.Payments
                .Include(p => p.Policy)
                    .ThenInclude(pol => pol.User)
                .Where(p => p.PolicyId == policyId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return payments.Select(p => MapToPaymentDto(p)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments for policy {PolicyId}", policyId);
            throw;
        }
    }

    public async Task<object> GetPaymentStatisticsAsync()
    {
        try
        {
            var totalPayments = await _context.Payments.CountAsync();
            var completedPayments = await _context.Payments.CountAsync(p => p.Status == PaymentStatus.Completed);
            var pendingPayments = await _context.Payments.CountAsync(p => p.Status == PaymentStatus.Pending);
            var failedPayments = await _context.Payments.CountAsync(p => p.Status == PaymentStatus.Failed);
            var refundedPayments = await _context.Payments.CountAsync(p => p.Status == PaymentStatus.Refunded);

            var totalAmount = await _context.Payments
                .Where(p => p.Status == PaymentStatus.Completed)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            var totalRefunded = await _context.Payments
                .Where(p => p.Status == PaymentStatus.Refunded)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            // Payments by method
            var creditCardPayments = await _context.Payments
                .Where(p => p.Method == PaymentMethod.CreditCard && p.Status == PaymentStatus.Completed)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            var debitCardPayments = await _context.Payments
                .Where(p => p.Method == PaymentMethod.DebitCard && p.Status == PaymentStatus.Completed)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            var bankTransferPayments = await _context.Payments
                .Where(p => p.Method == PaymentMethod.BankTransfer && p.Status == PaymentStatus.Completed)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            var mobilePayments = await _context.Payments
                .Where(p => p.Method == PaymentMethod.MobilePayment && p.Status == PaymentStatus.Completed)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            return new
            {
                totalPayments,
                byStatus = new
                {
                    completed = completedPayments,
                    pending = pendingPayments,
                    failed = failedPayments,
                    refunded = refundedPayments
                },
                amounts = new
                {
                    totalReceived = totalAmount,
                    totalRefunded = totalRefunded,
                    netRevenue = totalAmount - totalRefunded,
                    successRate = totalPayments > 0 ? (completedPayments / (double)totalPayments * 100) : 0
                },
                byMethod = new
                {
                    creditCard = creditCardPayments,
                    debitCard = debitCardPayments,
                    bankTransfer = bankTransferPayments,
                    mobilePayment = mobilePayments
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment statistics");
            throw;
        }
    }

    // ============= HELPER METHODS =============

    /// <summary>
    /// Generates unique transaction ID in format: TXN-YYYY-NNNNNN
    /// Business Rule: Transaction IDs must be sequential and unique
    /// </summary>
    private async Task<string> GenerateTransactionIdAsync()
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"TXN-{year}-";

        // Get the latest transaction ID for this year
        var latestPayment = await _context.Payments
            .Where(p => p.TransactionId.StartsWith(prefix))
            .OrderByDescending(p => p.TransactionId)
            .FirstOrDefaultAsync();

        int nextNumber = 1;

        if (latestPayment != null)
        {
            // Extract number from TXN-2024-000001
            var numberPart = latestPayment.TransactionId.Split('-').Last();
            if (int.TryParse(numberPart, out int currentNumber))
            {
                nextNumber = currentNumber + 1;
            }
        }

        return $"{prefix}{nextNumber:D6}"; // D6 = 6 digits with leading zeros
    }

    /// <summary>
    /// Maps Payment entity to PaymentDto
    /// </summary>
    private PaymentDto MapToPaymentDto(Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            TransactionId = payment.TransactionId,
            PolicyId = payment.PolicyId,
            PolicyNumber = payment.Policy?.PolicyNumber ?? "Unknown",
            Amount = payment.Amount,
            Method = payment.Method.ToString(),
            Status = payment.Status.ToString(),
            PaymentDate = payment.PaymentDate,
            ProcessedDate = payment.ProcessedDate,
            Reference = payment.Reference,
            CreatedAt = payment.CreatedAt
        };
    }
}