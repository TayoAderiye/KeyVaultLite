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
                        options.UseSqlServer(connectionString, sql =>
                        {
                            sql.MigrationsAssembly("KeyVaultLite.Persistence.SqlServer");
                        });
                        break;
                    case var p when string.Equals(p, "sqlite", StringComparison.OrdinalIgnoreCase):
                        options.UseSqlite(connectionString, sqlite =>
                        {
                            sqlite.MigrationsAssembly("KeyVaultLite.Persistence.Sqlite");
                        });

                        break;
                    case var p when string.Equals(p, "postgresql", StringComparison.OrdinalIgnoreCase):
                        options.UseNpgsql(connectionString, npgsql =>
                        {
                            npgsql.MigrationsAssembly("KeyVaultLite.Persistence.PostgreSql");
                        });
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported database provider: {provider}");
                }

                options.EnableSensitiveDataLogging();
            });

            services.AddScoped<IKeyVaultDbContext, KeyVaultDbContext>();
        }       
    }
}
