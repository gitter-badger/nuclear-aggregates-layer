using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class PerformedBusinessOperationDomainEntityDto : IDomainEntityDto<PerformedBusinessOperation>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public int Operation { get; set; }

        [DataMember]
        public int Descriptor { get; set; }

        [DataMember]
        public string Context { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public long? Parent { get; set; }

        [DataMember]
        public EntityReference UseCaseRef { get; set; }

        [DataMember]
        public string OperationEntities { get; set; }
    }
}