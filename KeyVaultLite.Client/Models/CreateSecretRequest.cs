namespace KeyVaultLite.Client.Models;

public class CreateSecretRequest
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
    public Guid EnvironmentId { get; set; }
    public Guid EncryptionKeyId { get; set; }
}

