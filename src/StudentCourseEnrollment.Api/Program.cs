using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StudentCourseEnrollment.Api.Data;
using StudentCourseEnrollment.Api.Features.Auth;
using StudentCourseEnrollment.Api.Features.Courses;
using StudentCourseEnrollment.Api.Features.Enrollments;
using StudentCourseEnrollment.Api.Shared.ErrorHandling;
using StudentCourseEnrollment.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "https://localhost:5001",
                "http://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        void ConfigureSymmetricKeyValidation()
        {
            var secretKey = builder.Configuration["Jwt:SecretKey"] 
                ?? "DevelopmentSecretKey-ChangeInProduction-Minimum32Characters";
            var issuer = builder.Configuration["Jwt:Issuer"] ?? "StudentCourseEnrollment";
            var audience = builder.Configuration["Jwt:Audience"] ?? "StudentCourseEnrollment";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role
            };
        }

        void ConfigureExternalAuthority(string authority, bool requireHttpsMetadata)
        {
            var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "StudentCourseEnrollment";

            options.Authority = authority;
            options.RequireHttpsMetadata = requireHttpsMetadata;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = authority,
                ValidAudience = jwtAudience,
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role
            };
        }

        if (builder.Environment.IsDevelopment())
        {
            ConfigureSymmetricKeyValidation();
        }
        else
        {
            var entraIdInstance = builder.Configuration["Authentication:Schemes:EntraId:Instance"];
            var entraIdTenantId = builder.Configuration["Authentication:Schemes:EntraId:TenantId"];

            if (!string.IsNullOrEmpty(entraIdInstance) && !string.IsNullOrEmpty(entraIdTenantId))
            {
                var entraIdAuthority = $"{entraIdInstance.TrimEnd('/')}/{entraIdTenantId}/v2.0";
                ConfigureExternalAuthority(entraIdAuthority, true);
            }
            else
            {
                ConfigureSymmetricKeyValidation();
            }
        }
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanEnroll", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole(Role.Student.ToString());
    });

    options.AddPolicy("CanManageCourses", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole(Role.Admin.ToString());
    });
});

builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<UserProvisioningService>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<EnrollmentService>();

builder.Services.AddEnrollmentContext(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuth();
app.MapCourses();
app.MapEnrollments();

app.MapGet("/", () =>
{
    if (app.Environment.IsDevelopment())
    {
        return Results.Redirect("/swagger");
    }
    return Results.Ok(new { message = "Student Course Enrollment API", version = "1.0" });
});

await app.InitializeDbAsync();

app.Run();

public partial class Program { }
