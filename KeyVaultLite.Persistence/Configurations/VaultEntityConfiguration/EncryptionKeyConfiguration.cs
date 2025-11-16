using KeyVaultLite.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KeyVaultLite.Persistence.Configurations.VaultEntityConfiguration
{
    public class EncryptionKeyConfiguration : IEntityTypeConfiguration<EncryptionKey>
    {
        public void Configure(EntityTypeBuilder<EncryptionKey> builder)
        {
            builder.HasKey(d => d.Id);
            builder.HasIndex(e => e.Name).IsUnique();
            builder.Property(e => e.Description).HasMaxLength(300);

            builder.ToTable("EncryptionKeys", DatabaseSchemas.Vault);
        }
    }
}
