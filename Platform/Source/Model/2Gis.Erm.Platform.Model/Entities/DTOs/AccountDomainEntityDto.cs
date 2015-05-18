using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class AccountDomainEntityDto : IDomainEntityDto<Account>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public Guid ReplicationCode { get; set; }

        [DataMember]
        public long? DgppId { get; set; }

        [DataMember]
        public EntityReference BranchOfficeOrganizationUnitRef { get; set; }

        [DataMember]
        public EntityReference LegalPersonRef { get; set; }

        [DataMember]
        public string LegalPersonSyncCode1C { get; set; }

        [DataMember]
        public decimal Balance { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

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
        public decimal AccountDetailBalance { get; set; }

        [DataMember]
        public EntityReference CurrencyRef { get; set; }

        [DataMember]
        public decimal LockDetailBalance { get; set; }

        [DataMember]
        public bool OwnerCanBeChanged { get; set; }

        [DataMember]
        public Uri BasicOperationsServiceUrl { get; set; }
    }
}