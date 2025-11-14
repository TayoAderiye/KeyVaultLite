using KeyVaultLite.Application.DTOs;

namespace KeyVaultLite.Application.Interfaces;

public interface ISecretService
{
    Task<ListSecretsResponse> ListSecretsAsync(Guid environmentId, string? tag = null, string? search = null);
    Task<SecretResponse?> GetSecretAsync(Guid environmentId, string name);
    Task<SecretResponse> CreateSecretAsync(CreateSecretRequest request);
    Task<SecretResponse> UpdateSecretAsync(Guid environmentId, string name, UpdateSecretRequest request);
    Task<bool> DeleteSecretAsync(Guid environmentId, string name);
}

