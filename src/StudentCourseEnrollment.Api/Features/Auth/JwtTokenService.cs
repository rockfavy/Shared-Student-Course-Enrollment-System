using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace StudentCourseEnrollment.Api.Features.Auth;

public class JwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(Guid userId, string email, string firstName, string lastName, string[] roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSecretKey()));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var fullName = $"{firstName} {lastName}".Trim();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, fullName),
            new Claim("FirstName", firstName),
            new Claim("LastName", lastName)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: GetIssuer(),
            audience: GetAudience(),
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GetSecretKey()
    {
        return _configuration["Jwt:SecretKey"] 
            ?? "DevelopmentSecretKey-ChangeInProduction-Minimum32Characters";
    }

    private string GetIssuer()
    {
        return _configuration["Jwt:Issuer"] ?? "StudentCourseEnrollment";
    }

    private string GetAudience()
    {
        return _configuration["Jwt:Audience"] ?? "StudentCourseEnrollment";
    }
}
