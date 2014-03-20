using System;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.Qds.Docs
{
    public class ClientGridDoc : IOperationSpecificEntityDto, IAuthorizationDoc
    {
        public long Id { get; set; }
        public Guid ReplicationCode { get; set; }
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

        public DocumentAuthorization Authorization { get; set; }
    }
}