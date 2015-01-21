using System;
using System.Runtime.Serialization;

using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class UserRoleDomainEntityDto : IDomainEntityDto<UserRole>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference UserRef { get; set; }

        [DataMember]
        public EntityReference RoleRef { get; set; }

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
    }
}