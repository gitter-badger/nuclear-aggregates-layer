using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class OrderValidationResultDomainEntityDto : IDomainEntityDto<OrderValidationResult>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public long OrderId { get; set; }

        [DataMember]
        public EntityReference OrderValidationGroupRef { get; set; }

        [DataMember]
        public int OrderValidationType { get; set; }

        [DataMember]
        public bool IsValid { get; set; }
    }
}