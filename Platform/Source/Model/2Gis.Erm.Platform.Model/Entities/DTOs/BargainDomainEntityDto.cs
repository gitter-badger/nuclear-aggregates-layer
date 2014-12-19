using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class BargainDomainEntityDto : IDomainEntityDto<Bargain>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public EntityReference BargainTypeRef { get; set; }

        [DataMember]
        public EntityReference CustomerLegalPersonRef { get; set; }

        [DataMember]
        public EntityReference ExecutorBranchOfficeRef { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public DateTime SignedOn { get; set; }

        [DataMember]
        public int DgppId { get; set; }

        [DataMember]
        public EntityReference OwnerRef { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public EntityReference CreatedByRef { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        public EntityReference ModifiedByRef { get; set; }

        [DataMember]
        public Guid ReplicationCode { get; set; }

        [DataMember]
        public DateTime? ClosedOn { get; set; }

        [DataMember]
        public DocumentsDebt HasDocumentsDebt { get; set; }

        [DataMember]
        public string DocumentsComment { get; set; }

        [DataMember]
        public byte[] Timestamp { get; set; }

        [DataMember]
        public DateTime? BargainEndDate { get; set; }

        [DataMember]
        public BargainKind BargainKind { get; set; }

        // COMMENT {all, 10.07.2014}: Мне кажется, что эти поля (права доступа) не имеют отношения к сущности договора, и им не место в DomainEntityDto. 
        // С другой стороны "у нас так принято". Кто-нибудь может помочь разрешить этот вопрос?
        [DataMember]
        public bool UserCanWorkWithAdvertisingAgencies { get; set; }

        [DataMember]
        public bool IsLegalPersonChoosingDenied { get; set; }

        [DataMember]
        public bool IsBranchOfficeOrganizationUnitChoosingDenied { get; set; }

        [DataMember]
        public long ClientId { get; set; }
    }
}