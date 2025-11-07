namespace InsureClaim.Core.Entities;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Customer;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    public ICollection<Policy> Policies { get; set; } = new List<Policy>();
    public ICollection<Claim> Claims { get; set; } = new List<Claim>();
}

public enum UserRole
{
    Customer = 1,
    Agent = 2,
    Admin = 3
}