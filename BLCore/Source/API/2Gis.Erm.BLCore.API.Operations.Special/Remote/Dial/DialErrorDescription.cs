using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Dial
{
    [DataContract]
    public class DialErrorDescription
    {      
        public DialErrorDescription(string message)
        {
            Message = message;
        }

        [DataMember]
        public string Message { get; private set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
