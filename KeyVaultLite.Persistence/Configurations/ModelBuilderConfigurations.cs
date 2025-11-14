using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace KeyVaultLite.Persistence.Configurations
{
    public class ModelBuilderConfigurations
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            ApplyConfigurationsFromAssembly(modelBuilder);
        }

        private static void ApplyConfigurationsFromAssembly(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                Assembly.GetExecutingAssembly(),
                t => t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));
        }
    }
}
