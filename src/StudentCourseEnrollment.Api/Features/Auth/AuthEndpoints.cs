using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using StudentCourseEnrollment.Api.Data;
using StudentCourseEnrollment.Api.Models;
using StudentCourseEnrollment.Shared.DTOs.Auth;

namespace StudentCourseEnrollment.Api.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuth(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth");

        group.MapPost("/register", RegisterAsync);
        group.MapPost("/login", LoginAsync);
        ProvisionUserEndpoint.MapProvisionUser(app);
    }

    public static async Task<IResult> RegisterAsync(
        RegisterRequest request,
        EnrollmentContext db)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Results.BadRequest("Email is required");
            }

            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(request.Email))
            {
                return Results.BadRequest("Invalid email format");
            }

            if (string.IsNullOrWhiteSpace(request.FirstName))
            {
                return Results.BadRequest("FirstName is required");
            }

            if (request.FirstName.Length < 2)
            {
                return Results.BadRequest("FirstName must be at least 2 characters");
            }

            if (string.IsNullOrWhiteSpace(request.LastName))
            {
                return Results.BadRequest("LastName is required");
            }

            if (request.LastName.Length < 2)
            {
                return Results.BadRequest("LastName must be at least 2 characters");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return Results.BadRequest("Password is required");
            }

            if (request.Password.Length < 6)
            {
                return Results.BadRequest("Password must be at least 6 characters");
            }

            if (await db.Students.AnyAsync(s => s.Email == request.Email))
            {
                return Results.Problem(
                    title: "Email already registered",
                    statusCode: 400
                );
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var student = new Student
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = passwordHash
            };

            db.Students.Add(student);
            await db.SaveChangesAsync();

            var response = new RegisterResponse(
                Id: student.Id,
                Email: student.Email,
                FirstName: student.FirstName,
                LastName: student.LastName
            );

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "An error occurred during registration",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }

    public static async Task<IResult> LoginAsync(
        LoginRequest request,
        EnrollmentContext db,
        JwtTokenService tokenService)
    {
        try
        {
            var student = await db.Students
                .FirstOrDefaultAsync(s => s.Email == request.Email);

            if (student == null)
            {
                return Results.Problem(
                    title: "Invalid credentials",
                    detail: "You do not have a valid account. Please register for a new account.",
                    statusCode: 401
                );
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, student.PasswordHash))
            {
                return Results.Problem(
                    title: "Invalid credentials",
                    detail: "Incorrect password. Please try again.",
                    statusCode: 401
                );
            }

            var roles = new[] { student.Role };
            var token = tokenService.GenerateToken(student.Id, student.Email, student.FirstName, student.LastName, roles);

            var response = new LoginResponse(
                Token: token,
                Id: student.Id,
                Email: student.Email,
                FirstName: student.FirstName,
                LastName: student.LastName
            );

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "An error occurred during login",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }
}
