using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Model.DTOs.DomainEntity
{
    [DataContract]
    public class OrganizationUnitDtoDomainEntityDto : IDomainEntityDto<OrganizationUnitDto>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public EntityReference CreatedByRef { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public EntityReference ModifiedByRef { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }
    }
}