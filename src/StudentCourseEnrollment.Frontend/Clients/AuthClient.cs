using System.Net;
using System.Net.Http.Json;
using StudentCourseEnrollment.Shared.DTOs.Auth;

namespace StudentCourseEnrollment.Frontend.Clients;

public class AuthClient : IAuthClient
{
    private readonly HttpClient _httpClient;

    public AuthClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<RegisterResponse?> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/auth/register", request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<RegisterResponse>();
            }
            
            var errorMessage = await GetErrorMessageAsync(response);
            throw new HttpRequestException(errorMessage);
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            throw new HttpRequestException("The request took too long to complete. Please check your internet connection and try again.", ex);
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Unable to connect to the server. Please check your internet connection and try again.", ex);
        }
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/auth/login", request);
        
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }
        
        var errorMessage = await GetErrorMessageAsync(response);
        throw new HttpRequestException(errorMessage);
    }

    public async Task<ProvisionUserResponse?> ProvisionUserAsync()
    {
        try
        {
            var response = await _httpClient.PostAsync("/auth/provision", null);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProvisionUserResponse>();
            }
            
            var errorMessage = await GetErrorMessageAsync(response);
            throw new HttpRequestException(errorMessage);
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            throw new HttpRequestException("The request took too long to complete. Please check your internet connection and try again.", ex);
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Unable to connect to the server. Please check your internet connection and try again.", ex);
        }
    }

    private async Task<string> GetErrorMessageAsync(HttpResponseMessage response)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync();
            
            if (content.StartsWith("{") && content.Contains("\"title\""))
            {
                using var doc = System.Text.Json.JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("detail", out var detail) && !string.IsNullOrWhiteSpace(detail.GetString()))
                {
                    return detail.GetString()!;
                }
                if (doc.RootElement.TryGetProperty("title", out var title))
                {
                    return title.GetString() ?? $"Error: {response.StatusCode}";
                }
            }
            
            if (!string.IsNullOrWhiteSpace(content) && content.Length < 200)
            {
                return content;
            }
        }
        catch
        {
        }
        
        return response.StatusCode switch
        {
            HttpStatusCode.BadRequest => "Invalid request. Please check your input and try again.",
            HttpStatusCode.Unauthorized => "Invalid email or password. Please try again.",
            HttpStatusCode.Conflict => "This email is already registered. Please use a different email or log in.",
            HttpStatusCode.InternalServerError => "A server error occurred. Please try again in a few moments.",
            HttpStatusCode.ServiceUnavailable => "The service is temporarily unavailable. Please try again later.",
            _ => "An error occurred while processing your request. Please try again."
        };
    }
}
