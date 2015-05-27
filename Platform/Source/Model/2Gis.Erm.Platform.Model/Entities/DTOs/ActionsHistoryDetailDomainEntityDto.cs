using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class ActionsHistoryDetailDomainEntityDto : IDomainEntityDto<ActionsHistoryDetail>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference ActionsHistoryRef { get; set; }

        [DataMember]
        public string PropertyName { get; set; }

        [DataMember]
        public string OriginalValue { get; set; }

        [DataMember]
        public string ModifiedValue { get; set; }

        [DataMember]
        public byte[] Timestamp { get; set; }
    }
}