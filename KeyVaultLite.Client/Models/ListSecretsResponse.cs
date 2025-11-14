namespace KeyVaultLite.Client.Models;

public class ListSecretsResponse
{
    public List<SecretSummary> Secrets { get; set; } = new();
    public int Total { get; set; }
}

