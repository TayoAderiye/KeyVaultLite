using KeyVaultLite.Domain.Entities.Base;

namespace KeyVaultLite.Domain.Entities
{
    public class EncryptionKey : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] KeyBytes { get; set; } = default!;
        public bool IsActive { get; set; } = true;
    }
}