using KeyVaultLite.Application.Interfaces;
using KeyVaultLite.Domain.Entities;
using KeyVaultLite.Domain.Entities.Base;
using KeyVaultLite.Persistence.Configurations;
using KeyVaultLite.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;

namespace KeyVaultLite.Persistence
{
    public class KeyVaultDbContext : DbContext, IKeyVaultDbContext
    {
        public KeyVaultDbContext(DbContextOptions<KeyVaultDbContext> options)
            : base(options)
        {
        }

        public DbSet<Secret> Secrets { get; set; }
        public DbSet<Domain.Entities.Environment> Environments { get; set; }
        public DbSet<EncryptionKey> EncryptionKeys { get; set; }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.Entries()
                         .Where(x => x is { Entity: IEntityTimeStamp, State: EntityState.Modified })
                         .Select(x => x.Entity)
                         .ForEach((x) => x.GetType().GetProperty(nameof(IEntityTimeStamp.UpdatedAt))?.SetValue(x, DateTime.UtcNow));

            return base.SaveChangesAsync(cancellationToken);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderConfigurations.Configure(modelBuilder);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Model.GetEntityTypes().ToList().ForEach(entityType =>
            {
                if (entityType.ClrType == typeof(BaseEntity<>) || entityType.ClrType == typeof(EntityTimeStamp) || entityType.ClrType == typeof(IEntityTimeStamp))
                    modelBuilder.Entity<EntityTimeStamp>(x => x.UseBaseConfigurations());

                if (entityType.ClrType == typeof(AuditStamp))
                    modelBuilder.Entity<AuditStamp>(x => x.UseAuditTimeStampConfigurations());

                if (entityType.GetTableName() is { } table && table.StartsWith("AspNet"))
                    entityType.SetTableName(table[6..]);
            });
   
        }
    }

}
