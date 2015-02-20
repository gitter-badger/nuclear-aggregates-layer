using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class PositionDomainEntityDto : IDomainEntityDto<Position>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public Guid ReplicationCode { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsComposite { get; set; }

        [DataMember]
        public PositionCalculationMethod CalculationMethodEnum { get; set; }

        [DataMember]
        public PositionBindingObjectType BindingObjectTypeEnum { get; set; }

        [DataMember]
        public SalesModel SalesModel { get; set; }

        [DataMember]
        public EntityReference PlatformRef { get; set; }

        [DataMember]
        public EntityReference CategoryRef { get; set; }

        [DataMember]
        public EntityReference AdvertisementTemplateRef { get; set; }

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
        public long? DgppId { get; set; }

        [DataMember]
        public int ExportCode { get; set; }

        [DataMember]
        public bool IsControlledByAmount { get; set; }

        [DataMember]
        public bool RestrictChildPositionPlatforms { get; set; }

        [DataMember]
        public bool IsPublished { get; set; }

        [DataMember]
        public bool RestrictChildPositionPlatformsCanBeChanged { get; set; }

        [DataMember]
        public bool IsReadOnlyTemplate { get; set; }
    }
}