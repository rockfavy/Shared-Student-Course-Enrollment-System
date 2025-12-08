using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudentCourseEnrollment.Api.Data;
using StudentCourseEnrollment.Api.Features.Auth;
using StudentCourseEnrollment.Api.Models;
using StudentCourseEnrollment.Shared.DTOs.Auth;
using StudentCourseEnrollment.Tests.Unit.TestHelpers;
using Xunit;

namespace StudentCourseEnrollment.Tests.Unit.Features.Auth;

public class AuthEndpointsTests : IDisposable
{
    private readonly EnrollmentContext _dbContext;
    private readonly JwtTokenService _tokenService;

    public AuthEndpointsTests()
    {
        _dbContext = TestDbContextFactory.CreateInMemoryContext();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:SecretKey", "TestSecretKeyForUnitTests-Minimum32Characters" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" }
            })
            .Build();
        _tokenService = new JwtTokenService(configuration);
    }

    [Fact]
    public async Task RegisterAsync_With_Valid_Request_Should_Return_Success()
    {
        var request = new RegisterRequest(
            Email: $"test{Guid.NewGuid()}@example.com",
            FirstName: "Test",
            LastName: "User",
            Password: "Password123!"
        );

        var result = await AuthEndpoints.RegisterAsync(request, _dbContext);

        Assert.NotNull(result);
        var typedResult = Assert.IsType<Ok<RegisterResponse>>(result);
        Assert.NotNull(typedResult.Value);
        Assert.NotEqual(Guid.Empty, typedResult.Value.Id);
        Assert.Equal(request.Email, typedResult.Value.Email);
        Assert.Equal(request.FirstName, typedResult.Value.FirstName);
        Assert.Equal(request.LastName, typedResult.Value.LastName);
    }

    [Fact]
    public async Task RegisterAsync_With_Empty_Email_Should_Return_BadRequest()
    {
        var request = new RegisterRequest(
            Email: string.Empty,
            FirstName: "Test",
            LastName: "User",
            Password: "Password123!"
        );

        var result = await AuthEndpoints.RegisterAsync(request, _dbContext);

        var typedResult = Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Email is required", typedResult.Value);
    }

    [Fact]
    public async Task RegisterAsync_With_Invalid_Email_Format_Should_Return_BadRequest()
    {
        var request = new RegisterRequest(
            Email: "invalid-email",
            FirstName: "Test",
            LastName: "User",
            Password: "Password123!"
        );

        var result = await AuthEndpoints.RegisterAsync(request, _dbContext);

        var typedResult = Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Invalid email format", typedResult.Value);
    }

    [Fact]
    public async Task RegisterAsync_With_Empty_FirstName_Should_Return_BadRequest()
    {
        var request = new RegisterRequest(
            Email: $"test{Guid.NewGuid()}@example.com",
            FirstName: string.Empty,
            LastName: "User",
            Password: "Password123!"
        );

        var result = await AuthEndpoints.RegisterAsync(request, _dbContext);

        var typedResult = Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("FirstName is required", typedResult.Value);
    }

    [Fact]
    public async Task RegisterAsync_With_Short_FirstName_Should_Return_BadRequest()
    {
        var request = new RegisterRequest(
            Email: $"test{Guid.NewGuid()}@example.com",
            FirstName: "A",
            LastName: "User",
            Password: "Password123!"
        );

        var result = await AuthEndpoints.RegisterAsync(request, _dbContext);

        var typedResult = Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("FirstName must be at least 2 characters", typedResult.Value);
    }

    [Fact]
    public async Task RegisterAsync_With_Empty_Password_Should_Return_BadRequest()
    {
        var request = new RegisterRequest(
            Email: $"test{Guid.NewGuid()}@example.com",
            FirstName: "Test",
            LastName: "User",
            Password: string.Empty
        );

        var result = await AuthEndpoints.RegisterAsync(request, _dbContext);

        var typedResult = Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Password is required", typedResult.Value);
    }

    [Fact]
    public async Task RegisterAsync_With_Short_Password_Should_Return_BadRequest()
    {
        var request = new RegisterRequest(
            Email: $"test{Guid.NewGuid()}@example.com",
            FirstName: "Test",
            LastName: "User",
            Password: "12345"
        );

        var result = await AuthEndpoints.RegisterAsync(request, _dbContext);

        var typedResult = Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Password must be at least 6 characters", typedResult.Value);
    }

    [Fact]
    public async Task RegisterAsync_With_Duplicate_Email_Should_Return_Problem()
    {
        var email = $"duplicate{Guid.NewGuid()}@example.com";
        var existingStudent = new Student
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = "Existing",
            LastName = "User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
        };
        _dbContext.Students.Add(existingStudent);
        await _dbContext.SaveChangesAsync();

        var request = new RegisterRequest(
            Email: email,
            FirstName: "New",
            LastName: "User",
            Password: "Password123!"
        );

        var result = await AuthEndpoints.RegisterAsync(request, _dbContext);

        var typedResult = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(400, typedResult.StatusCode);
    }

    [Fact]
    public async Task RegisterAsync_Should_Hash_Password()
    {
        var request = new RegisterRequest(
            Email: $"test{Guid.NewGuid()}@example.com",
            FirstName: "Test",
            LastName: "User",
            Password: "Password123!"
        );

        await AuthEndpoints.RegisterAsync(request, _dbContext);

        var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Email == request.Email);
        Assert.NotNull(student);
        Assert.NotEqual(request.Password, student.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify(request.Password, student.PasswordHash));
    }

    [Fact]
    public async Task LoginAsync_With_Valid_Credentials_Should_Return_Token()
    {
        var email = $"login{Guid.NewGuid()}@example.com";
        var password = "Password123!";
        var student = new Student
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = "Login",
            LastName = "User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };
        _dbContext.Students.Add(student);
        await _dbContext.SaveChangesAsync();

        var request = new LoginRequest(
            Email: email,
            Password: password
        );

        var result = await AuthEndpoints.LoginAsync(request, _dbContext, _tokenService);

        var typedResult = Assert.IsType<Ok<LoginResponse>>(result);
        Assert.NotNull(typedResult.Value);
        Assert.NotEmpty(typedResult.Value.Token);
        Assert.Equal(student.Id, typedResult.Value.Id);
        Assert.Equal(email, typedResult.Value.Email);
    }

    [Fact]
    public async Task LoginAsync_With_NonExistent_Email_Should_Return_Unauthorized()
    {
        var request = new LoginRequest(
            Email: "nonexistent@example.com",
            Password: "Password123!"
        );

        var result = await AuthEndpoints.LoginAsync(request, _dbContext, _tokenService);

        var typedResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult>(result);
        Assert.Equal(401, typedResult.StatusCode);
    }

    [Fact]
    public async Task LoginAsync_With_Wrong_Password_Should_Return_Unauthorized()
    {
        var email = $"wrongpass{Guid.NewGuid()}@example.com";
        var student = new Student
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = "Test",
            LastName = "User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!")
        };
        _dbContext.Students.Add(student);
        await _dbContext.SaveChangesAsync();

        var request = new LoginRequest(
            Email: email,
            Password: "WrongPassword123!"
        );

        var result = await AuthEndpoints.LoginAsync(request, _dbContext, _tokenService);

        var typedResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult>(result);
        Assert.Equal(401, typedResult.StatusCode);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
