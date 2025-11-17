namespace KeyVaultLite.Application.DTOs.Responses;

public class ListSecretsResponse
{
    public List<SecretSummaryResponse> Secrets { get; set; } = new();
    public int Total { get; set; }
}

