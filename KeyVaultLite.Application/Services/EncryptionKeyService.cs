using KeyVaultLite.Application.DTOs.Requests;
using KeyVaultLite.Application.DTOs.Responses;
using KeyVaultLite.Application.Interfaces;
using KeyVaultLite.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeyVaultLite.Application.Services
{
    public class EncryptionKeyService : IEncryptionKeyService
    {
        private readonly IKeyVaultDbContext _context;
        private readonly IEncryptionService _encryptionService;

        public EncryptionKeyService(IKeyVaultDbContext context, IEncryptionService encryptionService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        }

        public async Task<EncryptionKeyResponse> CreateAsync(CreateEncryptionKeyRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Name is required", nameof(request.Name));

            var exists = await _context.EncryptionKeys
                .AnyAsync(k => k.Name == request.Name && !k.IsDeleted, cancellationToken);

            if (exists)
                throw new InvalidOperationException($"Encryption key with name '{request.Name}' already exists.");

            var keyBytes = _encryptionService.GenerateKey(); // 32 bytes

            var key = new EncryptionKey
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request?.Description,
                KeyBytes = keyBytes,
                IsActive = true,
                IsDeleted = false
            };

            _context.EncryptionKeys.Add(key);
            await _context.SaveChangesAsync(cancellationToken);

            return MapToResponse(key);
        }

        public async Task<IReadOnlyList<EncryptionKeyResponse>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
        {
            var query = _context.EncryptionKeys.AsQueryable();

            query = query.Where(k => !k.IsDeleted);

            if (!includeInactive)
            {
                query = query.Where(k => k.IsActive);
            }

            var keys = await query
                .OrderByDescending(k => k.CreatedAt)
                .ToListAsync(cancellationToken);

            return [.. keys.Select(MapToResponse)];
        }

        public async Task<EncryptionKeyResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var key = await _context.EncryptionKeys
                .Where(k => k.Id == id && !k.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            return key == null ? throw new KeyNotFoundException($"Encryption key with id '{id}' was not found.") : MapToResponse(key);
        }

        public async Task<byte[]> GetKeyBytesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var key = await _context.EncryptionKeys
                .Where(k => k.Id == id && !k.IsDeleted && k.IsActive)
                .Select(k => new { k.KeyBytes })
                .FirstOrDefaultAsync(cancellationToken);

            return key == null ? throw new KeyNotFoundException($"Encryption key with id '{id}' was not found or inactive.") : key.KeyBytes;
        }

        private static EncryptionKeyResponse MapToResponse(EncryptionKey key)
            => new()
            {
                Id = key.Id,
                Name = key.Name,
                Description = key.Description,
                IsActive = key.IsActive,
                CreatedAt = key.CreatedAt,
                UpdatedAt = key.UpdatedAt
            };
    }
}
