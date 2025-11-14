using KeyVaultLite.Application.Interfaces;
using KeyVaultLite.Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.Json;

namespace KeyVaultLite.Application.Extension
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddControllers(options =>
            {
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
            services.AddMvc();
            services.AddSiteCors();
            services.AddSiteSwagger();
            services.AddOwnService();
            services.AddHealthChecks();
            return services;
        }
        public static void AddOwnService(this IServiceCollection services)
        {
            services.AddScoped<IEnvironmentService, EnvironmentService>();
            services.AddScoped<ISecretService, SecretService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
        }
    }
}
