using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace KeyVaultLite.Application.Extension
{
    public static class CorsExtension
    {
        public const string corsPolicyName = "SiteCorsPolicy";
        public static void AddSiteCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(corsPolicyName, builder =>
                {
                    builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
        }

        public static void UseSiteCors(this IApplicationBuilder app)
        {
            app.UseCors(corsPolicyName);
        }
    }
}