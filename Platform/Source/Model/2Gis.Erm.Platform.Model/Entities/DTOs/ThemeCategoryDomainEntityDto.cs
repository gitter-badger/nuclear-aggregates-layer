using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class ThemeCategoryDomainEntityDto : IDomainEntityDto<ThemeCategory>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference ThemeRef { get; set; }

        [DataMember]
        public EntityReference CategoryRef { get; set; }

        [DataMember]
        public EntityReference CreatedByRef { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public EntityReference ModifiedByRef { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        public byte[] Timestamp { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }
    }
}