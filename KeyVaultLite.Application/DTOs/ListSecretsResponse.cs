namespace KeyVaultLite.Application.DTOs;

public class ListSecretsResponse
{
    public List<SecretSummaryResponse> Secrets { get; set; } = new();
    public int Total { get; set; }
}

