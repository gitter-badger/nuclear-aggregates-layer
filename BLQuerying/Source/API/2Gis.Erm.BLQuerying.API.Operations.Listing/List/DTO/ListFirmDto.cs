using System;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListFirmDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public long? TerritoryId { get; set; }
        public string TerritoryName { get; set; }
        public int PromisingScore { get; set; }
        public DateTime? LastQualifyTime { get; set; }
        public DateTime? LastDisqualifyTime { get; set; }
        public long OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public DateTime CreatedOn { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool ClosedForAscertainment { get; set; }
        public bool IsOwner { get; set; }
    }
}