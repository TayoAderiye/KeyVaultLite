using KeyVaultLite.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace KeyVaultLite.Domain.Entities;

public class Environment : BaseEntity<Guid>
{
    public string Name { get; set; }
    public string? Description { get; set; }
}

