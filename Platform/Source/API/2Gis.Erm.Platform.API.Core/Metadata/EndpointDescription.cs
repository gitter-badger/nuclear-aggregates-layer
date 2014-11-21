using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Metadata
{
    [DataContract]
    public sealed class EndpointDescription
    {
        [DataMember]
        public string Name { get; set; }
        
        [DataMember]
        public Uri Address { get; set; }

        [DataMember]
        public string ContractName { get; set; }

        [DataMember]
        public string ContractNamespace { get; set; }

        [DataMember]
        public bool Available { get; set; }

        [DataMember]
        public Version Version { get; set; }

        [DataMember]
        public string BusinessLogicAdaptation { get; set; }
    }
}
