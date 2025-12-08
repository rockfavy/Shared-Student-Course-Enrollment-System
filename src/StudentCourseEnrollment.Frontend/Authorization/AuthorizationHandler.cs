using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using StudentCourseEnrollment.Frontend.Services;

namespace StudentCourseEnrollment.Frontend.Authorization;

public class AuthorizationHandler : DelegatingHandler
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IJSRuntime _jsRuntime;

    public AuthorizationHandler(AuthenticationStateProvider authenticationStateProvider, IJSRuntime jsRuntime)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _jsRuntime = jsRuntime;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (_authenticationStateProvider is CustomAuthenticationStateProvider customProvider)
        {
            var token = await customProvider.GetAccessTokenAsync();
            
            if (!string.IsNullOrWhiteSpace(token) && !request.Headers.Contains("Authorization"))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            if (_authenticationStateProvider is CustomAuthenticationStateProvider unauthorizedProvider)
            {
                await unauthorizedProvider.ClearAuthenticationStateAsync();
                
                try
                {
                    await _jsRuntime.InvokeVoidAsync("eval", "window.location.href = '/login'");
                }
                catch
                {
                }
            }
        }

        return response;
    }
}
