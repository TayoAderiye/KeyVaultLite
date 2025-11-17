using KeyVaultLite.Application.Extension;
using KeyVaultLite.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KeyVaultLite.Api.Extensions
{
    public static class ServiceExtension
    {
        public static async Task<WebApplication> UseServicesAsync(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseHttpsRedirection();
            }
            app.UseRouting();
            app.MapControllers();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

            });
            app.UseHealthChecks("/_health");
            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseSiteCors();
            app.UseSiteSwagger();
            app.UseLoggerProvider();
            await app.MigrateDbContext();

            return app;
        }

        public static async Task MigrateDbContext(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var keyVaultDbContext = scope.ServiceProvider.GetService<KeyVaultDbContext>();

            var pendingMigrations = await keyVaultDbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                await keyVaultDbContext.Database.MigrateAsync();
            }

            if (!await keyVaultDbContext.Environments.AnyAsync(e => e.Name.ToLower() == "development"))
            {
                await keyVaultDbContext.AddAsync(new Domain.Entities.Environment
                {
                    Name = "Development",
                    Description = "Development environment for testing",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                await keyVaultDbContext.SaveChangesAsync();
            }
        }
    }
}
