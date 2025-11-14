using KeyVaultLite.Domain.Entities.Base;

namespace KeyVaultLite.Domain.Entities;

public class Secret : BaseEntity<Guid>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public byte[] EncryptedValue { get; set; } = Array.Empty<byte>();
    public byte[] EncryptionIV { get; set; } = Array.Empty<byte>();    
    public string? Tags { get; set; } // JSON array stored as string    
    public int Version { get; set; } = 1;
    public Guid EnvironmentId { get; set; }    
    public Environment? Environment { get; set; }
}

