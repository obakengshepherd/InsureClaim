using System.ComponentModel.DataAnnotations;

namespace InsureClaim.Application.DTOs;

/// <summary>
/// DTO for recording a new payment
/// Why: Validates payment data before processing
/// Business Rule: Payments must be linked to active policies
/// </summary>
public class RecordPaymentDto
{
    [Required(ErrorMessage = "Policy ID is required")]
    public int PolicyId { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(1, 10000000, ErrorMessage = "Amount must be between R1 and R10,000,000")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Payment method is required")]
    [Range(1, 5, ErrorMessage = "Payment method must be CreditCard (1), DebitCard (2), BankTransfer (3), Cash (4), or MobilePayment (5)")]
    public int Method { get; set; }

    [StringLength(200, ErrorMessage = "Reference cannot exceed 200 characters")]
    public string? Reference { get; set; } // External payment gateway reference
}