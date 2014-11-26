using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class DepCardDomainEntityDto : IDomainEntityDto<DepCard>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public bool IsHiddenOrArchived { get; set; }
    }
}