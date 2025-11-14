
namespace KeyVaultLite.Application.Interfaces;

public interface IEnvironmentService
{
    Task<List<Domain.Entities.Environment>> ListEnvironmentsAsync();
    Task<Domain.Entities.Environment?> GetEnvironmentAsync(Guid id);
    Task<Domain.Entities.Environment?> GetEnvironmentByNameAsync(string name);
    Task<Domain.Entities.Environment> CreateEnvironmentAsync(string name, string? description = null);
}

