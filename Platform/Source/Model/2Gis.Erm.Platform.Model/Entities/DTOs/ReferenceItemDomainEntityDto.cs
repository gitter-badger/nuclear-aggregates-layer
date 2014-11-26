using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class ReferenceItemDomainEntityDto : IDomainEntityDto<ReferenceItem>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public int Code { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public EntityReference ReferenceRef { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }
    }
}