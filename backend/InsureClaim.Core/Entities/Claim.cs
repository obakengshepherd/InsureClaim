namespace InsureClaim.Core.Entities;

/// <summary>
/// Represents an insurance claim filed by a customer
/// Business Rule: Claims must be linked to active policies
/// Workflow: Submitted → Under Review → Approved/Denied
/// </summary>
public class InsuranceClaim
{
    public int Id { get; set; }
    public string ClaimNumber { get; set; } = string.Empty; // e.g., CLM-2024-001234
    
    // Foreign keys
    public int PolicyId { get; set; }
    public Policy Policy { get; set; } = null!;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string Description { get; set; } = string.Empty;
    public decimal ClaimAmount { get; set; } // Amount being claimed
    public decimal? ApprovedAmount { get; set; } // Actual payout if approved
    
    public ClaimStatus Status { get; set; } = ClaimStatus.Submitted;
    
    public DateTime IncidentDate { get; set; }
    public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedDate { get; set; }
    
    public string? DocumentPath { get; set; } // Path to uploaded proof documents
    public string? ReviewNotes { get; set; } // Admin notes
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public enum ClaimStatus
{
    Submitted = 1,
    UnderReview = 2,
    Approved = 3,
    Denied = 4,
    Paid = 5
}