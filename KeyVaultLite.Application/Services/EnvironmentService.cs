using KeyVaultLite.Application.DTOs.Requests;
using KeyVaultLite.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KeyVaultLite.Application.Services;

public class EnvironmentService(IKeyVaultDbContext context) : IEnvironmentService
{

    public async Task<List<Domain.Entities.Environment>> ListEnvironmentsAsync(CancellationToken cancellationToken)
    {
        return await context.Environments
            .OrderBy(e => e.CreatedAt)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<Domain.Entities.Environment?> GetEnvironmentAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Environments
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken: cancellationToken);
    }

    public async Task<Domain.Entities.Environment?> GetEnvironmentByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await context.Environments
            .FirstOrDefaultAsync(e => e.Name == name, cancellationToken: cancellationToken);
    }

    public async Task<Domain.Entities.Environment> CreateEnvironmentAsync(CreateEnvironmentRequest request, CancellationToken cancellationToken)
    {
        var environment = new Domain.Entities.Environment
        {
            Name = request.Name,
            Description = request?.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Environments.Add(environment);
        await context.SaveChangesAsync(cancellationToken);

        return environment;
    }
}

