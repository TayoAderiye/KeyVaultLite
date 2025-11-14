using KeyVaultLite.Application.Extension;

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
            app.UseSiteCors();
            app.UseSiteSwagger();
            app.UseLoggerProvider();
            //await app.MigrateDbContext();

            return app;
        }
    }
}
