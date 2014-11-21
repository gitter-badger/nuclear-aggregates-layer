using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class CountryDomainEntityDto : IDomainEntityDto<Country>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string IsoCode { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public EntityReference CurrencyRef { get; set; }

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
        public Uri IdentityServiceUrl { get; set; }
    }
}