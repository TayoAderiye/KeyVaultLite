using System.ComponentModel.DataAnnotations;

namespace KeyVaultLite.Application.DTOs.Requests;

public class CreateSecretRequest
{
    [Required]
    [MaxLength(255)]
    //[RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Name can only contain alphanumeric characters, hyphens, and underscores")]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(10000)]
    public string Value { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    public List<string>? Tags { get; set; }
    
    [Required]
    public Guid EnvironmentId { get; set; }
    [Required]
    public Guid EncryptionKeyId { get; set; }
}

