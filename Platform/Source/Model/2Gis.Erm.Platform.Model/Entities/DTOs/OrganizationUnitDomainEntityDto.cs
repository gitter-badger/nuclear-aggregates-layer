using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class OrganizationUnitDomainEntityDto : IDomainEntityDto<OrganizationUnit>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public Guid ReplicationCode { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int? DgppId { get; set; }

        [DataMember]
        public DateTime FirstEmitDate { get; set; }

        [DataMember]
        public EntityReference CountryRef { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

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
        public string Code { get; set; }

        [DataMember]
        public string SyncCode1C { get; set; }

        [DataMember]
        public EntityReference TimeZoneRef { get; set; }

        [DataMember]
        public string ElectronicMedia { get; set; }

        [DataMember]
        public DateTime? ErmLaunchDate { get; set; }

        [DataMember]
        public DateTime? InfoRussiaLaunchDate { get; set; }
    }
}