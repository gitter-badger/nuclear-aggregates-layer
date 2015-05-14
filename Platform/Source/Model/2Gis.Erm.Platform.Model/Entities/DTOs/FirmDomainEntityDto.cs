using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class FirmDomainEntityDto : IDomainEntityDto<Firm>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public Guid ReplicationCode { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int PromisingScore { get; set; }

        [DataMember]
        public UsingOtherMediaOption UsingOtherMedia { get; set; }

        [DataMember]
        public ProductType ProductType { get; set; }

        [DataMember]
        public MarketType MarketType { get; set; }

        [DataMember]
        public EntityReference OrganizationUnitRef { get; set; }

        [DataMember]
        public EntityReference TerritoryRef { get; set; }

        [DataMember]
        public EntityReference ClientRef { get; set; }

        [DataMember]
        public bool ClosedForAscertainment { get; set; }

        [DataMember]
        public DateTime? LastQualifyTime { get; set; }

        [DataMember]
        public DateTime? LastDisqualifyTime { get; set; }

        [DataMember]
        public string Information { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public EntityReference OwnerRef { get; set; }

        [DataMember]
        public EntityReference CreatedByRef { get; set; }

        [DataMember]
        public EntityReference ModifiedByRef { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        public byte[] Timestamp { get; set; }

        [DataMember]
        public BudgetType BudgetType { get; set; }

        [DataMember]
        public Geolocation Geolocation { get; set; }

        [DataMember]
        public InCityBranchesAmount InCityBranchesAmount { get; set; }

        [DataMember]
        public OutCityBranchesAmount OutCityBranchesAmount { get; set; }

        [DataMember]
        public StaffAmount StaffAmount { get; set; }

        [DataMember]
        public Guid? ClientReplicationCode { get; set; }

        [DataMember]
        public Uri BasicOperationsServiceUrl { get; set; }
    }
}