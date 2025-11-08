using InsureClaim.Application.DTOs;
using InsureClaim.Application.Interfaces;
using InsureClaim.Core.Entities;
using InsureClaim.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InsureClaim.Application.Services;

/// <summary>
/// Service handling insurance policy management
/// Why: Encapsulates policy business logic and premium calculations
/// Business Impact: Automates policy creation and reduces manual errors
/// </summary>
public class PolicyService : IPolicyService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PolicyService> _logger;

    public PolicyService(ApplicationDbContext context, ILogger<PolicyService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PolicyDto> CreatePolicyAsync(CreatePolicyDto createPolicyDto)
    {
        try
        {
            _logger.LogInformation("Creating policy for user {UserId}", createPolicyDto.UserId);

            // Verify user exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == createPolicyDto.UserId);
            if (!userExists)
            {
                throw new InvalidOperationException($"User with ID {createPolicyDto.UserId} not found");
            }

            // Generate unique policy number
            var policyNumber = await GeneratePolicyNumberAsync();

            // Calculate end date
            var endDate = createPolicyDto.StartDate.AddMonths(createPolicyDto.DurationMonths);

            // Calculate premium based on coverage and type
            var premiumAmount = CalculatePremium(
                createPolicyDto.CoverageAmount,
                (PolicyType)createPolicyDto.Type,
                createPolicyDto.DurationMonths
            );

            // Create policy entity
            var policy = new Policy
            {
                PolicyNumber = policyNumber,
                UserId = createPolicyDto.UserId,
                Type = (PolicyType)createPolicyDto.Type,
                CoverageAmount = createPolicyDto.CoverageAmount,
                PremiumAmount = premiumAmount,
                StartDate = createPolicyDto.StartDate,
                EndDate = endDate,
                Status = PolicyStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            _context.Policies.Add(policy);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Policy created successfully: {PolicyNumber} for user {UserId}",
                policyNumber,
                createPolicyDto.UserId
            );

            // Return DTO
            return await GetPolicyDtoAsync(policy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating policy for user {UserId}", createPolicyDto.UserId);
            throw;
        }
    }

    public async Task<List<PolicyDto>> GetAllPoliciesAsync(int? userId = null, string? userRole = null)
    {
        try
        {
            var query = _context.Policies
                .Include(p => p.User)
                .AsQueryable();

            // If user is Customer or Agent, filter by their ID
            if (userId.HasValue && userRole != "Admin")
            {
                query = query.Where(p => p.UserId == userId.Value);
            }

            var policies = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return policies.Select(p => MapToPolicyDto(p)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving policies");
            throw;
        }
    }

    public async Task<PolicyDto?> GetPolicyByIdAsync(int policyId)
    {
        try
        {
            var policy = await _context.Policies
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == policyId);

            if (policy == null)
            {
                _logger.LogWarning("Policy not found with ID: {PolicyId}", policyId);
                return null;
            }

            return MapToPolicyDto(policy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving policy {PolicyId}", policyId);
            throw;
        }
    }

    public async Task<PolicyDto?> UpdatePolicyAsync(int policyId, UpdatePolicyDto updatePolicyDto)
    {
        try
        {
            var policy = await _context.Policies.FindAsync(policyId);

            if (policy == null)
            {
                _logger.LogWarning("Policy not found with ID: {PolicyId}", policyId);
                return null;
            }

            // Update only provided fields
            if (updatePolicyDto.CoverageAmount.HasValue)
            {
                policy.CoverageAmount = updatePolicyDto.CoverageAmount.Value;
                // Recalculate premium if coverage changes
                var durationMonths = (int)(policy.EndDate - policy.StartDate).TotalDays / 30;
                policy.PremiumAmount = CalculatePremium(
                    policy.CoverageAmount,
                    policy.Type,
                    durationMonths
                );
            }

            if (updatePolicyDto.Status.HasValue)
            {
                policy.Status = (PolicyStatus)updatePolicyDto.Status.Value;
            }

            if (updatePolicyDto.EndDate.HasValue)
            {
                policy.EndDate = updatePolicyDto.EndDate.Value;
            }

            policy.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Policy updated: {PolicyNumber}", policy.PolicyNumber);

            return await GetPolicyDtoAsync(policy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating policy {PolicyId}", policyId);
            throw;
        }
    }

    public async Task<bool> DeletePolicyAsync(int policyId)
    {
        try
        {
            var policy = await _context.Policies.FindAsync(policyId);

            if (policy == null)
            {
                _logger.LogWarning("Policy not found with ID: {PolicyId}", policyId);
                return false;
            }

            // Soft delete - change status to Cancelled
            policy.Status = PolicyStatus.Cancelled;
            policy.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Policy cancelled: {PolicyNumber}", policy.PolicyNumber);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting policy {PolicyId}", policyId);
            throw;
        }
    }

    public async Task<List<PolicyDto>> GetUserPoliciesAsync(int userId)
    {
        try
        {
            var policies = await _context.Policies
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return policies.Select(p => MapToPolicyDto(p)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving policies for user {UserId}", userId);
            throw;
        }
    }

    // ============= HELPER METHODS =============

    /// <summary>
    /// Generates unique policy number in format: POL-YYYY-NNNNNN
    /// Business Rule: Policy numbers must be sequential and unique
    /// </summary>
    private async Task<string> GeneratePolicyNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"POL-{year}-";

        // Get the latest policy number for this year
        var latestPolicy = await _context.Policies
            .Where(p => p.PolicyNumber.StartsWith(prefix))
            .OrderByDescending(p => p.PolicyNumber)
            .FirstOrDefaultAsync();

        int nextNumber = 1;

        if (latestPolicy != null)
        {
            // Extract number from POL-2024-000001
            var numberPart = latestPolicy.PolicyNumber.Split('-').Last();
            if (int.TryParse(numberPart, out int currentNumber))
            {
                nextNumber = currentNumber + 1;
            }
        }

        return $"{prefix}{nextNumber:D6}"; // D6 = 6 digits with leading zeros
    }

    /// <summary>
    /// Calculates monthly premium based on coverage, type, and duration
    /// Business Logic: Different policy types have different risk factors
    /// </summary>
    private decimal CalculatePremium(decimal coverageAmount, PolicyType policyType, int durationMonths)
    {
        // Base premium rates (percentage of coverage per month)
        decimal ratePerMonth = policyType switch
        {
            PolicyType.Life => 0.005m,      // 0.5% per month
            PolicyType.Auto => 0.008m,      // 0.8% per month
            PolicyType.Health => 0.006m,    // 0.6% per month
            PolicyType.Property => 0.004m,  // 0.4% per month
            _ => 0.005m
        };

        // Calculate monthly premium
        var monthlyPremium = coverageAmount * ratePerMonth;

        // Apply duration discount (longer duration = slight discount)
        if (durationMonths >= 24)
        {
            monthlyPremium *= 0.90m; // 10% discount for 2+ years
        }
        else if (durationMonths >= 12)
        {
            monthlyPremium *= 0.95m; // 5% discount for 1+ year
        }

        return Math.Round(monthlyPremium, 2);
    }

    /// <summary>
    /// Maps Policy entity to PolicyDto
    /// </summary>
    private PolicyDto MapToPolicyDto(Policy policy)
    {
        return new PolicyDto
        {
            Id = policy.Id,
            PolicyNumber = policy.PolicyNumber,
            UserId = policy.UserId,
            UserName = policy.User?.FullName ?? "Unknown",
            Type = policy.Type.ToString(),
            CoverageAmount = policy.CoverageAmount,
            PremiumAmount = policy.PremiumAmount,
            StartDate = policy.StartDate,
            EndDate = policy.EndDate,
            Status = policy.Status.ToString(),
            CreatedAt = policy.CreatedAt,
            UpdatedAt = policy.UpdatedAt
        };
    }

    /// <summary>
    /// Gets PolicyDto with user information loaded
    /// </summary>
    private async Task<PolicyDto> GetPolicyDtoAsync(Policy policy)
    {
        var user = await _context.Users.FindAsync(policy.UserId);
        policy.User = user!;
        return MapToPolicyDto(policy);
    }
}