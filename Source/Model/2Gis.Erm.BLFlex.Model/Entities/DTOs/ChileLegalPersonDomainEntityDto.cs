using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Model.Entities.DTOs
{
    [DataContract]
    public class ChileLegalPersonDomainEntityDto : IDomainEntityDto<Platform.Model.Entities.Erm.LegalPerson>
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public LegalPersonType LegalPersonTypeEnum { get; set; }
        [DataMember]
        public EntityReference ClientRef { get; set; }
        [DataMember]
        public string LegalName { get; set; }
        [DataMember]
        public string LegalAddress { get; set; }
        [DataMember]
        public string Rut { get; set; }
        [DataMember]
        public string Comment { get; set; }
        [DataMember]
        public string CardNumber { get; set; }
        [DataMember]
        public string OperationsKind { get; set; }
        [DataMember]
        public EntityReference CommuneRef { get; set; }
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
        public bool HasProfiles { get; set; }
    }
}
