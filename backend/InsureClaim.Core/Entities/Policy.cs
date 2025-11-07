namespace InsureClaim.Core.Entities;

public class Policy
{
    public int Id { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public PolicyType Type { get; set; }
    public decimal CoverageAmount { get; set; }
    public decimal PremiumAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public PolicyStatus Status { get; set; } = PolicyStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public ICollection<Claim> Claims { get; set; } = new List<Claim>();
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