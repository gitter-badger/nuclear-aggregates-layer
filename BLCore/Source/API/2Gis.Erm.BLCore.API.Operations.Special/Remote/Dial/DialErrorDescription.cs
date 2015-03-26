using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Dial
{
    [DataContract(Namespace = ServiceNamespaces.Dialing.Dial201503)]
    public class DialErrorDescription
    {      
        public DialErrorDescription(string message)
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
