using KeyVaultLite.Application.DTOs.Requests;

namespace KeyVaultLite.Application.Interfaces;

public interface IEnvironmentService
{
    Task<List<Domain.Entities.Environment>> ListEnvironmentsAsync(CancellationToken cancellationToken);
    Task<Domain.Entities.Environment?> GetEnvironmentAsync(Guid id, CancellationToken cancellationToken);
    Task<Domain.Entities.Environment?> GetEnvironmentByNameAsync(string name, CancellationToken cancellationToken);
    Task<Domain.Entities.Environment> CreateEnvironmentAsync(CreateEnvironmentRequest request, CancellationToken cancellationToken);
}

