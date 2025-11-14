using KeyVaultLite.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KeyVaultLite.Persistence.Configurations.VaultEntityConfiguration
{
    public class SecretConfiguration : IEntityTypeConfiguration<Secret>
    {
        public void Configure(EntityTypeBuilder<Secret> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => new { e.Name, e.EnvironmentId }).IsUnique();
            builder.Property(e => e.Name).IsRequired().HasMaxLength(255);
            builder.Property(e => e.Description).HasMaxLength(1000);
            builder.Property(e => e.EncryptedValue).IsRequired();
            builder.Property(e => e.EncryptionIV).IsRequired();
            builder.HasOne(e => e.Environment)
                    .WithMany()
                    .HasForeignKey(e => e.EnvironmentId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Secrets", DatabaseSchemas.Vault);
        }
    }
}