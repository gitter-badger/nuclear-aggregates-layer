using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class RolePrivilegeDomainEntityDto : IDomainEntityDto<RolePrivilege>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference RoleRef { get; set; }

        [DataMember]
        public EntityReference PrivilegeRef { get; set; }

        [DataMember]
        public byte Priority { get; set; }

        [DataMember]
        public int Mask { get; set; }

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