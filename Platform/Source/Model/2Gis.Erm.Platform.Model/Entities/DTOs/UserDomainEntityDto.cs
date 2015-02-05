using System;
using System.Runtime.Serialization;

using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class UserDomainEntityDto : IDomainEntityDto<User>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Account { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public EntityReference DepartmentRef { get; set; }

        [DataMember]
        public EntityReference ParentRef { get; set; }

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
        public string DisplayName { get; set; }

        [DataMember]
        public bool IsServiceUser { get; set; }
    }
}