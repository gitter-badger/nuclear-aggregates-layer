using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Releasing.Releases
{
    [StableContract]
    [DataContract(Namespace = ServiceNamespaces.Releasing.Release201308)]
    public sealed class ExternalReleaseProcessingMessage
    {
        [DataMember]
        public bool IsBlocking { get; set; }

        [DataMember]
        public string MessageType { get; set; }

        [DataMember]
        public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("IsBlocking: {0}, MessageType: {1}, Description: {2}", IsBlocking, MessageType, Description);
        }
    }
}