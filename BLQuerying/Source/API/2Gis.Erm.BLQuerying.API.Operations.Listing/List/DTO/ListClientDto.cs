using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListClientDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public Guid ReplicationCode { get; set; }
        public string Name { get; set; }
        public string MainAddress { get; set; }
        public long TerritoryId { get; set; }
        public string TerritoryName { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public bool IsAdvertisingAgency { get; set; }

        public long? MainFirmId { get; set; }
        public string MainFirmName { get; set; }
        public string MainPhoneNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastQualifyTime { get; set; }
        public DateTime? LastDisqualifyTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public InformationSource InformationSourceEnum { get; set; }
        public bool IsOwner { get; set; }
    }
}