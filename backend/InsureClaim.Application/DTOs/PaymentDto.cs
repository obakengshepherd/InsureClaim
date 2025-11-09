namespace InsureClaim.Application.DTOs;

/// <summary>
/// DTO for payment response
/// Why: Returns payment information to the client
/// </summary>
public class PaymentDto
{
    public int Id { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public int PolicyId { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public string? Reference { get; set; }
    public DateTime CreatedAt { get; set; }
}