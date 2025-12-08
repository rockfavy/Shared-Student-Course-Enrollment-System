using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using StudentCourseEnrollment.Frontend.Components.Pages;
using StudentCourseEnrollment.Frontend.Services;
using StudentCourseEnrollment.Shared;
using System.Security.Claims;
using Xunit;

namespace StudentCourseEnrollment.Tests.Unit.Components.Pages;

public class HomePageTests : TestContext
{
    private CustomAuthenticationStateProvider CreateAuthStateProvider(string? token = null)
    {
        var mockJSRuntime = new Mock<IJSRuntime>(MockBehavior.Loose);
        
        var tokenToReturn = token;
        mockJSRuntime.Setup(x => x.InvokeAsync<string?>(
            It.Is<string>(s => s == "localStorage.getItem"), 
            It.Is<object[]>(args => args != null && args.Length > 0 && args[0].ToString() == "authToken")))
            .ReturnsAsync(tokenToReturn);
        
        mockJSRuntime.Setup(x => x.InvokeAsync<string?>(It.IsAny<string>(), It.IsAny<object[]>()))
            .ReturnsAsync((string method, object[] args) => 
            {
                if (method == "localStorage.getItem" && args != null && args.Length > 0 && args[0]?.ToString() == "authToken")
                {
                    return tokenToReturn;
                }
                return null;
            });
        mockJSRuntime.Setup(x => x.InvokeAsync<IJSVoidResult?>(It.IsAny<string>(), It.IsAny<object[]>()))
            .ReturnsAsync((IJSVoidResult?)null);
        return new CustomAuthenticationStateProvider(mockJSRuntime.Object);
    }

    private TestAuthorizationContext RegisterServices(CustomAuthenticationStateProvider authStateProvider, bool syncAuthState = false)
    {
        Services.AddSingleton<AuthenticationStateProvider>(authStateProvider);
        Services.AddSingleton(authStateProvider);
        var authContext = this.AddTestAuthorization();
        
        // Sync test authorization with CustomAuthenticationStateProvider if requested
        if (syncAuthState)
        {
            var authState = authStateProvider.GetAuthenticationStateAsync().Result;
            if (authState.User.Identity?.IsAuthenticated == true)
            {
                authContext.SetAuthorized(authState.User.Identity.Name ?? "TestUser");
                var roleClaim = authState.User.FindFirst(ClaimTypes.Role);
                if (roleClaim != null)
                {
                    authContext.SetRoles(roleClaim.Value);
                }
            }
            else
            {
                authContext.SetNotAuthorized();
            }
        }
        
        return authContext;
    }

    [Fact]
    public void HomePage_Renders_Page_Title()
    {
        var authStateProvider = CreateAuthStateProvider();
        var authContext = RegisterServices(authStateProvider);

        var cut = RenderComponent<Home>();

        Assert.Contains("Student Course Enrollment System", cut.Markup);
    }

    [Fact]
    public void HomePage_Renders_Available_Courses_Card()
    {
        var authStateProvider = CreateAuthStateProvider();
        var authContext = RegisterServices(authStateProvider);

        var cut = RenderComponent<Home>();

        Assert.Contains("Available Courses", cut.Markup);
        Assert.Contains("View Courses", cut.Markup);
        Assert.Contains("/courses", cut.Markup);
    }

    [Fact]
    public void HomePage_Renders_Get_Started_Card_For_Unauthenticated_Users()
    {
        var authStateProvider = CreateAuthStateProvider();
        var authContext = RegisterServices(authStateProvider);
        authContext.SetNotAuthorized();

        var cut = RenderComponent<Home>();

        Assert.Contains("Get Started", cut.Markup);
        Assert.Contains("Login", cut.Markup);
        Assert.Contains("Register", cut.Markup);
        Assert.Contains("/login", cut.Markup);
        Assert.Contains("/register", cut.Markup);
    }

    [Fact]
    public async Task HomePage_Renders_My_Enrollments_Card_For_Authenticated_Students()
    {
        var token = "test-token";
        var authStateProvider = CreateAuthStateProvider(token);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, "Test Student"),
            new Claim(ClaimTypes.Email, "student@test.com"),
            new Claim(ClaimTypes.Role, Role.Student.ToString())
        };

        var identity = new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
        var user = new ClaimsPrincipal(identity);
        
        await authStateProvider.SetAuthenticationStateAsync(user, token);
        
        var authState = await authStateProvider.GetAuthenticationStateAsync();
        Assert.True(authState.User.Identity?.IsAuthenticated == true);
        
        var roleClaim = authState.User.FindFirst(ClaimTypes.Role);
        Assert.NotNull(roleClaim);
        Assert.Equal(Role.Student.ToString(), roleClaim.Value);
        
        var authContext = RegisterServices(authStateProvider, syncAuthState: true);

        var cut = RenderComponent<CascadingAuthenticationState>(childContent => childContent
            .AddChildContent<Home>());

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("My Enrollments", cut.Markup);
            Assert.Contains("View Enrollments", cut.Markup);
            Assert.Contains("/enrollments", cut.Markup);
        }, timeout: TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void HomePage_Does_Not_Render_My_Enrollments_Card_For_Unauthenticated_Users()
    {
        var authStateProvider = CreateAuthStateProvider();
        var authContext = RegisterServices(authStateProvider);
        authContext.SetNotAuthorized();

        var cut = RenderComponent<Home>();

        Assert.DoesNotContain("My Enrollments", cut.Markup);
    }

    [Fact]
    public async Task HomePage_Does_Not_Render_Get_Started_Card_For_Authenticated_Users()
    {
        var token = "test-token";
        var authStateProvider = CreateAuthStateProvider(token);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, "Test Student"),
            new Claim(ClaimTypes.Email, "student@test.com"),
            new Claim(ClaimTypes.Role, Role.Student.ToString())
        };

        var identity = new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
        var user = new ClaimsPrincipal(identity);
        
        await authStateProvider.SetAuthenticationStateAsync(user, token);
        
        var authState = await authStateProvider.GetAuthenticationStateAsync();
        Assert.True(authState.User.Identity?.IsAuthenticated == true);
        
        var roleClaim = authState.User.FindFirst(ClaimTypes.Role);
        Assert.NotNull(roleClaim);
        Assert.Equal(Role.Student.ToString(), roleClaim.Value);
        
        var authContext = RegisterServices(authStateProvider, syncAuthState: true);

        var cut = RenderComponent<CascadingAuthenticationState>(childContent => childContent
            .AddChildContent<Home>());

        cut.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("Get Started", cut.Markup);
        }, timeout: TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void HomePage_Renders_Card_Layout()
    {
        var authStateProvider = CreateAuthStateProvider();
        var authContext = RegisterServices(authStateProvider);
        authContext.SetNotAuthorized();

        var cut = RenderComponent<Home>();

        // Check for card structure
        var cards = cut.FindAll(".card");
        Assert.True(cards.Count >= 2); // At least Available Courses and Get Started cards
    }
}
