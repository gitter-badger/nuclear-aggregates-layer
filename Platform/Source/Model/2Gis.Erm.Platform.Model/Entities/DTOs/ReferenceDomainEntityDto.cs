using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class ReferenceDomainEntityDto : IDomainEntityDto<Reference>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string CodeName { get; set; }
    }
}