using System.Net;
using System.Net.Http.Json;
using KeyVaultLite.Client.Models;

namespace KeyVaultLite.Client;

public class KeyVaultClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public KeyVaultClient(string baseUrl, HttpClient? httpClient = null)
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _httpClient = httpClient ?? new HttpClient();
        _httpClient.BaseAddress = new Uri(_baseUrl);
    }

    /// <summary>
    /// List all secrets (metadata only, not values)
    /// </summary>
    public async Task<ListSecretsResponse> ListSecretsAsync(string? tag = null, string? search = null, CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(tag))
            queryParams.Add($"tag={Uri.EscapeDataString(tag)}");
        if (!string.IsNullOrEmpty(search))
            queryParams.Add($"search={Uri.EscapeDataString(search)}");

        var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
        var response = await _httpClient.GetAsync($"/api/secrets{queryString}", cancellationToken);
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ListSecretsResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Get a secret by name (includes decrypted value)
    /// </summary>
    public async Task<Secret> GetSecretAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        var response = await _httpClient.GetAsync($"/api/secrets/{Uri.EscapeDataString(name)}", cancellationToken);
        
        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new KeyNotFoundException($"Secret with name '{name}' not found");

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Secret>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Create a new secret
    /// </summary>
    public async Task<Secret> CreateSecretAsync(CreateSecretRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var response = await _httpClient.PostAsJsonAsync("/api/secrets", request, cancellationToken);
        
        if (response.StatusCode == HttpStatusCode.Conflict)
            throw new InvalidOperationException($"Secret with name '{request.Name}' already exists");

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Secret>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Update an existing secret
    /// </summary>
    public async Task<Secret> UpdateSecretAsync(string name, UpdateSecretRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var response = await _httpClient.PutAsJsonAsync($"/api/secrets/{Uri.EscapeDataString(name)}", request, cancellationToken);
        
        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new KeyNotFoundException($"Secret with name '{name}' not found");

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Secret>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Delete a secret
    /// </summary>
    public async Task DeleteSecretAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        var response = await _httpClient.DeleteAsync($"/api/secrets/{Uri.EscapeDataString(name)}", cancellationToken);
        
        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new KeyNotFoundException($"Secret with name '{name}' not found");

        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Check if the API is healthy
    /// </summary>
    public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/health", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

