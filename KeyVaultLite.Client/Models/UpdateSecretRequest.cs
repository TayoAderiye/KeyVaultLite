namespace KeyVaultLite.Client.Models;

public class UpdateSecretRequest
{
    public string? Value { get; set; }
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
}

