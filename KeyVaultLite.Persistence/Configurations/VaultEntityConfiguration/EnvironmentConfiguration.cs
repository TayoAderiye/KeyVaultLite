using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KeyVaultLite.Persistence.Configurations.VaultEntityConfiguration
{
    public class EnvironmentConfiguration : IEntityTypeConfiguration<Domain.Entities.Environment>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Environment> builder)
        {
            builder.HasKey(d => d.Id);
            builder.HasIndex(e => e.Name).IsUnique();
            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Description).HasMaxLength(500);

            builder.ToTable("Environments", DatabaseSchemas.Vault);
        }
    }
}