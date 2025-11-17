using KeyVaultLite.Application.DTOs.Requests;
using KeyVaultLite.Application.DTOs.Responses;

namespace KeyVaultLite.Application.Interfaces;

public interface ISecretService
{
    Task<ListSecretsResponse> ListSecretsAsync(Guid environmentId, string? tag = null, string? search = null, CancellationToken cancellationToken = default);
    Task<SecretResponse?> GetSecretAsync(Guid environmentId, string name, CancellationToken cancellationToken);
    Task<SecretResponse?> GetSecretAsync(Guid environmentId, Guid id, CancellationToken cancellationToken);
    Task<SecretResponse> CreateSecretAsync(CreateSecretRequest request, CancellationToken cancellationToken);
    Task<SecretResponse> UpdateSecretAsync(Guid environmentId, string name, UpdateSecretRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteSecretAsync(Guid environmentId, string name, CancellationToken cancellationToken);
    Task<SecretResponse> ReEncryptSecretWithNewKeyAsync(Guid secretId, Guid newEncryptionKeyId, CancellationToken cancellationToken);
    Task<SecretResponse?> RevealSecretAsync(Guid environmentId, string name, CancellationToken cancellationToken);
    Task<SecretResponse?> RevealSecretAsync(Guid environmentId, Guid id, CancellationToken cancellationToken);
}

