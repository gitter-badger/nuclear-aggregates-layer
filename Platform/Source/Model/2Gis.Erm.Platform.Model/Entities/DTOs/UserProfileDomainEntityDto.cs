using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class UserProfileDomainEntityDto : IDomainEntityDto<UserProfile>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference UserRef { get; set; }

        [DataMember]
        public EntityReference TimeZoneRef { get; set; }

        [DataMember]
        public int CultureInfoLCID { get; set; }

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
        public string Email { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string Mobile { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string Company { get; set; }

        [DataMember]
        public string Position { get; set; }

        [DataMember]
        public DateTime? Birthday { get; set; }

        [DataMember]
        public int Gender { get; set; }

        [DataMember]
        public string PlanetURL { get; set; }

        [DataMember]
        public long ProfileId { get; set; }

        [DataMember]
        public string DomainAccountName { get; set; }
    }
}