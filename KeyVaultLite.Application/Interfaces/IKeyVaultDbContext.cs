using KeyVaultLite.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeyVaultLite.Application.Interfaces
{
    public interface IKeyVaultDbContext
    {
        public DbSet<Secret> Secrets { get; set; }
        public DbSet<Domain.Entities.Environment> Environments { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}