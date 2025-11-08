using System.IdentityModel.Tokens.Jwt;
using System.Text;
using InsureClaim.Application.Interfaces;
using InsureClaim.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SysClaim = System.Security.Claims.Claim;  // ALIAS TO AVOID CONFLICT
using SysClaimTypes = System.Security.Claims.ClaimTypes;  // ALIAS

namespace InsureClaim.Application.Services;

/// <summary>
/// Service for generating and managing JWT tokens
/// Why: Centralizes all token logic for consistency
/// Business Impact: Enables secure stateless authentication across distributed systems
/// </summary>
public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
        _secretKey = _configuration["JwtSettings:SecretKey"]!;
        _issuer = _configuration["JwtSettings:Issuer"]!;
        _audience = _configuration["JwtSettings:Audience"]!;
        _expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"]!);
    }

    public string GenerateToken(User user)
    {
        // Create claims - information embedded in the token
        // Using SysClaim alias to avoid conflict with InsuranceClaim entity
        var claims = new[]
        {
            new SysClaim(SysClaimTypes.NameIdentifier, user.Id.ToString()),
            new SysClaim(SysClaimTypes.Email, user.Email),
            new SysClaim(SysClaimTypes.Name, user.FullName),
            new SysClaim(SysClaimTypes.Role, user.Role.ToString()),
            new SysClaim("UserId", user.Id.ToString()),
            new SysClaim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Create signing credentials
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Create token
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials
        );

        // Return serialized token
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTime GetTokenExpiration()
    {
        return DateTime.UtcNow.AddMinutes(_expirationMinutes);
    }
}