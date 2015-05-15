using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class ActionsHistoryDomainEntityDto : IDomainEntityDto<ActionsHistory>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference EntityRef { get; set; }

        [DataMember]
        public int EntityType { get; set; }

        [DataMember]
        public int ActionType { get; set; }

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
    }
}