using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.FirmInfo
{
    [DataContract]
    public class FirmInfoOperationErrorDescription
    {
        public FirmInfoOperationErrorDescription(string message)
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