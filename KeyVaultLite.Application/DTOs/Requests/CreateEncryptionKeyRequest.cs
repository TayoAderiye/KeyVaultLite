using System.ComponentModel.DataAnnotations;

namespace KeyVaultLite.Application.DTOs.Requests
{
    public class CreateEncryptionKeyRequest
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
