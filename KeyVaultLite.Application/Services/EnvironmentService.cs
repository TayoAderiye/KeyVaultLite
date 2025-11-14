using KeyVaultLite.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KeyVaultLite.Application.Services;

public class EnvironmentService(IKeyVaultDbContext context) : IEnvironmentService
{

    public async Task<List<Domain.Entities.Environment>> ListEnvironmentsAsync()
    {
        return await context.Environments
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<Domain.Entities.Environment?> GetEnvironmentAsync(Guid id)
    {
        return await context.Environments
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Domain.Entities.Environment?> GetEnvironmentByNameAsync(string name)
    {
        return await context.Environments
            .FirstOrDefaultAsync(e => e.Name == name);
    }

    public async Task<Domain.Entities.Environment> CreateEnvironmentAsync(string name, string? description = null)
    {
        var environment = new Domain.Entities.Environment
        {
            Name = name,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Environments.Add(environment);
        await context.SaveChangesAsync();

        return environment;
    }
}

