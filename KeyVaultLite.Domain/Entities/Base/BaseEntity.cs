using System;

namespace KeyVaultLite.Domain.Entities.Base
{
    public class BaseEntity<TKey> : EntityTimeStamp where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
    }
}
