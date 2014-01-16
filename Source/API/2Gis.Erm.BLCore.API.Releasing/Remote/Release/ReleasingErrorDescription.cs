using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Releasing.Remote.Release
{
    [StableContract]
    [DataContract(Namespace = ServiceNamespaces.Releasing.Release201308)]
    public sealed class ReleasingErrorDescription
    {
        public ReleasingErrorDescription(string message)
        {
            Message = message;
        }

        [DataMember]
        public string Message { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
