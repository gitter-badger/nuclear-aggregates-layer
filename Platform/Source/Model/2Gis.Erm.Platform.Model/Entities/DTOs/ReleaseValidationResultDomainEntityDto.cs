using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class ReleaseValidationResultDomainEntityDto : IDomainEntityDto<ReleaseValidationResult>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference ReleaseInfoRef { get; set; }

        [DataMember]
        public long? OrderId { get; set; }

        [DataMember]
        public bool IsBlocking { get; set; }

        [DataMember]
        public string RuleCode { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}