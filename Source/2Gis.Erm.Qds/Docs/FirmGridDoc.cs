using System;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.Qds.Docs
{
    public sealed class FirmGridDoc : IOperationSpecificEntityDto, IAuthorizationDoc
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public string TerritoryId { get; set; }
        public string TerritoryName { get; set; }
        public int? PromisingScore { get; set; }
        public DateTime? LastQualifyTime { get; set; }
        public DateTime? LastDisqualifyTime { get; set; }
        public string OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? ClosedForAscertainment { get; set; }

        public DocumentAuthorization Authorization { get; set; }
    }
}