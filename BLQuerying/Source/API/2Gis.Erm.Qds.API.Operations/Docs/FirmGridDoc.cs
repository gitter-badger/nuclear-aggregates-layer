using System;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.Qds.API.Operations.Docs
{
    public sealed class FirmGridDoc : IOperationSpecificEntityDto, IAuthorizationDoc
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int PromisingScore { get; set; }
        public DateTime? LastQualifyTime { get; set; }
        public DateTime? LastDisqualifyTime { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool ClosedForAscertainment { get; set; }

        // relations
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public string TerritoryId { get; set; }
        public string TerritoryName { get; set; }
        public string OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }

        public DocumentAuthorization Authorization { get; set; }
    }
}