using Google.Protobuf.WellKnownTypes;
using KeyVaultLite.Application.Interfaces;
using KeyVaultLite.Persistence;
using KeyVaultLite.Persistence.Configurations;
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
                            sql.MigrationsAssembly(MigrationAssemblies.SqlServer);
                            sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                            sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                            sql.MigrationsHistoryTable("__VaultMigrationsHistory", DatabaseSchemas.Vault);
                        });
                        break;
                    case var p when string.Equals(p, "sqlite", StringComparison.OrdinalIgnoreCase):
                        options.UseSqlite(connectionString, sqlite =>
                        {
                            sqlite.MigrationsAssembly(MigrationAssemblies.Sqlite);
                            sqlite.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                            sqlite.MigrationsHistoryTable("__VaultMigrationsHistory", DatabaseSchemas.Vault);
                        });
                        break;
                    case var p when string.Equals(p, "postgresql", StringComparison.OrdinalIgnoreCase):
                        options.UseNpgsql(connectionString, npgsql =>
                        {
                            npgsql.MigrationsAssembly(MigrationAssemblies.PostgreSql);
                            npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                            npgsql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                            npgsql.MigrationsHistoryTable("__VaultMigrationsHistory", DatabaseSchemas.Vault);
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
