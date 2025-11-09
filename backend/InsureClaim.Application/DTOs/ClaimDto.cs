namespace InsureClaim.Application.DTOs;

/// <summary>
/// DTO for claim response
/// Why: Returns claim information to the client
/// </summary>
public class ClaimDto
{
    public int Id { get; set; }
    public string ClaimNumber { get; set; } = string.Empty;
    
    // Policy information
    public int PolicyId { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    
    // User information
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    
    // Claim details
    public string Description { get; set; } = string.Empty;
    public decimal ClaimAmount { get; set; }
    public decimal? ApprovedAmount { get; set; }
    
    // Status and workflow
    public string Status { get; set; } = string.Empty; // Submitted, UnderReview, Approved, Denied, Paid
    
    // Dates
    public DateTime IncidentDate { get; set; }
    public DateTime SubmittedDate { get; set; }
    public DateTime? ReviewedDate { get; set; }
    
    // Additional info
    public string? DocumentPath { get; set; }
    public string? ReviewNotes { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}