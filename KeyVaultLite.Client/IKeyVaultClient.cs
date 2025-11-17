using KeyVaultLite.Client.Models;

namespace KeyVaultLite.Client
{
    public interface IKeyVaultClient : IDisposable
    {    // Environments
        Task<IReadOnlyList<EnvironmentInfo>> GetEnvironmentsAsync(CancellationToken cancellationToken = default);
        Task<EnvironmentInfo?> GetEnvironmentByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<EnvironmentInfo?> GetEnvironmentByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<EnvironmentInfo> CreateEnvironmentAsync(string name, string? description = null, CancellationToken cancellationToken = default);

        // Encryption keys
        Task<EncryptionKey> CreateEncryptionKeyAsync(string name, string? description = null, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<EncryptionKey>> GetEncryptionKeysAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
        Task<EncryptionKey?> GetEncryptionKeyByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // Secrets
        Task<ListSecretsResponse> ListSecretsAsync(Guid environmentId, string? search = null, CancellationToken cancellationToken = default);
        Task<Secret?> GetSecretAsync(Guid environmentId, Guid secretId, CancellationToken cancellationToken = default);
        Task<Secret?> GetSecretAsync(Guid environmentId, string name, CancellationToken cancellationToken = default);
        Task<Secret?> RevealSecretAsync(Guid environmentId, Guid secretId, CancellationToken cancellationToken = default);
        Task<Secret?> RevealSecretAsync(Guid environmentId, string name, CancellationToken cancellationToken = default);
        Task<Secret> CreateSecretAsync(CreateSecretRequest request, CancellationToken cancellationToken = default);
        Task<Secret> UpdateSecretAsync(Guid environmentId, Guid secretId, UpdateSecretRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteSecretAsync(Guid environmentId, string name, CancellationToken cancellationToken = default);

        // (Optional) rotate secret with new key – uncomment when you expose route in API
        // Task<Secret> ReEncryptSecretWithNewKeyAsync(Guid secretId, Guid newEncryptionKeyId, CancellationToken cancellationToken = default);
    }
}