namespace KeyVaultLite.Domain.Entities.Base
{
    public class AuditStamp : IAuditTimeStamp
    {
        public string CreatedBy { get; set; }
        public string DeletedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
