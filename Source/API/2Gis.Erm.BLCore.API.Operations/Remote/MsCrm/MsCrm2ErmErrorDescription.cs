using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.MsCrm
{
    [StableContract]
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.MsCrm201309)]
    public sealed class MsCrm2ErmErrorDescription
    {
        public MsCrm2ErmErrorDescription(string message)
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