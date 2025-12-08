using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Authentication.WebAssembly.Msal;
using Microsoft.Authentication.WebAssembly.Msal.Models;
using StudentCourseEnrollment.Frontend;
using StudentCourseEnrollment.Frontend.Authorization;
using StudentCourseEnrollment.Frontend.Clients;
using StudentCourseEnrollment.Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

if (builder.HostEnvironment.IsProduction())
{
    var azureAdConfig = builder.Configuration.GetSection("AzureAd");
    var authority = azureAdConfig["Authority"];
    var clientId = azureAdConfig["ClientId"];
    
    if (!string.IsNullOrEmpty(authority) && !string.IsNullOrEmpty(clientId))
    {
        builder.Services.AddMsalAuthentication(options =>
        {
            builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
            options.ProviderOptions.DefaultAccessTokenScopes.Add($"api://{clientId}/.default");
            options.ProviderOptions.LoginMode = "redirect";
            options.ProviderOptions.Authentication.RedirectUri = builder.HostEnvironment.BaseAddress + "authentication/login-callback";
            options.ProviderOptions.Authentication.PostLogoutRedirectUri = builder.HostEnvironment.BaseAddress;
        });
    }
    else
    {
        builder.Services.AddScoped<CustomAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthenticationStateProvider>());
    }
}
else
{
    builder.Services.AddScoped<CustomAuthenticationStateProvider>();
    builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthenticationStateProvider>());
}

var apiBaseAddress = builder.Configuration["ApiBaseAddress"] ?? "https://localhost:49541";

if (!apiBaseAddress.EndsWith("/"))
{
    apiBaseAddress += "/";
}

builder.Services.AddHttpClient<IAuthClient, AuthClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
})
.AddHttpMessageHandler<AuthorizationHandler>();

builder.Services.AddHttpClient<ICoursesClient, CoursesClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
})
.AddHttpMessageHandler<AuthorizationHandler>();

builder.Services.AddHttpClient<IEnrollmentsClient, EnrollmentsClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
})
.AddHttpMessageHandler<AuthorizationHandler>();

builder.Services.AddScoped<AuthorizationHandler>();
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
