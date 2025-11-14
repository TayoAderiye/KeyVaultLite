using System;

namespace KeyVaultLite.Domain.Entities.Base
{
    public class EntityTimeStamp : IEntityTimeStamp
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}
