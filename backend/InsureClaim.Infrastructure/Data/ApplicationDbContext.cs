using InsureClaim.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsureClaim.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Policy> Policies { get; set; }
    public DbSet<Claim> Claims { get; set; }
    public DbSet<Payment> Payments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.PhoneNumber).HasMaxLength(20);
        });
        
        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasIndex(p => p.PolicyNumber).IsUnique();
            entity.Property(p => p.PolicyNumber).IsRequired().HasMaxLength(50);
            entity.Property(p => p.CoverageAmount).HasPrecision(18, 2);
            entity.Property(p => p.PremiumAmount).HasPrecision(18, 2);
            
            entity.HasOne(p => p.User)
                  .WithMany(u => u.Policies)
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<Claim>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => c.ClaimNumber).IsUnique();
            entity.Property(c => c.ClaimNumber).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Description).IsRequired().HasMaxLength(1000);
            entity.Property(c => c.ClaimAmount).HasPrecision(18, 2);
            entity.Property(c => c.ApprovedAmount).HasPrecision(18, 2);
            
            entity.HasOne(c => c.Policy)
                  .WithMany(p => p.Claims)
                  .HasForeignKey(c => c.PolicyId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(c => c.User)
                  .WithMany(u => u.Claims)
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasIndex(p => p.TransactionId).IsUnique();
            entity.Property(p => p.TransactionId).IsRequired().HasMaxLength(50);
            entity.Property(p => p.Amount).HasPrecision(18, 2);
            
            entity.HasOne(p => p.Policy)
                  .WithMany(pol => pol.Payments)
                  .HasForeignKey(p => p.PolicyId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}