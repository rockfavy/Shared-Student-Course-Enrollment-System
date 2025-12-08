using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using StudentCourseEnrollment.Api.Data;
using StudentCourseEnrollment.Shared.DTOs.Auth;

namespace StudentCourseEnrollment.Api.Features.Auth;

public static class ProvisionUserEndpoint
{
    public static void MapProvisionUser(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth");
        group.MapPost("/provision", ProvisionUserAsync).RequireAuthorization();
    }

    public static async Task<IResult> ProvisionUserAsync(
        HttpContext httpContext,
        UserProvisioningService provisioningService,
        EnrollmentContext db)
    {
        try
        {
            var user = httpContext.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return Results.Unauthorized();
            }

            var student = await provisioningService.ProvisionUserFromClaimsAsync(user);
            if (student == null)
            {
                return Results.Problem(
                    title: "User provisioning failed",
                    detail: "Could not provision user from authentication claims.",
                    statusCode: 400
                );
            }

            var response = new ProvisionUserResponse(
                Id: student.Id,
                Email: student.Email,
                FirstName: student.FirstName,
                LastName: student.LastName,
                Role: student.Role
            );

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "An error occurred during user provisioning",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }
}

