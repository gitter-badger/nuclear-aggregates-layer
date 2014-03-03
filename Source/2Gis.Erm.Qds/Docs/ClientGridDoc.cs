using System;

namespace DoubleGis.Erm.Qds.Docs
{
    public class ClientGridDoc : IAuthDoc
    {
        public long Id { get; set; }
        public string ReplicationCode { get; set; }
        public string Name { get; set; }
        public string MainAddress { get; set; }
        public long TerritoryId { get; set; }
        public string TerritoryName { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public string InformationSource { get; set; }

        public DocumentAuthorization Auth { get; set; }
    }
}