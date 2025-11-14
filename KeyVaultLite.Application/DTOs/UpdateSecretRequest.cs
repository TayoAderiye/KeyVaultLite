using System.ComponentModel.DataAnnotations;

namespace KeyVaultLite.Application.DTOs;

public class UpdateSecretRequest
{
    [MaxLength(10000)]
    public string? Value { get; set; }
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    public List<string>? Tags { get; set; }
}

