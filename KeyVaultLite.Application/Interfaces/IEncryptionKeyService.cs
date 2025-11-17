using KeyVaultLite.Application.DTOs.Requests;
using KeyVaultLite.Application.DTOs.Responses;

namespace KeyVaultLite.Application.Interfaces
{
    public interface IEncryptionKeyService
    {
        Task<EncryptionKeyResponse> CreateAsync(CreateEncryptionKeyRequest request, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<EncryptionKeyResponse>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
        Task<EncryptionKeyResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // Internal use only (e.g., to encrypt/decrypt secrets)
        Task<byte[]> GetKeyBytesAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
