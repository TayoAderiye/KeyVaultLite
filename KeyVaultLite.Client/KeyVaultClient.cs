using System.Net;
using System.Net.Http.Json;
using KeyVaultLite.Client.Models;

namespace KeyVaultLite.Client;

public class KeyVaultClient : IKeyVaultClient
{
    private const string ApiPrefix = "api/v1/vault/";
    private readonly HttpClient _httpClient;

    public KeyVaultClient(string baseUrl, HttpClient? httpClient = null)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("Base URL is required", nameof(baseUrl));

        _httpClient = httpClient ?? new HttpClient();
        _httpClient.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
    }

    #region Environments

    public async Task<IReadOnlyList<EnvironmentInfo>> GetEnvironmentsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"{ApiPrefix}environments", cancellationToken);
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadFromJsonAsync<List<EnvironmentInfo>>(cancellationToken: cancellationToken);
        return items ?? [];
    }

    public async Task<EnvironmentInfo?> GetEnvironmentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"{ApiPrefix}environments/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EnvironmentInfo>(cancellationToken: cancellationToken);
    }

    public async Task<EnvironmentInfo?> GetEnvironmentByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Environment name is required", nameof(name));

        var encoded = Uri.EscapeDataString(name);
        var response = await _httpClient.GetAsync($"{ApiPrefix}environments/{encoded}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EnvironmentInfo>(cancellationToken: cancellationToken);
    }

    public async Task<EnvironmentInfo> CreateEnvironmentAsync(string name, string? description = null, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            Name = name,
            Description = description
        };

        var response = await _httpClient.PostAsJsonAsync($"{ApiPrefix}environments/create", payload, cancellationToken);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<EnvironmentInfo>(cancellationToken: cancellationToken))!;
    }

    #endregion

    #region Encryption keys

    public async Task<EncryptionKey> CreateEncryptionKeyAsync(string name, string? description = null, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            Name = name,
            Description = description
        };

        var response = await _httpClient.PostAsJsonAsync($"{ApiPrefix}keys/create", payload, cancellationToken);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<EncryptionKey>(cancellationToken: cancellationToken))!;
    }

    public async Task<IReadOnlyList<EncryptionKey>> GetEncryptionKeysAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var url = $"{ApiPrefix}keys?includeInactive={includeInactive.ToString().ToLowerInvariant()}";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadFromJsonAsync<List<EncryptionKey>>(cancellationToken: cancellationToken);
        return items ?? [];
    }

    public async Task<EncryptionKey?> GetEncryptionKeyByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"{ApiPrefix}keys/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EncryptionKey>(cancellationToken: cancellationToken);
    }

    #endregion

    #region Secrets

    public async Task<ListSecretsResponse> ListSecretsAsync(Guid environmentId, string? search = null, CancellationToken cancellationToken = default)
    {
        var url = $"{ApiPrefix}environments/{environmentId}/secrets";
        if (!string.IsNullOrWhiteSpace(search))
        {
            url += $"?search={Uri.EscapeDataString(search)}";
        }

        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<ListSecretsResponse>(cancellationToken: cancellationToken))!;
    }

    public async Task<Secret?> GetSecretAsync(Guid environmentId, Guid secretId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"{ApiPrefix}secrets/{environmentId}/{secretId}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Secret>(cancellationToken: cancellationToken);
    }

    public async Task<Secret?> GetSecretAsync(Guid environmentId, string name, CancellationToken cancellationToken = default)
    {
        var encodedName = Uri.EscapeDataString(name);
        var response = await _httpClient.GetAsync($"{ApiPrefix}environments/{environmentId}/secrets/{encodedName}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Secret>(cancellationToken: cancellationToken);
    }

    public async Task<Secret?> RevealSecretAsync(Guid environmentId, Guid secretId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"{ApiPrefix}secrets/{environmentId}/reveal/{secretId}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Secret>(cancellationToken: cancellationToken);
    }

    public async Task<Secret?> RevealSecretAsync(Guid environmentId, string name, CancellationToken cancellationToken = default)
    {
        var encodedName = Uri.EscapeDataString(name);
        var response = await _httpClient.GetAsync($"{ApiPrefix}environments/{environmentId}/secrets/{encodedName}/reveal", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Secret>(cancellationToken: cancellationToken);
    }

    public async Task<Secret> CreateSecretAsync(CreateSecretRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"{ApiPrefix}secrets/create", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<Secret>(cancellationToken: cancellationToken))!;
    }

    public async Task<Secret> UpdateSecretAsync(Guid environmentId, Guid secretId, UpdateSecretRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"{ApiPrefix}secrets/{environmentId}/{secretId}",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Secret>(cancellationToken: cancellationToken))!;
    }

    public async Task<bool> DeleteSecretAsync(Guid environmentId, string name, CancellationToken cancellationToken = default)
    {
        var encodedName = Uri.EscapeDataString(name);
        var response = await _httpClient.DeleteAsync($"{ApiPrefix}secrets/{environmentId}/{encodedName}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        return response.IsSuccessStatusCode;
    }

    #endregion

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}

