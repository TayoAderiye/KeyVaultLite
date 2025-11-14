using KeyVaultLite.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KeyVaultLite.Persistence.Configurations.Base
{
    public static class  BaseEntityConfiguration
    {
        public static void UseBaseConfigurations(this EntityTypeBuilder<EntityTimeStamp> builder, string keyName = "Id")
        {
            builder.HasKey(keyName);

            builder.Property(keyName)
                .HasColumnName(keyName)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("CreatedAt")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .IsRequired();

            builder.Property(x => x.DeletedAt)
                .HasColumnName("DeletedAt")
                .IsRequired(false);


            builder.HasQueryFilter(x => x.DeletedAt != null);
        }

        public static void UseAuditTimeStampConfigurations(this EntityTypeBuilder<AuditStamp> builder)
        {
            builder.Property(x => x.CreatedBy)
                .HasColumnName("CreatedBy")
                .IsRequired(false);

            builder.Property(x => x.DeletedBy)
                .HasColumnName("DeletedBy")
                .IsRequired(false);

            builder.Property(x => x.UpdatedBy)
                .HasColumnName("UpdatedBy")
                .IsRequired(false);
        }
    }
}
