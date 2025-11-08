namespace InsureClaim.Core.Entities;

/// <summary>
/// Represents a user in the system (Customer, Agent, or Admin)
/// Business Rule: Email must be unique, all users require authentication
/// </summary>
public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    
    // Role-based access: Customer, Agent, Admin
    public UserRole Role { get; set; } = UserRole.Customer;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<Policy> Policies { get; set; } = new List<Policy>();
    public ICollection<InsuranceClaim> Claims { get; set; } = new List<InsuranceClaim>();  // CHANGED THIS LINE
}

public enum UserRole
{
    Customer = 1,
    Agent = 2,
    Admin = 3
}