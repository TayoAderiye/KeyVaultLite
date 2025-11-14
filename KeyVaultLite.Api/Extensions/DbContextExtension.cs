using KeyVaultLite.Application.Interfaces;
using KeyVaultLite.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KeyVaultLite.Api.Extensions
{
    public static class DbContextExtension
    {
        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DataBase");
            var provider = configuration.GetConnectionString("Provider");

            ArgumentNullException.ThrowIfNull(services, nameof(services));
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            if (string.IsNullOrWhiteSpace(provider))
                throw new ArgumentNullException(nameof(provider), "Database provider is not configured.");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "Database connection string is not configured.");

            services.AddDbContext<KeyVaultDbContext>(options =>
            {
                switch (provider)
                {
                    case var p when string.Equals(p, "sqlserver", StringComparison.OrdinalIgnoreCase):
                        options.UseSqlServer(connectionString);
                        break;
                    case var p when string.Equals(p, "sqlite", StringComparison.OrdinalIgnoreCase):
                        options.UseSqlite(connectionString);
                        break;
                    case var p when string.Equals(p, "postgresql", StringComparison.OrdinalIgnoreCase):
                        options.UseNpgsql(connectionString);
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported database provider: {provider}");
                }

                options.EnableSensitiveDataLogging();
            });

            services.AddScoped<IKeyVaultDbContext, KeyVaultDbContext>();
        }

        public static async Task MigrateDbContext(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var keyVaultDbContext = scope.ServiceProvider.GetService<KeyVaultDbContext>();
            var envService = scope.ServiceProvider.GetRequiredService<IEnvironmentService>();

            await keyVaultDbContext.Database.EnsureCreatedAsync();

            await keyVaultDbContext.Database.MigrateAsync();

            if (!await keyVaultDbContext.Environments.AnyAsync(e => e.Name.Equals("development", StringComparison.OrdinalIgnoreCase)))
            {
                await keyVaultDbContext.AddAsync(new Domain.Entities.Environment
                {
                    Name = "development",
                    Description = "Development environment for testing",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                await keyVaultDbContext.SaveChangesAsync();
            }
        }
    }
}
