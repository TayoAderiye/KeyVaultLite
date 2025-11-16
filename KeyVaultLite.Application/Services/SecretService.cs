using KeyVaultLite.Application.DTOs;
using KeyVaultLite.Application.Interfaces;
using KeyVaultLite.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace KeyVaultLite.Application.Services;

public class SecretService(IKeyVaultDbContext context, IEncryptionService encryptionService) : ISecretService
{

    public async Task<ListSecretsResponse> ListSecretsAsync(Guid environmentId, string? tag = null, string? search = null)
    {
        var query = context.Secrets
            .Where(s => s.EnvironmentId == environmentId);

        if (!string.IsNullOrEmpty(tag))
        {
            query = query.Where(s => s.Tags != null && s.Tags.Contains(tag));
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(s =>
                s.Name.Contains(search) ||
                s.Description != null && s.Description.Contains(search));
        }

        var secrets = await query
            .OrderBy(s => s.Name)
            .ToListAsync();

        var summaries = secrets.Select(s => new SecretSummaryResponse
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            Tags = ParseTags(s.Tags),
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt,
            Version = s.Version
        }).ToList();

        return new ListSecretsResponse
        {
            Secrets = summaries,
            Total = summaries.Count
        };
    }

    public async Task<SecretResponse?> GetSecretAsync(Guid environmentId, string name)
    {
        var secret = await context.Secrets
             .Include(s => s.EncryptionKey)
            .FirstOrDefaultAsync(s => s.Name == name && s.EnvironmentId == environmentId);

        if (secret == null)
            return null;

        var decryptedValue = encryptionService.Decrypt(secret.EncryptedValue, secret.EncryptionIV, secret.EncryptionKey.KeyBytes);

        return new SecretResponse
        {
            Id = secret.Id,
            Name = secret.Name,
            Value = decryptedValue,
            Description = secret.Description,
            Tags = ParseTags(secret.Tags),
            CreatedAt = secret.CreatedAt,
            UpdatedAt = secret.UpdatedAt,
            Version = secret.Version
        };
    }

    public async Task<SecretResponse> CreateSecretAsync(CreateSecretRequest request)
    {
        // Check if secret already exists in this environment
        var exists = await context.Secrets
            .AnyAsync(s => s.Name == request.Name && s.EnvironmentId == request.EnvironmentId);

        if (exists)
            throw new InvalidOperationException($"Secret with name '{request.Name}' already exists in this environment");

        // 🔥 Load the encryption key the user selected
        var encryptionKey = await context.EncryptionKeys
            .FirstOrDefaultAsync(k => k.Id == request.EncryptionKeyId && k.IsActive);

        if (encryptionKey is null)
            throw new InvalidOperationException($"Encryption key '{request.EncryptionKeyId}' not found or inactive");

        // Encrypt the value
        var (encryptedValue, iv) = encryptionService.Encrypt(request.Value, encryptionKey.KeyBytes);

        var secret = new Secret
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            EncryptedValue = encryptedValue,
            EncryptionIV = iv,
            Tags = request.Tags != null ? JsonSerializer.Serialize(request.Tags) : null,
            EnvironmentId = request.EnvironmentId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 1
        };

        context.Secrets.Add(secret);
        await context.SaveChangesAsync();

        return new SecretResponse
        {
            Id = secret.Id,
            Name = secret.Name,
            Description = secret.Description,
            Tags = request.Tags,
            CreatedAt = secret.CreatedAt,
            UpdatedAt = secret.UpdatedAt,
            Version = secret.Version
        };
    }

    public async Task<SecretResponse> ReEncryptSecretWithNewKeyAsync(Guid secretId, Guid newEncryptionKeyId)
    {
        // 1. Load the secret including its current encryption key
        var secret = await context.Secrets
            .Include(s => s.EncryptionKey)
            .FirstOrDefaultAsync(s => s.Id == secretId) ?? throw new InvalidOperationException($"Secret with id '{secretId}' not found");

        // 2. If it's already using that key, no point rotating
        if (secret.EncryptionKeyId == newEncryptionKeyId)
            throw new InvalidOperationException("Secret is already encrypted with the selected encryption key");

        // 3. Load the new encryption key
        var newKey = await context.EncryptionKeys
            .FirstOrDefaultAsync(k => k.Id == newEncryptionKeyId && k.IsActive) ?? throw new InvalidOperationException($"Encryption key '{newEncryptionKeyId}' not found or inactive");

        // (Optional) If you want to enforce same environment / tenant, check that here

        // 4. Decrypt using the old key
        if (secret.EncryptionKey is null)
            throw new InvalidOperationException("Secret does not have a valid encryption key associated");

        var plaintext = encryptionService.Decrypt(
            secret.EncryptedValue,
            secret.EncryptionIV,
            secret.EncryptionKey.KeyBytes);

        // 5. Re-encrypt with the new key
        var (newEncryptedValue, newIv) = encryptionService.Encrypt(plaintext, newKey.KeyBytes);

        // 6. Update the secret
        secret.EncryptedValue = newEncryptedValue;
        secret.EncryptionIV = newIv;
        secret.EncryptionKeyId = newKey.Id;
        secret.EncryptionKey = newKey;
        secret.UpdatedAt = DateTime.UtcNow;
        secret.Version += 1; // optional but nice

        await context.SaveChangesAsync();

        // 7. Return updated DTO
        return new SecretResponse
        {
            Id = secret.Id,
            Name = secret.Name,
            Description = secret.Description,
            Tags = ParseTags(secret.Tags),
            CreatedAt = secret.CreatedAt,
            UpdatedAt = secret.UpdatedAt,
            Version = secret.Version
        };
    }

    public async Task<SecretResponse> UpdateSecretAsync(Guid environmentId, string name, UpdateSecretRequest request)
    {
        var secret = await context.Secrets
            .Include(s => s.EncryptionKey)
            .FirstOrDefaultAsync(s => s.Name == name && s.EnvironmentId == environmentId) ?? throw new KeyNotFoundException($"Secret with name '{name}' not found in this environment");

        // Update encrypted value if provided
        if (!string.IsNullOrEmpty(request.Value))
        {
            var (encryptedValue, iv) = encryptionService.Encrypt(request.Value, secret.EncryptionKey.KeyBytes);
            secret.EncryptedValue = encryptedValue;
            secret.EncryptionIV = iv;
        }

        // Update description if provided
        if (request.Description != null)
        {
            secret.Description = request.Description;
        }

        // Update tags if provided
        if (request.Tags != null)
        {
            secret.Tags = JsonSerializer.Serialize(request.Tags);
        }

        secret.UpdatedAt = DateTime.UtcNow;
        secret.Version++;

        await context.SaveChangesAsync();

        return new SecretResponse
        {
            Id = secret.Id,
            Name = secret.Name,
            Description = secret.Description,
            Tags = ParseTags(secret.Tags),
            CreatedAt = secret.CreatedAt,
            UpdatedAt = secret.UpdatedAt,
            Version = secret.Version
        };
    }

    public async Task<bool> DeleteSecretAsync(Guid environmentId, string name)
    {
        var secret = await context.Secrets
            .FirstOrDefaultAsync(s => s.Name == name && s.EnvironmentId == environmentId);

        if (secret == null)
            return false;

        context.Secrets.Remove(secret);
        await context.SaveChangesAsync();

        return true;
    }

    private static List<string>? ParseTags(string? tagsJson)
    {
        if (string.IsNullOrEmpty(tagsJson))
            return null;

        try
        {
            return JsonSerializer.Deserialize<List<string>>(tagsJson);
        }
        catch
        {
            return null;
        }
    }
}

