using InsureClaim.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsureClaim.Infrastructure.Data;

/// <summary>
/// Main database context for InsureClaim application
/// Why: Centralizes all database operations and entity configurations
/// Business Impact: Ensures data integrity and supports complex queries for reporting
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }
    
    // DbSets - represent tables in the database
    public DbSet<User> Users { get; set; }
    public DbSet<Policy> Policies { get; set; }
    public DbSet<InsuranceClaim> Claims { get; set; }  // CHANGED THIS LINE
    public DbSet<Payment> Payments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique(); // Business rule: unique emails
            entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.PhoneNumber).HasMaxLength(20);
        });
        
        // Policy configuration
        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasIndex(p => p.PolicyNumber).IsUnique();
            entity.Property(p => p.PolicyNumber).IsRequired().HasMaxLength(50);
            entity.Property(p => p.CoverageAmount).HasPrecision(18, 2);
            entity.Property(p => p.PremiumAmount).HasPrecision(18, 2);
            
            // Relationship: User has many Policies
            entity.HasOne(p => p.User)
                  .WithMany(u => u.Policies)
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Claim configuration - CHANGED 'Claim' to 'InsuranceClaim'
        modelBuilder.Entity<InsuranceClaim>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => c.ClaimNumber).IsUnique();
            entity.Property(c => c.ClaimNumber).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Description).IsRequired().HasMaxLength(1000);
            entity.Property(c => c.ClaimAmount).HasPrecision(18, 2);
            entity.Property(c => c.ApprovedAmount).HasPrecision(18, 2);
            entity.Property(c => c.DocumentPath).HasMaxLength(500);
            entity.Property(c => c.ReviewNotes).HasMaxLength(1000);
            
            // Relationships
            entity.HasOne(c => c.Policy)
                  .WithMany(p => p.Claims)
                  .HasForeignKey(c => c.PolicyId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(c => c.User)
                  .WithMany(u => u.Claims)
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Payment configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasIndex(p => p.TransactionId).IsUnique();
            entity.Property(p => p.TransactionId).IsRequired().HasMaxLength(50);
            entity.Property(p => p.Amount).HasPrecision(18, 2);
            entity.Property(p => p.Reference).HasMaxLength(200);
            
            // Relationship: Policy has many Payments
            entity.HasOne(p => p.Policy)
                  .WithMany(pol => pol.Payments)
                  .HasForeignKey(p => p.PolicyId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Seed initial admin user (for development)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FullName = "System Admin",
                Email = "admin@insureclaim.com",
                // Password: Admin@123 (hashed with BCrypt)
                PasswordHash = "$2a$11$xQPvZ7wXxmPPfWlG8EwuUeFxkCfYMKWQQQvZ7wXxmPPfWlG8EwuUe",
                PhoneNumber = "+27123456789",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}