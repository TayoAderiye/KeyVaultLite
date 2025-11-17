namespace KeyVaultLite.Application.DTOs.Responses;

public class SecretResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Value { get; set; } // Only included when retrieving a single secret
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Version { get; set; }
}

