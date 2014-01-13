using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Releasing.Releases
{
    [StableContract]
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Release201308)]
    public sealed class ReleaseStartingResult
    {
        [DataMember]
        public bool Succeed { get; set; }

        [DataMember]
        public long ReleaseId { get; set; }

        [DataMember]
        public ReleaseProcessingMessage[] ProcessingMessages { get; set; }
    }
}