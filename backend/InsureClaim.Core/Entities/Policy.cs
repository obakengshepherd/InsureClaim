namespace InsureClaim.Core.Entities;

/// <summary>
/// Represents an insurance policy purchased by a customer
/// Business Rule: Premium is calculated based on coverage and risk factors
/// </summary>
public class Policy
{
    public int Id { get; set; }
    public string PolicyNumber { get; set; } = string.Empty; // e.g., POL-2024-001234
    
    // Foreign key
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public PolicyType Type { get; set; }
    public decimal CoverageAmount { get; set; } // The insured value
    public decimal PremiumAmount { get; set; } // Monthly/annual payment
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public PolicyStatus Status { get; set; } = PolicyStatus.Active;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public ICollection<InsuranceClaim> Claims { get; set; } = new List<InsuranceClaim>();  // CHANGED THIS LINE
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public enum PolicyType
{
    Life = 1,
    Auto = 2,
    Health = 3,
    Property = 4
}

public enum PolicyStatus
{
    Active = 1,
    Expired = 2,
    Cancelled = 3,
    Suspended = 4
}